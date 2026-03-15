using UnityEngine;

public class ChaseAndShootAI : MonoBehaviour, IEnemyAI
{
    [SerializeField] private float shootRange = 8f;  // この距離以内で射撃
    [SerializeField] private float stopRange  = 3f;  // この距離以内で停止（近すぎる場合）

    public void UpdateAI(EnemyController controller)
    {
        Transform player = controller.PlayerTransform;
        if (player == null) return;

        float dist = Vector2.Distance(controller.transform.position, player.position);

        if (dist > stopRange)
            controller.MoveToward(player.position);
        else
            controller.StopMovement();

        if (dist <= shootRange)
            controller.TryFire();
    }
}
