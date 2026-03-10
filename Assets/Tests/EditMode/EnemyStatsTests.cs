using NUnit.Framework;
using UnityEngine;

public class EnemyStatsTests
{
    private GameObject testGameObject;
    private EnemyStats enemyStats;
    private EnemyData testData;

    [SetUp]
    public void SetUp()
    {
        // Create test data
        testData = ScriptableObject.CreateInstance<EnemyData>();
        testData.maxHp = 5;
        testData.moveSpeed = 3f;
        testData.turnSpeed = 90f;
        testData.fireCooldown = 1.5f;
        testData.bulletSpeed = 10f;
        testData.maxBounces = 1;
        testData.bulletLifetime = 5f;
        testData.dropChance = 0.5f;
        testData.dropTable = new ModuleDropEntry[0];

        // Create GameObject with EnemyStats
        testGameObject = new GameObject("TestEnemy");
        testGameObject.AddComponent<BoxCollider2D>();
        enemyStats = testGameObject.AddComponent<EnemyStats>();

        // Use reflection to set the private data field
        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(enemyStats, testData);

        // Call Awake manually
        enemyStats.SendMessage("Awake");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testGameObject);
        Object.DestroyImmediate(testData);
    }

    [Test]
    public void CurrentHp_InitializesToMaxHp()
    {
        Assert.AreEqual(5, enemyStats.CurrentHp);
    }

    [Test]
    public void MoveSpeed_ReturnsDataValue()
    {
        Assert.AreEqual(3f, enemyStats.MoveSpeed);
    }

    [Test]
    public void TurnSpeed_ReturnsDataValue()
    {
        Assert.AreEqual(90f, enemyStats.TurnSpeed);
    }

    [Test]
    public void FireCooldown_ReturnsDataValue()
    {
        Assert.AreEqual(1.5f, enemyStats.FireCooldown);
    }

    [Test]
    public void BulletSpeed_ReturnsDataValue()
    {
        Assert.AreEqual(10f, enemyStats.BulletSpeed);
    }

    [Test]
    public void MaxBounces_ReturnsDataValue()
    {
        Assert.AreEqual(1, enemyStats.MaxBounces);
    }

    [Test]
    public void BulletLifetime_ReturnsDataValue()
    {
        Assert.AreEqual(5f, enemyStats.BulletLifetime);
    }

    [Test]
    public void TakeDamage_ReducesHp()
    {
        enemyStats.TakeDamage(2);
        Assert.AreEqual(3, enemyStats.CurrentHp);
    }

    [Test]
    public void TakeDamage_HpDoesNotGoBelowZero()
    {
        enemyStats.TakeDamage(10);
        Assert.AreEqual(0, enemyStats.CurrentHp);
    }

    [Test]
    public void TakeDamage_DeactivatesObjectWhenHpReachesZero()
    {
        enemyStats.TakeDamage(5);
        Assert.IsFalse(testGameObject.activeSelf);
    }

    [Test]
    public void TakeDamage_MultipleHits_ReducesHpCorrectly()
    {
        enemyStats.TakeDamage(2);
        enemyStats.TakeDamage(1);
        Assert.AreEqual(2, enemyStats.CurrentHp);
    }

    [Test]
    public void TakeDamage_ZeroAmount_DoesNotChangeHp()
    {
        int initialHp = enemyStats.CurrentHp;
        enemyStats.TakeDamage(0);
        Assert.AreEqual(initialHp, enemyStats.CurrentHp);
    }

    [Test]
    public void TakeDamage_NegativeAmount_IncreasesHp()
    {
        enemyStats.TakeDamage(-2);
        // Based on implementation: CurrentHp = Max(0, CurrentHp - amount)
        // If amount is negative, CurrentHp increases
        Assert.AreEqual(7, enemyStats.CurrentHp);
    }

    [Test]
    public void TryDropModule_NoDropTable_DoesNotCrash()
    {
        // This tests that the method handles null/empty drop tables gracefully
        testData.dropTable = null;
        enemyStats.TakeDamage(5); // This calls Die() which calls TryDropModule()
        // If we get here without exception, test passes
        Assert.Pass();
    }

    [Test]
    public void TryDropModule_EmptyDropTable_DoesNotCrash()
    {
        testData.dropTable = new ModuleDropEntry[0];
        enemyStats.TakeDamage(5);
        Assert.Pass();
    }

    [Test]
    public void TryDropModule_ZeroDropChance_DoesNotDrop()
    {
        // Create a mock module prefab
        GameObject modulePrefab = new GameObject("Module");
        testData.dropChance = 0f;
        testData.dropTable = new ModuleDropEntry[]
        {
            new ModuleDropEntry { modulePrefab = modulePrefab, weight = 1f }
        };

        // Set Random seed for deterministic behavior
        Random.InitState(12345);

        int childCount = Object.FindObjectsOfType<GameObject>().Length;
        enemyStats.TakeDamage(5);
        int newChildCount = Object.FindObjectsOfType<GameObject>().Length;

        // Should not instantiate new objects
        Assert.AreEqual(childCount, newChildCount);

        Object.DestroyImmediate(modulePrefab);
    }
}