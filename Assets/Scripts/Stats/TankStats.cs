using UnityEngine;

[CreateAssetMenu(fileName = "TankStats", menuName = "TankGame/TankStats")]
public class TankStats : ScriptableObject
{
    [Header("移動")]
    public float moveSpeed    = 6f;
    public float turnSpeed    = 120f;

    [Header("射撃")]
    public float fireCooldown   = 1.0f;
    public float bulletSpeed    = 12f;
    public float bulletLifetime = 5f;

    [Header("弾薬")]
    public int   maxAmmo          = 5;
    public float ammoRechargeTime = 3f; // 1発補充するのにかかる秒数

    [Header("耐久")]
    public int maxHp = 3;
}