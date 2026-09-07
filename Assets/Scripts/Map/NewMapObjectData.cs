using UnityEngine;

[CreateAssetMenu(fileName = "NewMapObjectData", menuName = "Scriptable Objects/Map Object Data")]
public class MapObjectData : ScriptableObject
{
    [Header("ID and Prefab")]
    public string objectID;
    public GameObject objectPrefab;
    public float spawnOffsetY;
    public float weight;
}
