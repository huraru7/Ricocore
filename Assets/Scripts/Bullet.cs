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

    // 発射者のCollider（バウンスするまで自己衝突を無視する）
    private Collider2D ownerCol;

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

    // Get() 時に ownerCollider を最新状態に更新する（プール生成とSetOwner呼び出しの順序ずれを防ぐ）
    public void UpdateOwner(Collider2D ownerCollider)
    {
        ownerCol = ownerCollider;
    }

    void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;

        // プール返却時に IgnoreCollision を必ずリセット
        if (ownerCol != null)
            Physics2D.IgnoreCollision(col, ownerCol, false);
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

        // バウンスするまで発射者との衝突を無視する（発射直後の自己命中を防ぐ）
        if (ownerCol != null)
            Physics2D.IgnoreCollision(col, ownerCol, true);
    }

    // 物理演算が走る前に速度を保存する
    void FixedUpdate()
    {
        prevVelocity = rb.linearVelocity;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (pool == null) { Debug.LogError("[Bullet] pool が未設定です。Init() が呼ばれていません。", this); return; }
            pool.Release(this);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁などの跳弾対象
        if (collision.collider.TryGetComponent<IBounceable>(out _))
        {
            bounceCount++;
            if (bounceCount > maxBounces)
            {
                pool?.Release(this);
                return;
            }

            // 初回バウンス後、発射者への衝突を有効化（自滅を許容）
            if (bounceCount == 1 && ownerCol != null)
                Physics2D.IgnoreCollision(col, ownerCol, false);

            Vector2 reflected = Vector2.Reflect(prevVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflected * speed;
            transform.up = reflected;
            return;
        }

        // ダメージ対象（親階層も検索して IDamageable を取得）
        IDamageable target = collision.collider.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(1);
            pool?.Release(this);
        }
    }
}