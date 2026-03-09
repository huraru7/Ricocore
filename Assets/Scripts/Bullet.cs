using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D  col;
    private IObjectPool<Bullet> pool;
    private int bounceCount;
    private float timer;
    private Vector2 prevVelocity;

    // 発射時に設定されるパラメータ
    private float speed;
    private int   maxBounces;
    private float lifetime;

    // 発射者のCollider（発射直後のみ自己衝突を無視する）
    private Collider2D ownerCol;
    private float      ownerIgnoreTimer;
    private const float OwnerIgnoreDuration = 0.1f;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Init(IObjectPool<Bullet> objectPool, Collider2D ownerCollider)
    {
        pool     = objectPool;
        ownerCol = ownerCollider;
    }

    void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;

        // プール返却時に IgnoreCollision を必ずリセット
        if (ownerCol != null)
            Physics2D.IgnoreCollision(col, ownerCol, false);
        ownerIgnoreTimer = 0f;
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

        // 発射直後のみ発射者との衝突を無視する
        if (ownerCol != null)
        {
            Physics2D.IgnoreCollision(col, ownerCol, true);
            ownerIgnoreTimer = OwnerIgnoreDuration;
        }
    }

    // 物理演算が走る前に速度を保存する
    void FixedUpdate()
    {
        prevVelocity = rb.linearVelocity;
    }

    void Update()
    {
        // 発射者への無視タイマーを更新
        if (ownerIgnoreTimer > 0f)
        {
            ownerIgnoreTimer -= Time.deltaTime;
            if (ownerIgnoreTimer <= 0f && ownerCol != null)
                Physics2D.IgnoreCollision(col, ownerCol, false);  // 解除 → 跳弾後に発射者にも当たれる
        }

        timer -= Time.deltaTime;
        if (timer <= 0f) pool.Release(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁などの跳弾対象
        if (collision.collider.TryGetComponent<IBounceable>(out _))
        {
            bounceCount++;
            if (bounceCount > maxBounces) { pool.Release(this); return; }

            Vector2 reflected = Vector2.Reflect(prevVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflected * speed;
            transform.up = reflected;
            return;
        }

        // ダメージ対象
        if (collision.collider.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(1);
            pool.Release(this);
        }
    }
}
