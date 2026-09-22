using UnityEngine;
using System;
using System.Collections.Generic;

public class MapHandler : MonoBehaviour
{
    private const string SeedKey = "PlayerSeed";

    [Header("Prefabs & Groups")]
    public GameObject groundTile;
    public Transform terrainGroup;
    public Transform enemyGroup;

    [Header("Data Lists")]
    public List<MapObjectData> mapObjects;
    public List<EnemyData> enemyTypes;

    [Header("Map Settings")]
    public int TotalGroundTiles = 200;
    public int spacing = 40;
    private Vector3 initPos = new Vector3(6, 0, 0);

    [Header("Dynamic Enemy Spawning")]
    [SerializeField] private Distance distanceTracker;
    [SerializeField] private float spawnAheadDistance = 60f;
    [SerializeField] private float initialEnemySpawnX = 150f;
    private float lastSpawnX;

    public static event Action<Vector3> OnFirstTileLoaded;

    public int PlayerSeed { get; private set; }
    private System.Random mapPRNG;    

    void Awake()
    {
        CheckSeed();
    }

    void Start()
    { 
        if (distanceTracker == null)
        {
            distanceTracker = FindAnyObjectByType<Distance>();
        }

        ResetMap();
    }

    void Update()
    {
        CheckDynamicEnemySpawn();
    }

    void CheckSeed()
    {
        if (!PlayerPrefs.HasKey(SeedKey))
        {
            int uniquePlayerSeed = UnityEngine.Random.Range(0, 999999);
            PlayerPrefs.SetInt(SeedKey, uniquePlayerSeed);
            PlayerPrefs.Save();
        }

        PlayerSeed = PlayerPrefs.GetInt(SeedKey);
    }

    public void ResetMap()
    {
        ClearMap();
        mapPRNG = new System.Random(PlayerSeed);
        lastSpawnX = initialEnemySpawnX;
        GenerateMap();
    }

    private void ClearMap()
    {
        if (terrainGroup != null)
        {
            for (int i = terrainGroup.childCount - 1; i >= 0; i--)
            {
                Destroy(terrainGroup.GetChild(i).gameObject);
            }
        }

        if (enemyGroup != null)
        {
            for (int i = enemyGroup.childCount - 1; i >= 0; i--)
            {
                Destroy(enemyGroup.GetChild(i).gameObject);
            }
        }
    }

    private void GenerateMap()
    {
        float totalObjectWeight = 0f;
        if (mapObjects != null)
        {
            foreach (var obj in mapObjects)
            {
                totalObjectWeight += obj.weight;
            }
        }

        for (int i = -5; i < TotalGroundTiles; i++)
        {
            Vector3 spawnPos = new Vector3(i * spacing, 0, 0);
            Vector3 tilePos = spawnPos + initPos;
            GameObject newTile = Instantiate(groundTile, tilePos, Quaternion.identity, terrainGroup);
            
            if (i == 0) 
            { 
                OnFirstTileLoaded?.Invoke(tilePos); 
            }

            NumberedPrefab script = newTile.GetComponent<NumberedPrefab>();
            if (script != null)
            {
                script.SetNumber(i + 1);
            }

            MapObjectData selectedObj = null;

            if (mapObjects != null && mapObjects.Count > 0 && totalObjectWeight > 0f)
            {
                float randomRoll = (float)(mapPRNG.NextDouble() * totalObjectWeight);
                float currentSum = 0f;

                foreach (var obj in mapObjects)
                {
                    currentSum += obj.weight;
                    if (randomRoll <= currentSum)
                    {
                        selectedObj = obj;
                        break;
                    }
                }

                if (selectedObj == null)
                {
                    selectedObj = mapObjects[0];
                }
            }

            if (selectedObj != null && selectedObj.objectPrefab != null)
            {
                Vector3 offsetY = new Vector3(0, selectedObj.spawnOffsetY, 0);
                Instantiate(selectedObj.objectPrefab, tilePos + offsetY, Quaternion.identity, terrainGroup);
            }
        }
    }

    private void CheckDynamicEnemySpawn()
    {
        if (distanceTracker == null || enemyTypes == null || enemyTypes.Count == 0) return;

        float currentDist = distanceTracker.dist;

        if (currentDist + spawnAheadDistance >= lastSpawnX)
        {
            SpawnEnemyAtDistance(lastSpawnX, currentDist);
            lastSpawnX += spacing;
        }
    }

    private void SpawnEnemyAtDistance(float spawnX, float currentDist)
    {
        int enemyExtra = mapPRNG.Next(1, 128);
        int mult = Mathf.Max(0, Mathf.FloorToInt(Mathf.Log(enemyExtra)) - 1);

        int maxIndex = 0;
        if (currentDist >= 1000f)
        {
            maxIndex = Mathf.Min(2, enemyTypes.Count - 1);
        }
        else if (currentDist >= 500f)
        {
            maxIndex = Mathf.Min(1, enemyTypes.Count - 1);
        }

        float unlockedTotalWeight = 0f;
        for (int i = 0; i <= maxIndex; i++)
        {
            unlockedTotalWeight += enemyTypes[i].weight;
        }

        for (int j = 0; j < mult; j++)
        {
            EnemyData selectedEnemy = null;

            if (unlockedTotalWeight > 0f)
            {
                float randomRoll = (float)(mapPRNG.NextDouble() * unlockedTotalWeight);
                float currentSum = 0f;

                for (int i = 0; i <= maxIndex; i++)
                {
                    currentSum += enemyTypes[i].weight;
                    if (randomRoll <= currentSum)
                    {
                        selectedEnemy = enemyTypes[i];
                        break;
                    }
                }
            }

            if (selectedEnemy == null)
            {
                selectedEnemy = enemyTypes[0];
            }

            if (selectedEnemy != null && selectedEnemy.prefab != null)
            {
                Vector3 enemyRand = new Vector3(mapPRNG.Next(-10, 10), 30 + (6 * j), 0);
                Vector3 spawnPosition = new Vector3(spawnX, 0f, 0f) + enemyRand;
                Instantiate(selectedEnemy.prefab, spawnPosition, Quaternion.identity, enemyGroup);
            }
        }
    }
}