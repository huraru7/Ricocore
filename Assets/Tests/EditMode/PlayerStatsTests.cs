using NUnit.Framework;
using UnityEngine;

public class PlayerStatsTests
{
    private GameObject testGameObject;
    private PlayerStats playerStats;
    private TankStats testTankStats;

    [SetUp]
    public void SetUp()
    {
        // Create test tank stats
        testTankStats = ScriptableObject.CreateInstance<TankStats>();
        testTankStats.maxHp = 3;
        testTankStats.moveSpeed = 6f;
        testTankStats.turnSpeed = 120f;
        testTankStats.fireCooldown = 0.15f;
        testTankStats.bulletSpeed = 12f;
        testTankStats.maxBounces = 3;
        testTankStats.bulletLifetime = 5f;

        // Create GameObject with PlayerStats
        testGameObject = new GameObject("TestPlayer");
        playerStats = testGameObject.AddComponent<PlayerStats>();

        // Use reflection to set the private baseStats field
        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(playerStats, testTankStats);

        // Call Awake manually
        playerStats.SendMessage("Awake");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testGameObject);
        Object.DestroyImmediate(testTankStats);
    }

    [Test]
    public void CurrentHp_InitializesToMaxHp()
    {
        Assert.AreEqual(3, playerStats.CurrentHp);
    }

    [Test]
    public void MaxHp_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(3, playerStats.MaxHp);
    }

    [Test]
    public void MoveSpeed_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(6f, playerStats.MoveSpeed);
    }

    [Test]
    public void TurnSpeed_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(120f, playerStats.TurnSpeed);
    }

    [Test]
    public void FireCooldown_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(0.15f, playerStats.FireCooldown);
    }

    [Test]
    public void BulletSpeed_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(12f, playerStats.BulletSpeed);
    }

    [Test]
    public void MaxBounces_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(3, playerStats.MaxBounces);
    }

    [Test]
    public void BulletLifetime_ReturnsBaseStatsValue()
    {
        Assert.AreEqual(5f, playerStats.BulletLifetime);
    }

    [Test]
    public void TakeDamage_ReducesCurrentHp()
    {
        playerStats.TakeDamage(1);
        Assert.AreEqual(2, playerStats.CurrentHp);
    }

    [Test]
    public void TakeDamage_HpDoesNotGoBelowZero()
    {
        playerStats.TakeDamage(10);
        Assert.AreEqual(0, playerStats.CurrentHp);
    }

    [Test]
    public void TakeDamage_DeactivatesObjectWhenHpReachesZero()
    {
        playerStats.TakeDamage(3);
        Assert.IsFalse(testGameObject.activeSelf);
    }

    [Test]
    public void Heal_IncreasesCurrentHp()
    {
        playerStats.TakeDamage(2);
        playerStats.Heal(1);
        Assert.AreEqual(2, playerStats.CurrentHp);
    }

    [Test]
    public void Heal_DoesNotExceedMaxHp()
    {
        playerStats.Heal(10);
        Assert.AreEqual(3, playerStats.CurrentHp);
    }

    [Test]
    public void Heal_FromZeroHp_RestoresHp()
    {
        playerStats.TakeDamage(3);
        testGameObject.SetActive(true); // Re-enable for testing
        playerStats.Heal(2);
        Assert.AreEqual(2, playerStats.CurrentHp);
    }

    [Test]
    public void SetModuleBonus_AffectsMoveSpeed()
    {
        StatBonus bonus = new StatBonus { moveSpeed = 2f };
        playerStats.SetModuleBonus(bonus);
        Assert.AreEqual(8f, playerStats.MoveSpeed); // 6 + 2
    }

    [Test]
    public void SetModuleBonus_AffectsTurnSpeed()
    {
        StatBonus bonus = new StatBonus { turnSpeed = 30f };
        playerStats.SetModuleBonus(bonus);
        Assert.AreEqual(150f, playerStats.TurnSpeed); // 120 + 30
    }

    [Test]
    public void SetModuleBonus_AffectsFireCooldown()
    {
        StatBonus bonus = new StatBonus { fireCooldown = -0.05f };
        playerStats.SetModuleBonus(bonus);
        Assert.AreEqual(0.10f, playerStats.FireCooldown, 0.001f); // 0.15 - 0.05
    }

    [Test]
    public void SetModuleBonus_AffectsBulletSpeed()
    {
        StatBonus bonus = new StatBonus { bulletSpeed = 3f };
        playerStats.SetModuleBonus(bonus);
        Assert.AreEqual(15f, playerStats.BulletSpeed); // 12 + 3
    }

    [Test]
    public void SetModuleBonus_AffectsMaxBounces()
    {
        StatBonus bonus = new StatBonus { maxBounces = 2 };
        playerStats.SetModuleBonus(bonus);
        Assert.AreEqual(5, playerStats.MaxBounces); // 3 + 2
    }

    [Test]
    public void SetModuleBonus_AffectsMaxHp()
    {
        StatBonus bonus = new StatBonus { hp = 2 };
        playerStats.SetModuleBonus(bonus);
        Assert.AreEqual(5, playerStats.MaxHp); // 3 + 2
    }

    [Test]
    public void SetSkillBonus_AffectsMoveSpeed()
    {
        StatBonus bonus = new StatBonus { moveSpeed = 1.5f };
        playerStats.SetSkillBonus(bonus);
        Assert.AreEqual(7.5f, playerStats.MoveSpeed); // 6 + 1.5
    }

    [Test]
    public void SetSkillBonus_AffectsMaxHp()
    {
        StatBonus bonus = new StatBonus { hp = 1 };
        playerStats.SetSkillBonus(bonus);
        Assert.AreEqual(4, playerStats.MaxHp); // 3 + 1
    }

    [Test]
    public void ModuleAndSkillBonus_Stack()
    {
        StatBonus moduleBonus = new StatBonus { moveSpeed = 2f, hp = 1 };
        StatBonus skillBonus = new StatBonus { moveSpeed = 1f, hp = 2 };

        playerStats.SetModuleBonus(moduleBonus);
        playerStats.SetSkillBonus(skillBonus);

        Assert.AreEqual(9f, playerStats.MoveSpeed); // 6 + 2 + 1
        Assert.AreEqual(6, playerStats.MaxHp); // 3 + 1 + 2
    }

    [Test]
    public void StatBonus_Zero_HasAllFieldsAsZero()
    {
        StatBonus zero = StatBonus.Zero;
        Assert.AreEqual(0f, zero.moveSpeed);
        Assert.AreEqual(0f, zero.turnSpeed);
        Assert.AreEqual(0f, zero.fireCooldown);
        Assert.AreEqual(0f, zero.bulletSpeed);
        Assert.AreEqual(0, zero.maxBounces);
        Assert.AreEqual(0, zero.hp);
    }

    [Test]
    public void SetModuleBonus_ReplacesOldBonus()
    {
        StatBonus bonus1 = new StatBonus { moveSpeed = 2f };
        StatBonus bonus2 = new StatBonus { moveSpeed = 3f };

        playerStats.SetModuleBonus(bonus1);
        Assert.AreEqual(8f, playerStats.MoveSpeed);

        playerStats.SetModuleBonus(bonus2);
        Assert.AreEqual(9f, playerStats.MoveSpeed);
    }

    [Test]
    public void TakeDamage_ZeroAmount_DoesNotChangeHp()
    {
        int initialHp = playerStats.CurrentHp;
        playerStats.TakeDamage(0);
        Assert.AreEqual(initialHp, playerStats.CurrentHp);
    }

    [Test]
    public void Heal_ZeroAmount_DoesNotChangeHp()
    {
        playerStats.TakeDamage(1);
        int currentHp = playerStats.CurrentHp;
        playerStats.Heal(0);
        Assert.AreEqual(currentHp, playerStats.CurrentHp);
    }

    [Test]
    public void MaxHp_WithBonuses_AffectsHealCap()
    {
        StatBonus bonus = new StatBonus { hp = 2 };
        playerStats.SetModuleBonus(bonus);

        playerStats.TakeDamage(2);
        playerStats.Heal(10);

        // Should heal to new MaxHp (5), not old MaxHp (3)
        Assert.AreEqual(5, playerStats.CurrentHp);
    }
}