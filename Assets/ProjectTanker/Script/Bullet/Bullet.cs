using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Tooltip("BulletSetting")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    private Rigidbody2D _rb;
    private Vector2 _direction;
    private TankBulletManager _owner;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 弾を初期化し、指定した方向へ発射する。
    /// </summary>
    /// <param name="direction">発射方向</param>
    /// <param name="owner">この弾を発射したタンクのBulletManager</param>
    public void Initialize(Vector2 direction, TankBulletManager owner)
    {
        _owner = owner;
        _direction = direction.normalized;
        _rb.linearVelocity = _direction * speed;
    }

    void OnDisable()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out _))
        {
            Vector2 normal = collision.contacts[0].normal;
            _direction = Vector2.Reflect(_direction, normal);
            _rb.linearVelocity = _direction * speed;
            return;
        }

        if (collision.gameObject.TryGetComponent<TankBulletManager>(out var bulletManager))
        {
            bulletManager.TakeDamage(damage);
            if (_owner != null) _owner.ReturnBullet(this);
        }
    }
}
