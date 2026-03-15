using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(EnemyStats))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;

    // IEnemyAI を実装した MonoBehaviour をアタッチして差し替える
    [SerializeField] private MonoBehaviour aiComponent;
    private IEnemyAI currentAI;

    private Rigidbody2D rb;
    private float nextFireTime;

    public Transform PlayerTransform { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Debug.Assert(turret     != null, $"[EnemyController] turret が未設定です: {gameObject.name}");
        Debug.Assert(firePoint  != null, $"[EnemyController] firePoint が未設定です: {gameObject.name}");
        Debug.Assert(bulletPool != null, $"[EnemyController] bulletPool が未設定です: {gameObject.name}");

        currentAI = aiComponent as IEnemyAI;
        if (aiComponent != null && currentAI == null)
            Debug.LogError($"[EnemyController] aiComponent が IEnemyAI を実装していません: {aiComponent.GetType().Name}", this);
    }

    void Start()
    {
        // Tagを使わずシーン内の PlayerStats からプレイヤーを取得
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
            PlayerTransform = player.transform;
        else
            Debug.LogWarning("[EnemyController] シーン内に PlayerStats が見つかりません。", this);

        bulletPool.SetOwnerCollider(GetComponent<Collider2D>());
    }

    void Update()
    {
        if (PlayerTransform == null)
        {
            StopMovement();
            return;
        }
        AimTurretAt(PlayerTransform.position);
        currentAI?.UpdateAI(this);
    }

    // --- AIから呼ばれるAPI ---

    public void MoveToward(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);

        float turnDir = Mathf.Sign(angleDiff);
        rb.angularVelocity = -turnDir * enemyStats.TurnSpeed;

        // ある程度向いていたら前進
        if (Mathf.Abs(angleDiff) < 30f)
            rb.linearVelocity = transform.up * enemyStats.MoveSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void TryFire()
    {
        if (Time.time < nextFireTime) return;
        bulletPool.Get(firePoint.position, firePoint.rotation, enemyStats.BulletSpeed, enemyStats.BulletLifetime);
        nextFireTime = Time.time + enemyStats.FireCooldown;
    }

    // --- 内部処理 ---

    private void AimTurretAt(Vector2 targetPos)
    {
        TurretHelper.AimAt(turret, targetPos);
    }
}
