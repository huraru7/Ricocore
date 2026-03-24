using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1.0f)] private float castRadius = 0.15f;
    [SerializeField] private int ignoreOwnerFrames = 3;

    private IObjectPool<Bullet> pool;
    private Vector2 moveDirection;
    private float speed;
    private float lifetime;
    private float timer;

    // 発射者のCollider（無視期間中にスキップ）
    private Collider2D ownerCol;
    private int ignoreOwnerFramesRemaining;

    // CircleCast 用バッファとフィルタ（GC Alloc 防止）
    private static readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[10];
    private static readonly ContactFilter2D castFilter = new() { useTriggers = true };

    public void Init(IObjectPool<Bullet> objectPool)
    {
        pool = objectPool;
    }

    public void UpdateOwner(Collider2D ownerCollider)
    {
        ownerCol = ownerCollider;
    }

    // BulletPool が position/rotation を設定した後に呼ぶ
    public void Launch(float bulletSpeed, float bulletLifetime)
    {
        speed     = bulletSpeed;
        lifetime  = bulletLifetime;
        timer     = 0f;
        moveDirection = transform.up;
        ignoreOwnerFramesRemaining = ignoreOwnerFrames;
    }

    void Update()
    {
        // owner無視フレームを毎フレーム減算
        if (ignoreOwnerFramesRemaining > 0)
            ignoreOwnerFramesRemaining--;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            if (pool == null) { Debug.LogError("[Bullet] pool が未設定です。Init() が呼ばれていません。", this); return; }
            pool.Release(this);
            return;
        }

        float step = speed * Time.deltaTime;

        // 全ヒットを取得（GC Alloc なし）
        int hitCount = Physics2D.CircleCast(
            transform.position, castRadius, moveDirection, castFilter, hitBuffer, step);

        // owner無視期間中はownerColをスキップして最初の有効ヒットを探す
        RaycastHit2D hit = default;
        for (int i = 0; i < hitCount; i++)
        {
            if (ignoreOwnerFramesRemaining > 0 && hitBuffer[i].collider == ownerCol)
                continue;
            hit = hitBuffer[i];
            break;
        }

        if (!hit)
        {
            // 衝突なし: 等速直線移動
            transform.position += (Vector3)(moveDirection * step);
        }
        else if (hit.collider.GetComponentInParent<IBounceable>() != null)
        {
            // 壁: 反射。反射後は即座にowner衝突を有効化
            moveDirection = Vector2.Reflect(moveDirection, hit.normal);
            transform.up = moveDirection;
            transform.position = hit.centroid + hit.normal * 0.01f;
            ignoreOwnerFramesRemaining = 0;
        }
        else
        {
            // 敵・プレイヤー: ダメージ & 消滅
            hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(1);
            pool?.Release(this);
        }
    }

    // Scene ビューで castRadius を可視化（調節の目安に）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, castRadius);
    }
}
