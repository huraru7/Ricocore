using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Integration tests that verify multiple components work together correctly
/// </summary>
public class IntegrationTests
{
    [Test]
    public void EnemyStats_WithEnemyData_PropertiesMatchData()
    {
        // Create enemy data
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.moveSpeed = 5f;
        data.turnSpeed = 100f;
        data.fireCooldown = 2f;
        data.bulletSpeed = 15f;
        data.maxBounces = 3;
        data.bulletLifetime = 10f;
        data.maxHp = 10;

        // Create enemy with stats
        GameObject enemy = new GameObject("Enemy");
        enemy.AddComponent<BoxCollider2D>();
        var stats = enemy.AddComponent<EnemyStats>();

        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(stats, data);

        stats.SendMessage("Awake");

        // Verify all properties are correctly passed through
        Assert.AreEqual(10, stats.CurrentHp);
        Assert.AreEqual(5f, stats.MoveSpeed);
        Assert.AreEqual(100f, stats.TurnSpeed);
        Assert.AreEqual(2f, stats.FireCooldown);
        Assert.AreEqual(15f, stats.BulletSpeed);
        Assert.AreEqual(3, stats.MaxBounces);
        Assert.AreEqual(10f, stats.BulletLifetime);

        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void PlayerStats_WithBonuses_CalculatesCorrectTotalStats()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 3;
        tankStats.moveSpeed = 6f;
        tankStats.turnSpeed = 120f;
        tankStats.fireCooldown = 0.15f;
        tankStats.bulletSpeed = 12f;
        tankStats.maxBounces = 3;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);

        stats.SendMessage("Awake");

        // Apply module bonus
        StatBonus moduleBonus = new StatBonus
        {
            hp = 2,
            moveSpeed = 2f,
            turnSpeed = 20f,
            fireCooldown = -0.05f,
            bulletSpeed = 3f,
            maxBounces = 1
        };
        stats.SetModuleBonus(moduleBonus);

        // Apply skill bonus
        StatBonus skillBonus = new StatBonus
        {
            hp = 1,
            moveSpeed = 1f,
            turnSpeed = 10f,
            fireCooldown = -0.02f,
            bulletSpeed = 2f,
            maxBounces = 1
        };
        stats.SetSkillBonus(skillBonus);

