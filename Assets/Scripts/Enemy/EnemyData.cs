using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "TankGame/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("移動")]
    public float moveSpeed = 3f;
    public float turnSpeed = 90f;

    [Header("射撃")]
    public float fireCooldown = 1.5f;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 5f;

    [Header("耐久")]
    public int maxHp = 2;

    [Header("経験値")]
    public int expReward = 10;

    [Header("ドロップ")]
    [Range(0f, 1f)] public float dropChance = 0.5f;
    public ModuleDropEntry[] dropTable;
}

[System.Serializable]
public class ModuleDropEntry
{
    public GameObject modulePrefab;
    public float weight = 1f;
}
