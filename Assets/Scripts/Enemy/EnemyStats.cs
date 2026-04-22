using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;

    public int   CurrentHp      { get; private set; }
    public float MoveSpeed      => data.moveSpeed;
    public float TurnSpeed      => data.turnSpeed;
    public float FireCooldown   => data.fireCooldown;
    public float BulletSpeed    => data.bulletSpeed;
    public float BulletLifetime => data.bulletLifetime;

    void Awake()
    {
        Debug.Assert(data != null, $"[EnemyStats] EnemyData が未設定です: {gameObject.name}");
        CurrentHp = data.maxHp;
    }

    /// <summary>
    /// 敵が死亡したとき発火する静的イベント。引数は付与する経験値量。
    /// ExperienceSystem が購読して経験値を加算する。
    /// </summary>
    public static event System.Action<int> OnEnemyKilled;

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        if (CurrentHp <= 0) Die();
    }

    private void Die()
    {
        OnEnemyKilled?.Invoke(data.expReward);
        TryDropModule();
        gameObject.SetActive(false);
    }

    private void TryDropModule()
    {
        if (data.dropTable == null || data.dropTable.Length == 0) return;
        if (Random.value > data.dropChance) return;

        // ウェイト抽選（weight <= 0 や Prefab 未設定のエントリは除外）
        float totalWeight = 0f;
        foreach (var entry in data.dropTable)
        {
            if (entry.modulePrefab != null && entry.weight > 0f)
                totalWeight += entry.weight;
        }

        if (totalWeight <= 0f) return;

        float roll       = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var entry in data.dropTable)
        {
            if (entry.modulePrefab == null || entry.weight <= 0f) continue;
            cumulative += entry.weight;
            if (roll <= cumulative)
            {
                Instantiate(entry.modulePrefab, transform.position, Quaternion.identity);
                return;
            }
        }
    }
}
