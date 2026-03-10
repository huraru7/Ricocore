using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<Bullet> pool;
    private Collider2D ownerCollider;

    void Awake()
    {
        Debug.Assert(bulletPrefab != null, $"[BulletPool] bulletPrefab が未設定です: {gameObject.name}");

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

    public Bullet Get(Vector2 position, Quaternion rotation, float bulletSpeed, int maxBounces, float bulletLifetime)
    {
        Bullet b = pool.Get();
        b.UpdateOwner(ownerCollider);  // 生成タイミングのずれを防ぐため毎回更新
        b.transform.SetPositionAndRotation(position, rotation);
        b.Launch(bulletSpeed, maxBounces, bulletLifetime);
        return b;
    }
}