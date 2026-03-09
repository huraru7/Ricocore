using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TankStats baseStats;

    public int   CurrentHp    { get; private set; }
    public int   MaxHp        => baseStats.maxHp        + moduleBonus.hp           + skillBonus.hp;
    public float MoveSpeed    => baseStats.moveSpeed    + moduleBonus.moveSpeed    + skillBonus.moveSpeed;
    public float TurnSpeed    => baseStats.turnSpeed    + moduleBonus.turnSpeed    + skillBonus.turnSpeed;
    public float FireCooldown => baseStats.fireCooldown + moduleBonus.fireCooldown + skillBonus.fireCooldown;
    public float BulletSpeed  => baseStats.bulletSpeed  + moduleBonus.bulletSpeed  + skillBonus.bulletSpeed;
    public int   MaxBounces   => baseStats.maxBounces   + moduleBonus.maxBounces   + skillBonus.maxBounces;
    public float BulletLifetime => baseStats.bulletLifetime;

    private StatBonus moduleBonus = StatBonus.Zero;
    private StatBonus skillBonus  = StatBonus.Zero;

    void Awake()
    {
        CurrentHp = MaxHp;
    }

    // モジュールシステムから呼ぶ
    public void SetModuleBonus(StatBonus bonus) { moduleBonus = bonus; }

    // スキルツリーから呼ぶ
    public void SetSkillBonus(StatBonus bonus) { skillBonus = bonus; }

    public void TakeDamage(int amount)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        if (CurrentHp <= 0) gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
    }
}