using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Tank")]
    [SerializeField] private GameObject tankBarrel;

    private Rigidbody2D rb;
    private Vector2 move;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        var kb = Keyboard.current;
        move = new Vector2(
            (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0),
            (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0)
        );
    }
    private void FixedUpdate()
    {
        if (move != Vector2.zero)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, -90f + Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 2f);
        }

        rb.linearVelocity = transform.up * moveSpeed * (move == Vector2.zero ? 0f : 1f);
        // rb.linearVelocity = move.normalized * moveSpeed;
    }
}
