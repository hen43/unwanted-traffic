using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("ID")]
    public string ID;
    public GameObject prefab;
    public float weight;

    [Header("Stats")]
    public int health;
    public int damage;
    public int speed;
    public int cashDrop;

    [Header("Behavior")]
    public AIType aiType;

    
    public enum AIType{
        Chase,
        Shoot
    }
} 