using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody2D _rb;
    private Vector2 _direction;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _direction = Vector2.up; // テスト用
        _rb.linearVelocity = _direction * speed;
    }

    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
        _rb.linearVelocity = _direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Wall>() == null) return;

        Vector2 normal = collision.contacts[0].normal;
        _direction = Vector2.Reflect(_direction, normal);
        _rb.linearVelocity = _direction * speed;
    }
}