        // Verify totals
        Assert.AreEqual(6, stats.MaxHp); // 3 + 2 + 1
        Assert.AreEqual(9f, stats.MoveSpeed); // 6 + 2 + 1
        Assert.AreEqual(150f, stats.TurnSpeed); // 120 + 20 + 10
        Assert.AreEqual(0.08f, stats.FireCooldown, 0.001f); // 0.15 - 0.05 - 0.02
        Assert.AreEqual(17f, stats.BulletSpeed); // 12 + 3 + 2
        Assert.AreEqual(5, stats.MaxBounces); // 3 + 1 + 1

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void IDamageable_PlayerAndEnemy_BothImplementCorrectly()
    {
        // Create player
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 5;

        GameObject player = new GameObject("Player");
        var playerStats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(playerStats, tankStats);
        playerStats.SendMessage("Awake");

        // Create enemy
        var enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.maxHp = 3;

        GameObject enemy = new GameObject("Enemy");
        enemy.AddComponent<BoxCollider2D>();
        var enemyStats = enemy.AddComponent<EnemyStats>();

        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(enemyStats, enemyData);
        enemyStats.SendMessage("Awake");

        // Test both as IDamageable
        IDamageable playerDamageable = playerStats;
        IDamageable enemyDamageable = enemyStats;

        Assert.IsNotNull(playerDamageable);
        Assert.IsNotNull(enemyDamageable);

        // Test damage on both
        playerDamageable.TakeDamage(2);
        enemyDamageable.TakeDamage(1);

        Assert.AreEqual(3, playerStats.CurrentHp);
        Assert.AreEqual(2, enemyStats.CurrentHp);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(tankStats);
        Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void EnemyData_DropTable_WeightedSelection()
    {
        var data = ScriptableObject.CreateInstance<EnemyData>();

        GameObject prefab1 = new GameObject("Module1");
        GameObject prefab2 = new GameObject("Module2");
        GameObject prefab3 = new GameObject("Module3");

        data.dropTable = new ModuleDropEntry[]
        {
            new ModuleDropEntry { modulePrefab = prefab1, weight = 1f },
            new ModuleDropEntry { modulePrefab = prefab2, weight = 2f },
            new ModuleDropEntry { modulePrefab = prefab3, weight = 3f }
        };

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var entry in data.dropTable)
        {
            totalWeight += entry.weight;
        }

        Assert.AreEqual(6f, totalWeight);

        // Verify each entry is valid
        Assert.AreEqual(prefab1, data.dropTable[0].modulePrefab);
        Assert.AreEqual(prefab2, data.dropTable[1].modulePrefab);
        Assert.AreEqual(prefab3, data.dropTable[2].modulePrefab);

        Object.DestroyImmediate(prefab1);
        Object.DestroyImmediate(prefab2);
        Object.DestroyImmediate(prefab3);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void StatBonus_MultipleApplications_WorkCorrectly()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 3;
        tankStats.moveSpeed = 5f;

        GameObject player = new GameObject("Player");
        var stats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(stats, tankStats);
        stats.SendMessage("Awake");

        // Apply first module bonus
        stats.SetModuleBonus(new StatBonus { moveSpeed = 2f });
        Assert.AreEqual(7f, stats.MoveSpeed);

        // Replace with different module bonus
        stats.SetModuleBonus(new StatBonus { moveSpeed = 3f });
        Assert.AreEqual(8f, stats.MoveSpeed);

        // Add skill bonus on top
        stats.SetSkillBonus(new StatBonus { moveSpeed = 1f });
        Assert.AreEqual(9f, stats.MoveSpeed);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void TankStats_DefaultValues_AreReasonable()
    {
        var tankStats = ScriptableObject.CreateInstance<TankStats>();

        // Test default values are sensible
        Assert.IsTrue(tankStats.moveSpeed > 0);
        Assert.IsTrue(tankStats.turnSpeed > 0);
        Assert.IsTrue(tankStats.fireCooldown > 0);
        Assert.IsTrue(tankStats.bulletSpeed > 0);
        Assert.IsTrue(tankStats.maxBounces >= 0);
        Assert.IsTrue(tankStats.bulletLifetime > 0);
        Assert.IsTrue(tankStats.maxHp > 0);

        Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void EnemyData_DefaultValues_AreReasonable()
    {
        var enemyData = ScriptableObject.CreateInstance<EnemyData>();

        // Test default values are sensible
        Assert.IsTrue(enemyData.moveSpeed > 0);
        Assert.IsTrue(enemyData.turnSpeed > 0);
        Assert.IsTrue(enemyData.fireCooldown > 0);
        Assert.IsTrue(enemyData.bulletSpeed > 0);
        Assert.IsTrue(enemyData.maxBounces >= 0);
        Assert.IsTrue(enemyData.bulletLifetime > 0);
        Assert.IsTrue(enemyData.maxHp > 0);
        Assert.IsTrue(enemyData.dropChance >= 0 && enemyData.dropChance <= 1);

        Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void Death_PlayerVsEnemy_BehaveDifferently()
    {
        // Create player
        var tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 1;

        GameObject player = new GameObject("Player");
        var playerStats = player.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(playerStats, tankStats);
        playerStats.SendMessage("Awake");

        // Create enemy
        var enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.maxHp = 1;
        enemyData.dropChance = 0f; // No drops for this test

        GameObject enemy = new GameObject("Enemy");
        enemy.AddComponent<BoxCollider2D>();
        var enemyStats = enemy.AddComponent<EnemyStats>();

        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(enemyStats, enemyData);
        enemyStats.SendMessage("Awake");

        // Kill both
        playerStats.TakeDamage(10);
        enemyStats.TakeDamage(10);

        // Both should be deactivated
        Assert.IsFalse(player.activeSelf);
        Assert.IsFalse(enemy.activeSelf);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(tankStats);
        Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void ChaseAndShootAI_DistanceThresholds_AreLogical()
    {
        GameObject aiObj = new GameObject("AI");
        var ai = aiObj.AddComponent<ChaseAndShootAI>();

        var shootRangeField = typeof(ChaseAndShootAI).GetField("shootRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var stopRangeField = typeof(ChaseAndShootAI).GetField("stopRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        float shootRange = (float)shootRangeField.GetValue(ai);
        float stopRange = (float)stopRangeField.GetValue(ai);

        // Shoot range should be greater than stop range
        Assert.IsTrue(shootRange > stopRange,
            "Shoot range should be greater than stop range for logical AI behavior");

        // Both should be positive
        Assert.IsTrue(shootRange > 0);
        Assert.IsTrue(stopRange > 0);

        Object.DestroyImmediate(aiObj);
    }
}