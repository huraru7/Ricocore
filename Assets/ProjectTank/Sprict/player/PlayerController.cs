using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //do 動いたときに追従して胴体が少しづつプレイヤーの向きを進行方向に向けていく。
    //do 
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}
