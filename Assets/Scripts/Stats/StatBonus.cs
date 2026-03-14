[System.Serializable]
public struct StatBonus
{
    public float moveSpeed;
    public float turnSpeed;
    public float fireCooldown;
    public float bulletSpeed;
    public int   maxBounces;
    public int   hp;
    public int   maxAmmo;

    public static StatBonus Zero => new StatBonus();
}