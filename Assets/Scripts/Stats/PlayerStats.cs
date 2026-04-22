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
    public float BulletLifetime => baseStats.bulletLifetime;
    public int   MaxAmmo        => baseStats.maxAmmo    + moduleBonus.maxAmmo      + skillBonus.maxAmmo;
    public float AmmoRechargeTime => baseStats.ammoRechargeTime;

    public int CurrentAmmo { get; private set; }

    private StatBonus moduleBonus = StatBonus.Zero;
    private StatBonus skillBonus  = StatBonus.Zero;

    private float rechargeTimer;

    /// <summary>UI 購読用リアクティブステート。PlayerState.Subscribe() で変化を受け取る。</summary>
    public PlayerState State { get; } = new();

    void Awake()
    {
        CurrentHp   = MaxHp;
        CurrentAmmo = MaxAmmo;

        // 初期値をステートに反映（Subscribe 時に即座に最新値が届くよう）
        State.CurrentHp.Value   = CurrentHp;
        State.MaxHp.Value       = MaxHp;
        State.CurrentAmmo.Value = CurrentAmmo;
        State.MaxAmmo.Value     = MaxAmmo;
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
                State.CurrentAmmo.Value = CurrentAmmo;
            }
        }
    }

    // 弾を1発消費する。残弾がなければ false を返す
    public bool UseAmmo()
    {
        if (CurrentAmmo <= 0) return false;
        CurrentAmmo--;
        State.CurrentAmmo.Value = CurrentAmmo;
        return true;
    }

    // モジュールシステムから呼ぶ
    public void SetModuleBonus(StatBonus bonus)
    {
        moduleBonus = bonus;
        // MaxHp / MaxAmmo はボーナス変化で変わるのでステートを更新
        State.MaxHp.Value   = MaxHp;
        State.MaxAmmo.Value = MaxAmmo;
    }

    // スキルツリーから呼ぶ
    public void SetSkillBonus(StatBonus bonus)
    {
        skillBonus = bonus;
        State.MaxHp.Value   = MaxHp;
        State.MaxAmmo.Value = MaxAmmo;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        State.CurrentHp.Value = CurrentHp;
        if (CurrentHp <= 0) gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
        State.CurrentHp.Value = CurrentHp;
    }

    void OnDestroy() => State.Dispose();
}
