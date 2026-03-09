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
    public int   MaxBounces     => data.maxBounces;
    public float BulletLifetime => data.bulletLifetime;

    void Awake()
    {
        CurrentHp = data.maxHp;
    }

    public void TakeDamage(int amount)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        if (CurrentHp <= 0) Die();
    }

    private void Die()
    {
        TryDropModule();
        gameObject.SetActive(false);
    }

    private void TryDropModule()
    {
        if (data.dropTable == null || data.dropTable.Length == 0) return;
        if (Random.value > data.dropChance) return;

        // ウェイト抽選
        float totalWeight = 0f;
        foreach (var entry in data.dropTable) totalWeight += entry.weight;

        float roll       = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var entry in data.dropTable)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
            {
                Instantiate(entry.modulePrefab, transform.position, Quaternion.identity);
                return;
            }
        }
    }
}
