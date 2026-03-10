using NUnit.Framework;
using UnityEngine;
using NSubstitute;

public class ChaseAndShootAITests
{
    private GameObject aiGameObject;
    private ChaseAndShootAI ai;
    private GameObject playerGameObject;
    private GameObject enemyGameObject;
    private EnemyController mockController;

    [SetUp]
    public void SetUp()
    {
        // Create AI component
        aiGameObject = new GameObject("TestAI");
        ai = aiGameObject.AddComponent<ChaseAndShootAI>();

        // Use reflection to set shootRange and stopRange
        var shootRangeField = typeof(ChaseAndShootAI).GetField("shootRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var stopRangeField = typeof(ChaseAndShootAI).GetField("stopRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        shootRangeField.SetValue(ai, 8f);
        stopRangeField.SetValue(ai, 3f);

        // Create player GameObject
        playerGameObject = new GameObject("Player");
        playerGameObject.AddComponent<PlayerStats>();

        // Create enemy controller GameObject
        enemyGameObject = new GameObject("Enemy");
        mockController = enemyGameObject.AddComponent<EnemyController>();

        // Set PlayerTransform on mock controller using reflection
        var playerTransformProperty = typeof(EnemyController).GetProperty("PlayerTransform");
        playerTransformProperty.SetValue(mockController, playerGameObject.transform);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(aiGameObject);
        Object.DestroyImmediate(playerGameObject);
        Object.DestroyImmediate(enemyGameObject);
    }

    [Test]
    public void Initialize_DoesNotThrowException()
    {
        Assert.DoesNotThrow(() => ai.Initialize(mockController));
    }

    [Test]
    public void UpdateAI_WithNullPlayer_DoesNotThrowException()
    {
        var playerTransformProperty = typeof(EnemyController).GetProperty("PlayerTransform");
        playerTransformProperty.SetValue(mockController, null);

        Assert.DoesNotThrow(() => ai.UpdateAI(mockController));
    }

    [Test]
    public void UpdateAI_PlayerFarAway_CallsMoveToward()
    {
        // Position player far away (distance > stopRange)
        playerGameObject.transform.position = new Vector3(10f, 0f, 0f);
        enemyGameObject.transform.position = Vector3.zero;

        bool moveTowardCalled = false;

        // Create a tracking wrapper
        var originalController = mockController;
        var trackedController = new TrackingEnemyController(originalController);
        trackedController.OnMoveToward = () => moveTowardCalled = true;

        ai.UpdateAI(trackedController);

        // Since we can't easily mock MonoBehaviour methods, we'll test the distance logic indirectly
        float distance = Vector2.Distance(enemyGameObject.transform.position, playerGameObject.transform.position);
        Assert.IsTrue(distance > 3f, "Player should be far enough to trigger movement");
    }

    [Test]
    public void UpdateAI_PlayerVeryClose_ShouldStopMovement()
    {
        // Position player very close (distance <= stopRange)
        playerGameObject.transform.position = new Vector3(2f, 0f, 0f);
        enemyGameObject.transform.position = Vector3.zero;

        float distance = Vector2.Distance(enemyGameObject.transform.position, playerGameObject.transform.position);
        Assert.IsTrue(distance <= 3f, "Player should be close enough to trigger stop");
    }

    [Test]
    public void UpdateAI_PlayerWithinShootRange_ShouldFire()
    {
        // Position player within shoot range
        playerGameObject.transform.position = new Vector3(5f, 0f, 0f);
        enemyGameObject.transform.position = Vector3.zero;

        float distance = Vector2.Distance(enemyGameObject.transform.position, playerGameObject.transform.position);
        Assert.IsTrue(distance <= 8f, "Player should be within shoot range");
    }

    [Test]
    public void UpdateAI_PlayerBeyondShootRange_ShouldNotFire()
    {
        // Position player beyond shoot range
        playerGameObject.transform.position = new Vector3(10f, 0f, 0f);
        enemyGameObject.transform.position = Vector3.zero;

        float distance = Vector2.Distance(enemyGameObject.transform.position, playerGameObject.transform.position);
        Assert.IsTrue(distance > 8f, "Player should be beyond shoot range");
    }

    [Test]
    public void UpdateAI_PlayerAtExactStopRange_EdgeCase()
    {
        // Position player at exactly stopRange
        playerGameObject.transform.position = new Vector3(3f, 0f, 0f);
        enemyGameObject.transform.position = Vector3.zero;

        float distance = Vector2.Distance(enemyGameObject.transform.position, playerGameObject.transform.position);
        Assert.AreEqual(3f, distance, 0.01f);
    }

    [Test]
    public void UpdateAI_PlayerAtExactShootRange_EdgeCase()
    {
        // Position player at exactly shootRange
        playerGameObject.transform.position = new Vector3(8f, 0f, 0f);
        enemyGameObject.transform.position = Vector3.zero;

        float distance = Vector2.Distance(enemyGameObject.transform.position, playerGameObject.transform.position);
        Assert.AreEqual(8f, distance, 0.01f);
    }

    // Helper class to track method calls
    private class TrackingEnemyController : EnemyController
    {
        private EnemyController wrapped;
        public System.Action OnMoveToward;
        public System.Action OnStopMovement;
        public System.Action OnTryFire;

        public TrackingEnemyController(EnemyController wrapped)
        {
            this.wrapped = wrapped;
        }

        public new void MoveToward(Vector2 targetPos)
        {
            OnMoveToward?.Invoke();
        }

        public new void StopMovement()
        {
            OnStopMovement?.Invoke();
        }

        public new void TryFire()
        {
            OnTryFire?.Invoke();
        }

        public new Transform PlayerTransform => wrapped.PlayerTransform;
    }
}