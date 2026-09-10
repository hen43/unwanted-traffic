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

    public static event Action<Vector3> OnFirstTileLoaded;

    public int PlayerSeed { get; private set; }
    private System.Random mapPRNG;    

    void Awake()
    {
        CheckSeed();
    }

    void Start()
    { 
        ResetMap();
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
        GenerateMap(); // OnFirstTileLoaded will fire inside here and spawn the player
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
        float totalWeight = 0f;
        if (mapObjects != null)
        {
            foreach (var obj in mapObjects)
            {
                totalWeight += obj.weight;
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

            if (mapObjects != null && mapObjects.Count > 0 && totalWeight > 0f)
            {
                float randomRoll = (float)(mapPRNG.NextDouble() * totalWeight);
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

            if (enemyTypes != null && enemyTypes.Count > 0 && enemyTypes[0].prefab != null)
            {
                int enemyExtra = mapPRNG.Next(1, 128);
                int mult = Mathf.Max(0, Mathf.FloorToInt(Mathf.Log(enemyExtra)) - 1);

                for (int j = 0; j < mult; j++)
                {
                    if(i >= 5)
                    {
                        Vector3 enemyRand = new Vector3(mapPRNG.Next(-10, 10), 30 + (6 * j), 0);
                        Instantiate(enemyTypes[0].prefab, tilePos + enemyRand, Quaternion.identity, enemyGroup);    
                    }
                }
            }
        }
    }
}