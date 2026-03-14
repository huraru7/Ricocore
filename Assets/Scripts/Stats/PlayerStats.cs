using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
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
    public int   MaxAmmo        => baseStats.maxAmmo    + moduleBonus.maxAmmo      + skillBonus.maxAmmo;
    public float AmmoRechargeTime => baseStats.ammoRechargeTime;

    public int CurrentAmmo { get; private set; }

    private StatBonus moduleBonus = StatBonus.Zero;
    private StatBonus skillBonus  = StatBonus.Zero;

    private float rechargeTimer;

    void Awake()
    {
        CurrentHp   = MaxHp;
        CurrentAmmo = MaxAmmo;
    }

    void Update()
    {
        if (CurrentAmmo < MaxAmmo)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= AmmoRechargeTime)
            {
                CurrentAmmo++;
                rechargeTimer = 0f;
            }
        }
    }

    // 弾を1発消費する。残弾がなければ false を返す
    public bool UseAmmo()
    {
        if (CurrentAmmo <= 0) return false;
        CurrentAmmo--;
        return true;
    }

    // モジュールシステムから呼ぶ
    public void SetModuleBonus(StatBonus bonus) { moduleBonus = bonus; }

    // スキルツリーから呼ぶ
    public void SetSkillBonus(StatBonus bonus) { skillBonus = bonus; }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        if (CurrentHp <= 0) gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
    }
}