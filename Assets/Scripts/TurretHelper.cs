using UnityEngine;

// PlayerController と EnemyController で共有する砲塔照準計算
public static class TurretHelper
{
    public static void AimAt(Transform turret, Vector2 targetPos, float bodyAngle, float maxTurretAngle)
    {
        Vector2 aimDir = (targetPos - (Vector2)turret.position).normalized;
        float desiredAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;

        float relative = Mathf.DeltaAngle(bodyAngle, desiredAngle);
        float clamped  = Mathf.Clamp(relative, -maxTurretAngle, maxTurretAngle);

        turret.rotation = Quaternion.Euler(0f, 0f, bodyAngle + clamped);
    }
}
