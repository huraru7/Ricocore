using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private IObjectPool<Bullet> pool;
    private int bounceCount;
    private float timer;
    private Vector2 prevVelocity;

    // 発射時に PlayerStats から受け取る
    private float speed;
    private int   maxBounces;
    private float lifetime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(IObjectPool<Bullet> objectPool, Collider2D playerCol)
    {
        pool = objectPool;
        if (playerCol != null)
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCol);
    }

    void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // BulletPool が position/rotation を設定した後に呼ぶ
    public void Launch(float bulletSpeed, int maxBounceCount, float bulletLifetime)
    {
        speed      = bulletSpeed;
        maxBounces = maxBounceCount;
        lifetime   = bulletLifetime;

        bounceCount  = 0;
        timer        = lifetime;
        prevVelocity = transform.up * speed;
        rb.linearVelocity = prevVelocity;
    }

    // 物理演算が走る前に速度を保存する
    void FixedUpdate()
    {
        prevVelocity = rb.linearVelocity;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) pool.Release(this);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.TryGetComponent<IBounceable>(out _)) return;

        bounceCount++;
        if (bounceCount > maxBounces) { pool.Release(this); return; }

        Vector2 reflected = Vector2.Reflect(prevVelocity.normalized, col.contacts[0].normal);
        rb.linearVelocity = reflected * speed;
        transform.up = reflected;
    }
}