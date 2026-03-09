using UnityEngine;
using UnityEngine.Pool;

public class EnemyBulletPool : MonoBehaviour
{
    [SerializeField] private Bullet      bulletPrefab;
    [SerializeField] private EnemyStats  enemyStats;
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxSize         = 20;

    private ObjectPool<Bullet> pool;
    private Collider2D ownerCollider;

    void Awake()
    {
        pool = new ObjectPool<Bullet>(
            createFunc:      CreateBullet,
            actionOnGet:     b => b.gameObject.SetActive(true),
            actionOnRelease: b => b.gameObject.SetActive(false),
            actionOnDestroy: b => Destroy(b.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize:         maxSize
        );
    }

    public void SetOwnerCollider(Collider2D col)
    {
        ownerCollider = col;
    }

    private Bullet CreateBullet()
    {
        Bullet b = Instantiate(bulletPrefab);
        b.Init(pool, ownerCollider);
        return b;
    }

    public Bullet Get(Vector2 position, Quaternion rotation)
    {
        Bullet b = pool.Get();
        b.transform.SetPositionAndRotation(position, rotation);
        b.Launch(enemyStats.BulletSpeed, enemyStats.MaxBounces, enemyStats.BulletLifetime);
        return b;
    }
}
