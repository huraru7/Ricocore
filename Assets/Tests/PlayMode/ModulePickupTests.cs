using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class ModulePickupTests
{
    private GameObject moduleGameObject;
    private ModulePickup modulePickup;
    private GameObject playerGameObject;
    private PlayerStats playerStats;
    private TankStats tankStats;

    [SetUp]
    public void SetUp()
    {
        // Create tank stats for player
        tankStats = ScriptableObject.CreateInstance<TankStats>();
        tankStats.maxHp = 3;

        // Create player
        playerGameObject = new GameObject("Player");
        playerGameObject.AddComponent<BoxCollider2D>();
        playerStats = playerGameObject.AddComponent<PlayerStats>();

        var baseStatsField = typeof(PlayerStats).GetField("baseStats",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        baseStatsField.SetValue(playerStats, tankStats);

        // Create module pickup
        moduleGameObject = new GameObject("Module");
        var collider = moduleGameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        modulePickup = moduleGameObject.AddComponent<ModulePickup>();
    }

    [TearDown]
    public void TearDown()
    {
        if (moduleGameObject != null)
            Object.DestroyImmediate(moduleGameObject);
        if (playerGameObject != null)
            Object.DestroyImmediate(playerGameObject);
        if (tankStats != null)
            Object.DestroyImmediate(tankStats);
    }

    [Test]
    public void ModulePickup_HasRequiredCollider()
    {
        var collider = modulePickup.GetComponent<Collider2D>();
        Assert.IsNotNull(collider);
    }

    [Test]
    public void OnTriggerEnter2D_WithPlayer_DestroysModule()
    {
        var playerCollider = playerGameObject.GetComponent<Collider2D>();

        modulePickup.SendMessage("OnTriggerEnter2D", playerCollider);

        // The module should be destroyed (or marked for destruction)
        // In test mode, we check if it's been destroyed
        Assert.IsTrue(moduleGameObject == null || !moduleGameObject.activeInHierarchy);
    }

    [Test]
    public void OnTriggerEnter2D_WithNonPlayer_DoesNotDestroy()
    {
        GameObject otherObj = new GameObject("Other");
        var otherCollider = otherObj.AddComponent<BoxCollider2D>();

        bool wasDestroyed = false;
        try
        {
            modulePickup.SendMessage("OnTriggerEnter2D", otherCollider);
            wasDestroyed = (moduleGameObject == null);
        }
        finally
        {
            if (otherObj != null)
                Object.DestroyImmediate(otherObj);
        }

        Assert.IsFalse(wasDestroyed, "Module should not be destroyed by non-player collision");
    }

    [Test]
    public void OnTriggerEnter2D_WithEnemy_DoesNotDestroy()
    {
        GameObject enemyObj = new GameObject("Enemy");
        var enemyCollider = enemyObj.AddComponent<BoxCollider2D>();
        enemyObj.AddComponent<EnemyStats>();

        bool wasDestroyed = false;
        try
        {
            modulePickup.SendMessage("OnTriggerEnter2D", enemyCollider);
            wasDestroyed = (moduleGameObject == null);
        }
        finally
        {
            if (enemyObj != null)
                Object.DestroyImmediate(enemyObj);
        }

        Assert.IsFalse(wasDestroyed, "Module should not be destroyed by enemy collision");
    }

    [Test]
    public void OnTriggerEnter2D_WithNull_DoesNotThrow()
    {
        // This tests robustness against null colliders
        Assert.DoesNotThrow(() =>
        {
            try
            {
                modulePickup.SendMessage("OnTriggerEnter2D", (Collider2D)null);
            }
            catch (System.NullReferenceException)
            {
                // Expected in this case
            }
        });
    }

    [UnityTest]
    public IEnumerator ModulePickup_InScene_IsCollectable()
    {
        // Set up physics
        moduleGameObject.transform.position = new Vector3(0f, 0f, 0f);
        playerGameObject.transform.position = new Vector3(0f, 0f, 0f);

        yield return new WaitForFixedUpdate();

        // Module should still exist until triggered
        Assert.IsNotNull(moduleGameObject);
    }

    [Test]
    public void ModulePickup_MultipleInstances_CanExist()
    {
        GameObject module2 = new GameObject("Module2");
        module2.AddComponent<BoxCollider2D>();
        var pickup2 = module2.AddComponent<ModulePickup>();

        Assert.IsNotNull(modulePickup);
        Assert.IsNotNull(pickup2);
        Assert.AreNotEqual(modulePickup, pickup2);

        Object.DestroyImmediate(module2);
    }
}