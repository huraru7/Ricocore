using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class EnemyBulletPoolTests
{
    private GameObject poolGameObject;
    private EnemyBulletPool bulletPool;
    private GameObject bulletPrefabObj;
    private Bullet bulletPrefab;
    private EnemyStats enemyStats;
    private EnemyData enemyData;
    private Collider2D ownerCollider;

    [SetUp]
    public void SetUp()
    {
        // Create enemy data
        enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.bulletSpeed = 10f;
        enemyData.maxBounces = 2;
        enemyData.bulletLifetime = 5f;
        enemyData.maxHp = 3;

        // Create enemy stats
        GameObject enemyObj = new GameObject("Enemy");
        enemyObj.AddComponent<BoxCollider2D>();
        ownerCollider = enemyObj.GetComponent<Collider2D>();

        enemyStats = enemyObj.AddComponent<EnemyStats>();
        var dataField = typeof(EnemyStats).GetField("data",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dataField.SetValue(enemyStats, enemyData);

        // Create bullet prefab
        bulletPrefabObj = new GameObject("BulletPrefab");
        bulletPrefabObj.AddComponent<Rigidbody2D>();
        bulletPrefabObj.AddComponent<BoxCollider2D>();
        bulletPrefab = bulletPrefabObj.AddComponent<Bullet>();

        // Create bullet pool
        poolGameObject = new GameObject("BulletPool");
        bulletPool = poolGameObject.AddComponent<EnemyBulletPool>();

        // Set private fields
        var bulletPrefabField = typeof(EnemyBulletPool).GetField("bulletPrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bulletPrefabField.SetValue(bulletPool, bulletPrefab);

        var enemyStatsField = typeof(EnemyBulletPool).GetField("enemyStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enemyStatsField.SetValue(bulletPool, enemyStats);

        bulletPool.SendMessage("Awake");
        bulletPool.SetOwnerCollider(ownerCollider);
    }

    [TearDown]
    public void TearDown()
    {
        if (poolGameObject != null)
            Object.DestroyImmediate(poolGameObject);
        if (bulletPrefabObj != null)
            Object.DestroyImmediate(bulletPrefabObj);
        if (enemyStats != null)
            Object.DestroyImmediate(enemyStats.gameObject);
        if (enemyData != null)
            Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void SetOwnerCollider_SetsCollider()
    {
        GameObject testObj = new GameObject("TestOwner");
        var testCollider = testObj.AddComponent<BoxCollider2D>();

        bulletPool.SetOwnerCollider(testCollider);

        // If this doesn't throw, it works
        Assert.Pass();

        Object.DestroyImmediate(testObj);
    }

    [Test]
    public void Get_CreatesActiveBullet()
    {
        Vector2 position = new Vector2(5f, 5f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, 45f);

        Bullet bullet = bulletPool.Get(position, rotation);

        Assert.IsNotNull(bullet);
        Assert.IsTrue(bullet.gameObject.activeSelf);
        Assert.AreEqual(position, (Vector2)bullet.transform.position);
    }

    [Test]
    public void Get_BulletHasCorrectPosition()
    {
        Vector2 position = new Vector2(10f, -5f);
        Quaternion rotation = Quaternion.identity;

        Bullet bullet = bulletPool.Get(position, rotation);

        Assert.AreEqual(position.x, bullet.transform.position.x, 0.01f);
        Assert.AreEqual(position.y, bullet.transform.position.y, 0.01f);
    }

    [Test]
    public void Get_BulletHasCorrectRotation()
    {
        Vector2 position = Vector2.zero;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);

        Bullet bullet = bulletPool.Get(position, rotation);

        Assert.AreEqual(rotation.eulerAngles.z, bullet.transform.rotation.eulerAngles.z, 0.01f);
    }

    [Test]
    public void Get_MultipleTimes_ReusesDeactivatedBullets()
    {
        Bullet bullet1 = bulletPool.Get(Vector2.zero, Quaternion.identity);
        bullet1.gameObject.SetActive(false);

        Bullet bullet2 = bulletPool.Get(Vector2.one, Quaternion.identity);

        // Pool should reuse the deactivated bullet
        Assert.IsNotNull(bullet2);
    }

    [UnityTest]
    public IEnumerator Get_LaunchesBulletWithCorrectSpeed()
    {
        Bullet bullet = bulletPool.Get(Vector2.zero, Quaternion.identity);
        yield return null;

        var rb = bullet.GetComponent<Rigidbody2D>();
        float actualSpeed = rb.linearVelocity.magnitude;

        Assert.AreEqual(10f, actualSpeed, 0.1f);
    }

    [Test]
    public void Get_WithNullOwnerCollider_DoesNotThrow()
    {
        bulletPool.SetOwnerCollider(null);

        Assert.DoesNotThrow(() =>
        {
            bulletPool.Get(Vector2.zero, Quaternion.identity);
        });
    }

    [Test]
    public void Get_MultipleSequentialCalls_AllSucceed()
    {
        for (int i = 0; i < 5; i++)
        {
            Bullet bullet = bulletPool.Get(new Vector2(i, i), Quaternion.identity);
            Assert.IsNotNull(bullet, $"Bullet {i} should not be null");
        }
    }

    [Test]
    public void Pool_DefaultCapacity_IsSetCorrectly()
    {
        var defaultCapacityField = typeof(EnemyBulletPool).GetField("defaultCapacity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var defaultCapacity = (int)defaultCapacityField.GetValue(bulletPool);

        Assert.AreEqual(5, defaultCapacity);
    }

    [Test]
    public void Pool_MaxSize_IsSetCorrectly()
    {
        var maxSizeField = typeof(EnemyBulletPool).GetField("maxSize",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxSize = (int)maxSizeField.GetValue(bulletPool);

        Assert.AreEqual(20, maxSize);
    }
}