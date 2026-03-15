using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;

    private Rigidbody2D rb;
    private Collider2D col;
    private Camera mainCamera;
    private float nextFireTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        bulletPool.SetOwnerCollider(col);
    }

    void Update()
    {
        AimTurret();

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + playerStats.FireCooldown;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    // W/S: 前進後退、A/D: 車体旋回
    void Move()
    {
        var kb = Keyboard.current;
        float fwd = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        float turn = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);

        rb.linearVelocity = transform.up * (fwd * playerStats.MoveSpeed);
        rb.angularVelocity = -turn * playerStats.TurnSpeed;
    }

    void AimTurret()
    {
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        TurretHelper.AimAt(turret, mouseWorld);
    }

    void Fire()
    {
        if (!playerStats.UseAmmo()) return;
        bulletPool.Get(firePoint.position, firePoint.rotation, playerStats.BulletSpeed, playerStats.BulletLifetime);
    }

    public void TakeDamage(int amount)
    {
        playerStats.TakeDamage(amount);
    }
}