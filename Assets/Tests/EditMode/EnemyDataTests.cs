using NUnit.Framework;
using UnityEngine;

public class EnemyDataTests
{
    private EnemyData enemyData;

    [SetUp]
    public void SetUp()
    {
        enemyData = ScriptableObject.CreateInstance<EnemyData>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void EnemyData_CanBeCreated()
    {
        Assert.IsNotNull(enemyData);
    }

    [Test]
    public void EnemyData_HasDefaultMoveSpeed()
    {
        Assert.AreEqual(3f, enemyData.moveSpeed);
    }

    [Test]
    public void EnemyData_HasDefaultTurnSpeed()
    {
        Assert.AreEqual(90f, enemyData.turnSpeed);
    }

    [Test]
    public void EnemyData_HasDefaultFireCooldown()
    {
        Assert.AreEqual(1.5f, enemyData.fireCooldown);
    }

    [Test]
    public void EnemyData_HasDefaultBulletSpeed()
    {
        Assert.AreEqual(10f, enemyData.bulletSpeed);
    }

    [Test]
    public void EnemyData_HasDefaultMaxBounces()
    {
        Assert.AreEqual(1, enemyData.maxBounces);
    }

    [Test]
    public void EnemyData_HasDefaultBulletLifetime()
    {
        Assert.AreEqual(5f, enemyData.bulletLifetime);
    }

    [Test]
    public void EnemyData_HasDefaultMaxHp()
    {
        Assert.AreEqual(2, enemyData.maxHp);
    }

    [Test]
    public void EnemyData_HasDefaultDropChance()
    {
        Assert.AreEqual(0.5f, enemyData.dropChance);
    }

    [Test]
    public void EnemyData_DropChance_CanBeSetToZero()
    {
        enemyData.dropChance = 0f;
        Assert.AreEqual(0f, enemyData.dropChance);
    }

    [Test]
    public void EnemyData_DropChance_CanBeSetToOne()
    {
        enemyData.dropChance = 1f;
        Assert.AreEqual(1f, enemyData.dropChance);
    }

    [Test]
    public void EnemyData_MoveSpeed_CanBeModified()
    {
        enemyData.moveSpeed = 5f;
        Assert.AreEqual(5f, enemyData.moveSpeed);
    }

    [Test]
    public void EnemyData_MaxHp_CanBeModified()
    {
        enemyData.maxHp = 10;
        Assert.AreEqual(10, enemyData.maxHp);
    }

    [Test]
    public void EnemyData_DropTable_CanBeNull()
    {
        enemyData.dropTable = null;
        Assert.IsNull(enemyData.dropTable);
    }

    [Test]
    public void EnemyData_DropTable_CanBeEmpty()
    {
        enemyData.dropTable = new ModuleDropEntry[0];
        Assert.AreEqual(0, enemyData.dropTable.Length);
    }

    [Test]
    public void EnemyData_DropTable_CanHaveMultipleEntries()
    {
        GameObject prefab1 = new GameObject("Module1");
        GameObject prefab2 = new GameObject("Module2");

        enemyData.dropTable = new ModuleDropEntry[]
        {
            new ModuleDropEntry { modulePrefab = prefab1, weight = 1f },
            new ModuleDropEntry { modulePrefab = prefab2, weight = 2f }
        };

        Assert.AreEqual(2, enemyData.dropTable.Length);
        Assert.AreEqual(prefab1, enemyData.dropTable[0].modulePrefab);
        Assert.AreEqual(1f, enemyData.dropTable[0].weight);
        Assert.AreEqual(prefab2, enemyData.dropTable[1].modulePrefab);
        Assert.AreEqual(2f, enemyData.dropTable[1].weight);

        Object.DestroyImmediate(prefab1);
        Object.DestroyImmediate(prefab2);
    }
}

public class ModuleDropEntryTests
{
    [Test]
    public void ModuleDropEntry_CanBeCreated()
    {
        var entry = new ModuleDropEntry();
        Assert.IsNotNull(entry);
    }

    [Test]
    public void ModuleDropEntry_HasDefaultWeight()
    {
        var entry = new ModuleDropEntry();
        Assert.AreEqual(1f, entry.weight);
    }

    [Test]
    public void ModuleDropEntry_Weight_CanBeModified()
    {
        var entry = new ModuleDropEntry { weight = 5f };
        Assert.AreEqual(5f, entry.weight);
    }

    [Test]
    public void ModuleDropEntry_ModulePrefab_CanBeSet()
    {
        GameObject prefab = new GameObject("TestModule");
        var entry = new ModuleDropEntry { modulePrefab = prefab };

        Assert.AreEqual(prefab, entry.modulePrefab);

        Object.DestroyImmediate(prefab);
    }

    [Test]
    public void ModuleDropEntry_ModulePrefab_CanBeNull()
    {
        var entry = new ModuleDropEntry { modulePrefab = null };
        Assert.IsNull(entry.modulePrefab);
    }
}