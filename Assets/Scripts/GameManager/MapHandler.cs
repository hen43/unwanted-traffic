using UnityEngine;
using System;
using System.Collections.Generic;

public class MapHandler : MonoBehaviour
{
    private const string SeedKey = "PlayerSeed";
    public GameObject groundTile;
    public Transform terrainGroup;
    public Transform enemyGroup;
    public List<MapObjectData> mapObjects;
    public List<EnemyData> enemyTypes;
    public int TotalGroundTiles = 200;
    public int spacing = 40;
    private Vector3 initPos = new Vector3(6, 0, 0);

    public int PlayerSeed { get; private set; }

    private System.Random mapPRNG;

    void Awake()
    {
        CheckSeed();
    }

    void Start()
    {
        GenerateMap();
    }

    void CheckSeed()
    {
        if (!PlayerPrefs.HasKey(SeedKey))
        {
            int newSeed = UnityEngine.Random.Range(0, 999999);

            PlayerPrefs.SetInt(SeedKey, newSeed);
            PlayerPrefs.Save();
        }

        PlayerSeed = PlayerPrefs.GetInt(SeedKey);
        
        mapPRNG = new System.Random(PlayerSeed);
    }

    void GenerateMap()
    {
        float totalWeight = 0f;
        if (mapObjects != null)
        {
            foreach (var obj in mapObjects)
            {
                totalWeight += obj.weight;
            }
        }

        for (int i = 0; i < TotalGroundTiles; i++)
        {
            //Ground Tile
            Vector3 spawnPos = new Vector3(i * spacing, 0, 0);
            Vector3 tilePos = spawnPos + initPos;
            GameObject newTile = Instantiate(groundTile, tilePos, Quaternion.identity, terrainGroup);

            NumberedPrefab script = newTile.GetComponent<NumberedPrefab>();
            if (script != null)
            {
                script.SetNumber(i + 1);
            }

            //Map Objects
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

            //Enemy
            int enemyExtra = mapPRNG.Next(1, 128);
            int mult = Mathf.FloorToInt(Mathf.Log((float)enemyExtra)) - 1;
            mult = Mathf.Max(0, mult);

            for (int j = 0; j < mult; j++)
            {
                Vector3 enemyRand = new Vector3(mapPRNG.Next(-10, 10), 30 + (6 * j), 0);
                GameObject newEnemy = Instantiate(enemyTypes[0].prefab, tilePos + enemyRand, Quaternion.identity, enemyGroup);
            }
        }
    }
}