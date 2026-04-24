using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Tank")]
    [SerializeField] private GameObject tank;
    [SerializeField] private GameObject tankBarret;
    [SerializeField] private GameObject tankBoby;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var kb = Keyboard.current;
        var move = new Vector2(
            (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0),
            (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0)
        );
        rb.linearVelocity = move.normalized * moveSpeed;
    }
}
