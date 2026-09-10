using UnityEngine;

public class HordeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject hordePrefab;
    [SerializeField] private float initXOffset = 10f;
    [SerializeField] private float initYOffset = 10f;
    [SerializeField] private float initSpeed = 5f;

    private Horde activeHorde;

    private void OnEnable()
    {
        MapHandler.OnFirstTileLoaded += SpawnHorde;
    }

    private void OnDisable()
    {
        MapHandler.OnFirstTileLoaded -= SpawnHorde;
    }

    public void SpawnHorde(Vector3 firstTilePos)
    {
        Vector3 spawnPoint = firstTilePos + new Vector3(-1 * initXOffset, initYOffset, 0);

        if (activeHorde == null)
        {
            activeHorde = Instantiate(hordePrefab, spawnPoint, Quaternion.identity).GetComponent<Horde>();
        }
        else
        {
            activeHorde.transform.position = spawnPoint;
        }

        activeHorde.setSpeed(initSpeed);
        
    }
}