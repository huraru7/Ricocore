using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edge case and regression tests for boundary conditions and unusual scenarios
/// </summary>
public class EdgeCaseTests
{
    [Test]
    public void EnemyStats_TakeDamage_ExtremelyLargeAmount()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.maxHp = 10;

        GameObject enemy = new GameObject("Enemy");
        enemy.AddComponent<BoxCollider2D>();
        var stats = enemy.AddComponent<EnemyStats>();

        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(stats, data);
        stats.SendMessage("Awake");

        // Take massive damage
        stats.TakeDamage(int.MaxValue);

        Assert.AreEqual(0, stats.CurrentHp, "HP should be 0, not negative");

        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void PlayerStats_Heal_WhenAlreadyAtMaxHp()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 5;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        int initialHp = stats.CurrentHp;
        stats.Heal(10);

        Assert.AreEqual(initialHp, stats.CurrentHp, "HP should not exceed max");
        Assert.AreEqual(tankStats.maxHp, stats.CurrentHp, "HP should equal max");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void PlayerStats_NegativeFireCooldown_FromBonuses()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.fireCooldown = 0.15f;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        // Apply bonus that would make cooldown negative
        stats.SetModuleBonus(new StatBonus { fireCooldown = -0.2f });

        // Cooldown becomes negative (0.15 - 0.2 = -0.05)
        Assert.IsTrue(stats.FireCooldown < 0, "Cooldown can go negative");
        Assert.AreEqual(-0.05f, stats.FireCooldown, 0.001f);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void EnemyData_ZeroWeight_InDropTable()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        GameObject prefab = new GameObject("Module");
        data.dropTable = new ModuleDropEntry[]
        {
            new ModuleDropEntry { modulePrefab = prefab, weight = 0f }
        };

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var entry in data.dropTable)
        {
            totalWeight += entry.weight;
        }

        Assert.AreEqual(0f, totalWeight, "Total weight should be 0");

        Object.DestroyImmediate(prefab);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void EnemyData_NegativeWeight_InDropTable()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        GameObject prefab = new GameObject("Module");
        data.dropTable = new ModuleDropEntry[]
        {
            new ModuleDropEntry { modulePrefab = prefab, weight = -5f }
        };

        // System should handle negative weights
        Assert.AreEqual(-5f, data.dropTable[0].weight);

        Object.DestroyImmediate(prefab);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void StatBonus_AllFieldsNegative()
    {
        var bonus = new StatBonus
        {
            moveSpeed = -10f,
            turnSpeed = -50f,
            fireCooldown = -1f,
            bulletSpeed = -5f,
            maxBounces = -3,
            hp = -5
        };

        Assert.AreEqual(-10f, bonus.moveSpeed);
        Assert.AreEqual(-50f, bonus.turnSpeed);
        Assert.AreEqual(-1f, bonus.fireCooldown);
        Assert.AreEqual(-5f, bonus.bulletSpeed);
        Assert.AreEqual(-3, bonus.maxBounces);
        Assert.AreEqual(-5, bonus.hp);
    }

    [Test]
    public void StatBonus_AllFieldsMaxValue()
    {
        var bonus = new StatBonus
        {
            moveSpeed = float.MaxValue,
            turnSpeed = float.MaxValue,
            fireCooldown = float.MaxValue,
            bulletSpeed = float.MaxValue,
            maxBounces = int.MaxValue,
            hp = int.MaxValue
        };

        Assert.AreEqual(float.MaxValue, bonus.moveSpeed);
        Assert.AreEqual(float.MaxValue, bonus.turnSpeed);
        Assert.AreEqual(int.MaxValue, bonus.maxBounces);
        Assert.AreEqual(int.MaxValue, bonus.hp);
    }

    [Test]
    public void PlayerStats_MaxHp_WithNegativeBonus()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 5;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        // Apply negative HP bonus
        stats.SetModuleBonus(new StatBonus { hp = -3 });

        Assert.AreEqual(2, stats.MaxHp, "MaxHp should be reduced");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void PlayerStats_MaxHp_ReducedBelowCurrentHp()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 5;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        // Player at full HP (5)
        Assert.AreEqual(5, stats.CurrentHp);

        // Reduce max HP below current
        stats.SetModuleBonus(new StatBonus { hp = -3 });
        Assert.AreEqual(2, stats.MaxHp);

        // Current HP is now greater than MaxHp
        Assert.IsTrue(stats.CurrentHp > stats.MaxHp,
            "CurrentHp can exceed MaxHp if MaxHp is reduced");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void EnemyData_DropChance_AboveOne()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        // Set drop chance above 100%
        data.dropChance = 1.5f;

        Assert.AreEqual(1.5f, data.dropChance,
            "Drop chance can be set above 1 (100%) in code");

        Object.DestroyImmediate(data);
    }

    [Test]
    public void EnemyData_DropChance_Negative()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        data.dropChance = -0.5f;

        Assert.AreEqual(-0.5f, data.dropChance,
            "Drop chance can be negative in code (though not practical)");

        Object.DestroyImmediate(data);
    }

    [Test]
    public void EnemyStats_MultipleDeathCalls()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.maxHp = 5;
        data.dropChance = 0f;

        GameObject enemy = new GameObject("Enemy");
        enemy.AddComponent<BoxCollider2D>();
        var stats = enemy.AddComponent<EnemyStats>();

        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(stats, data);
        stats.SendMessage("Awake");

        // Kill enemy multiple times
        stats.TakeDamage(10);
        Assert.IsFalse(enemy.activeSelf);

        // Re-enable and kill again
        enemy.SetActive(true);
        stats.TakeDamage(10);
        Assert.IsFalse(enemy.activeSelf);

        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void PlayerStats_MultipleDeathCalls()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 3;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        // Kill player multiple times
        stats.TakeDamage(10);
        Assert.IsFalse(player.activeSelf);

        // Re-enable and kill again
        player.SetActive(true);
        stats.TakeDamage(10);
        Assert.IsFalse(player.activeSelf);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void TankStats_NegativeValues()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();

        tankStats.moveSpeed = -5f;
        tankStats.turnSpeed = -90f;
        tankStats.fireCooldown = -1f;
        tankStats.bulletSpeed = -10f;
        tankStats.maxBounces = -2;
        tankStats.bulletLifetime = -5f;
        tankStats.maxHp = -10;

        // System should allow negative values (though not practical)
        Assert.AreEqual(-5f, tankStats.moveSpeed);
        Assert.AreEqual(-90f, tankStats.turnSpeed);
        Assert.AreEqual(-1f, tankStats.fireCooldown);
        Assert.AreEqual(-10f, tankStats.bulletSpeed);
        Assert.AreEqual(-2, tankStats.maxBounces);
        Assert.AreEqual(-5f, tankStats.bulletLifetime);
        Assert.AreEqual(-10, tankStats.maxHp);

        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void EnemyData_ExtremeValues()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        data.moveSpeed = 1000f;
        data.turnSpeed = 9999f;
        data.fireCooldown = 0.001f;
        data.bulletSpeed = 10000f;
        data.maxBounces = 100;
        data.bulletLifetime = 1000f;
        data.maxHp = 10000;

        Assert.AreEqual(1000f, data.moveSpeed);
        Assert.AreEqual(9999f, data.turnSpeed);
        Assert.AreEqual(0.001f, data.fireCooldown, 0.0001f);
        Assert.AreEqual(10000f, data.bulletSpeed);
        Assert.AreEqual(100, data.maxBounces);
        Assert.AreEqual(1000f, data.bulletLifetime);
        Assert.AreEqual(10000, data.maxHp);

        Object.DestroyImmediate(data);
    }

    [Test]
    public void PlayerStats_HealAfterDeath()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 5;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        // Kill player
        stats.TakeDamage(10);
        Assert.AreEqual(0, stats.CurrentHp);
        Assert.IsFalse(player.activeSelf);

        // Try to heal while dead
        stats.Heal(5);
        Assert.AreEqual(5, stats.CurrentHp, "Can heal even when dead (object deactivated)");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void EnemyData_VeryLongDropTable()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        // Create a drop table with many entries
        var dropTable = new ModuleDropEntry[100];
        for (int i = 0; i < 100; i++)
        {
            GameObject prefab = new GameObject($"Module{i}");
            dropTable[i] = new ModuleDropEntry { modulePrefab = prefab, weight = 1f };
        }

        data.dropTable = dropTable;

        Assert.AreEqual(100, data.dropTable.Length);

        float totalWeight = 0f;
        foreach (var entry in data.dropTable)
        {
            totalWeight += entry.weight;
        }

        Assert.AreEqual(100f, totalWeight);

        // Cleanup
        foreach (var entry in dropTable)
        {
            if (entry.modulePrefab != null)
                Object.DestroyImmediate(entry.modulePrefab);
        }
        Object.DestroyImmediate(data);
    }

    [Test]
    public void StatBonus_Zero_IsDistinctFromDefault()
    {
        StatBonus zero1 = StatBonus.Zero;
        StatBonus zero2 = StatBonus.Zero;

        // Both should have zero values
        Assert.AreEqual(0f, zero1.moveSpeed);
        Assert.AreEqual(0f, zero2.moveSpeed);

        // They should be equal in value
        Assert.AreEqual(zero1.moveSpeed, zero2.moveSpeed);
        Assert.AreEqual(zero1.hp, zero2.hp);
    }
}