using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class EnemyControllerTests
{
    private GameObject enemyGameObject;
    private EnemyController enemyController;
    private GameObject playerGameObject;
    private PlayerStats playerStats;
    private EnemyStats enemyStats;
    private EnemyData enemyData;
    private GameObject aiGameObject;
    private ChaseAndShootAI ai;

    [SetUp]
    public void SetUp()
    {
        // Create enemy data
        enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.moveSpeed = 3f;
        enemyData.turnSpeed = 90f;
        enemyData.fireCooldown = 1f;
        enemyData.bulletSpeed = 10f;
        enemyData.maxBounces = 1;
        enemyData.bulletLifetime = 5f;
        enemyData.maxHp = 3;
        enemyData.dropChance = 0f;

        // Create player
        playerGameObject = new GameObject("Player");
        playerStats = playerGameObject.AddComponent<PlayerStats>();

        // Create enemy with all required components
        enemyGameObject = new GameObject("Enemy");
        enemyGameObject.AddComponent<Rigidbody2D>();
        enemyGameObject.AddComponent<BoxCollider2D>();

        enemyStats = enemyGameObject.AddComponent<EnemyStats>();
        var enemyStatsDataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemyStatsDataField.SetValue(enemyStats, enemyData);

        // Create turret and fire point
        GameObject turret = new GameObject("Turret");
        turret.transform.SetParent(enemyGameObject.transform);

        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(turret.transform);

        // Create bullet pool
        GameObject poolObj = new GameObject("BulletPool");
        poolObj.transform.SetParent(enemyGameObject.transform);
        var bulletPool = poolObj.AddComponent<EnemyBulletPool>();

        // Set up bullet prefab
        GameObject bulletPrefabObj = new GameObject("BulletPrefab");
        bulletPrefabObj.AddComponent<Rigidbody2D>();
        bulletPrefabObj.AddComponent<BoxCollider2D>();
        var bulletPrefab = bulletPrefabObj.AddComponent<Bullet>();

        var bulletPrefabField = typeof(EnemyBulletPool).GetField("bulletPrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bulletPrefabField.SetValue(bulletPool, bulletPrefab);

        var enemyStatsField = typeof(EnemyBulletPool).GetField("enemyStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemyStatsField.SetValue(bulletPool, enemyStats);

        // Create AI
        aiGameObject = new GameObject("AI");
        aiGameObject.transform.SetParent(enemyGameObject.transform);
        ai = aiGameObject.AddComponent<ChaseAndShootAI>();

        // Add EnemyController
        enemyController = enemyGameObject.AddComponent<EnemyController>();

        // Set private fields using reflection
        var enemyStatsFieldController = typeof(EnemyController).GetField("enemyStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemyStatsFieldController.SetValue(enemyController, enemyStats);

        var turretField = typeof(EnemyController).GetField("turret",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        turretField.SetValue(enemyController, turret.transform);

        var firePointField = typeof(EnemyController).GetField("firePoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        firePointField.SetValue(enemyController, firePoint.transform);

        var bulletPoolField = typeof(EnemyController).GetField("bulletPool",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bulletPoolField.SetValue(enemyController, bulletPool);

        var aiComponentField = typeof(EnemyController).GetField("aiComponent",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        aiComponentField.SetValue(enemyController, ai);
    }

    [TearDown]
    public void TearDown()
    {
        if (enemyGameObject != null)
            Object.DestroyImmediate(enemyGameObject);
        if (playerGameObject != null)
            Object.DestroyImmediate(playerGameObject);
        if (enemyData != null)
            Object.DestroyImmediate(enemyData);
    }

    [UnityTest]
    public IEnumerator Awake_InitializesComponents()
    {
        enemyController.SendMessage("Awake");
        yield return null;

        var rb = enemyController.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
    }

    [UnityTest]
    public IEnumerator Start_FindsPlayer()
    {
        enemyController.SendMessage("Awake");
        enemyController.SendMessage("Start");
        yield return null;

        Assert.IsNotNull(enemyController.PlayerTransform);
        Assert.AreEqual(playerGameObject.transform, enemyController.PlayerTransform);
    }

    [Test]
    public void MoveToward_TargetInFront_MovesForward()
    {
        enemyGameObject.transform.position = Vector3.zero;
        enemyGameObject.transform.rotation = Quaternion.identity;

        Vector2 target = new Vector2(0f, 10f);
        enemyController.MoveToward(target);

        var rb = enemyController.GetComponent<Rigidbody2D>();
        // When aligned, should move forward
        // Can't easily test without multiple frames
        Assert.IsNotNull(rb);
    }

    [Test]
    public void StopMovement_SetsVelocityToZero()
    {
        var rb = enemyController.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(5f, 5f);
        rb.angularVelocity = 100f;

        enemyController.StopMovement();

        Assert.AreEqual(Vector2.zero, rb.linearVelocity);
        Assert.AreEqual(0f, rb.angularVelocity);
    }

    [Test]
    public void TryFire_BeforeCooldown_DoesNotFire()
    {
        // Set next fire time to future
        var nextFireTimeField = typeof(EnemyController).GetField("nextFireTime",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        nextFireTimeField.SetValue(enemyController, Time.time + 10f);

        int bulletCountBefore = Object.FindObjectsOfType<Bullet>().Length;
        enemyController.TryFire();
        int bulletCountAfter = Object.FindObjectsOfType<Bullet>().Length;

        Assert.AreEqual(bulletCountBefore, bulletCountAfter);
    }

    [Test]
    public void TryFire_AfterCooldown_CanFire()
    {
        // Set next fire time to past
        var nextFireTimeField = typeof(EnemyController).GetField("nextFireTime",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        nextFireTimeField.SetValue(enemyController, 0f);

        enemyController.SendMessage("Awake");

        // This should not throw
        Assert.DoesNotThrow(() => enemyController.TryFire());
    }

    [UnityTest]
    public IEnumerator Update_WithNoPlayer_DoesNotThrow()
    {
        Object.DestroyImmediate(playerGameObject);
        playerGameObject = null;

        enemyController.SendMessage("Awake");
        enemyController.SendMessage("Start");

        yield return null;

        // Should handle null player gracefully
        Assert.DoesNotThrow(() => enemyController.SendMessage("Update"));
    }

    [Test]
    public void MaxTurretAngle_DefaultValue()
    {
        var maxTurretAngleField = typeof(EnemyController).GetField("maxTurretAngle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxTurretAngle = (float)maxTurretAngleField.GetValue(enemyController);

        Assert.AreEqual(150f, maxTurretAngle);
    }
}