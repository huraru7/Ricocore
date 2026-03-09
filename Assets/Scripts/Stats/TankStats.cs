using UnityEngine;

[CreateAssetMenu(fileName = "TankStats", menuName = "TankGame/TankStats")]
public class TankStats : ScriptableObject
{
    [Header("移動")]
    public float moveSpeed    = 6f;
    public float turnSpeed    = 120f;

    [Header("射撃")]
    public float fireCooldown   = 0.15f;
    public float bulletSpeed    = 12f;
    public int   maxBounces     = 3;
    public float bulletLifetime = 5f;

    [Header("耐久")]
    public int maxHp = 3;
}