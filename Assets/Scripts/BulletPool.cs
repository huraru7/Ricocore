using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<Bullet> pool;
    private Collider2D playerCollider;

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

    public void SetPlayerCollider(Collider2D col)
    {
        playerCollider = col;
    }

    private Bullet CreateBullet()
    {
        Bullet b = Instantiate(bulletPrefab);
        b.Init(pool, playerCollider);
        return b;
    }

    public Bullet Get(Vector2 position, Quaternion rotation)
    {
        Bullet b = pool.Get();
        b.transform.SetPositionAndRotation(position, rotation);
        b.Launch(playerStats.BulletSpeed, playerStats.MaxBounces, playerStats.BulletLifetime);
        return b;
    }
}