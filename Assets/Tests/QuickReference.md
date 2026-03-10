# Test Suite Quick Reference

## Running Tests

### Unity Test Runner
1. Open: `Window > General > Test Runner`
2. Run EditMode tests: Click "Run All" in EditMode tab
3. Run PlayMode tests: Click "Run All" in PlayMode tab

### Expected Results
- All tests should pass ✅
- No compilation errors
- Total execution time: < 2 minutes

## Test Organization

### EditMode Tests (Fast, No Runtime)
- **EnemyStatsTests.cs**: Enemy HP, damage, death, drops
- **PlayerStatsTests.cs**: Player HP, healing, bonuses
- **EnemyDataTests.cs**: ScriptableObject data
- **ChaseAndShootAITests.cs**: AI behavior logic
- **IntegrationTests.cs**: Cross-component testing
- **EdgeCaseTests.cs**: Boundary conditions, extreme values

### PlayMode Tests (Slower, Requires Runtime)
- **BulletTests.cs**: Bullet physics and collisions
- **EnemyControllerTests.cs**: Enemy movement and control
- **EnemyBulletPoolTests.cs**: Object pooling
- **ModulePickupTests.cs**: Item collection

## Test Counts

| Test File | Test Count |
|-----------|------------|
| EnemyStatsTests | 20 |
| PlayerStatsTests | 30 |
| ChaseAndShootAITests | 8 |
| EnemyDataTests | 23 |
| IntegrationTests | 10 |
| EdgeCaseTests | 20 |
| BulletTests | 11 |
| EnemyControllerTests | 10 |
| EnemyBulletPoolTests | 11 |
| ModulePickupTests | 7 |
| **TOTAL** | **150+** |

## Key Test Scenarios

### Critical Paths
✓ Enemy takes damage and dies
✓ Player takes damage and dies
✓ Player heals correctly
✓ Bonuses stack properly
✓ Bullets launch and collide
✓ Enemies chase and shoot player
✓ Module drops and player collects

### Edge Cases
✓ int.MaxValue damage
✓ Negative stat values
✓ Zero values (damage, heal)
✓ Null references
✓ Multiple death calls
✓ MaxHp reduced below CurrentHp
✓ Healing after death

## Common Issues

### Tests Won't Run
1. Check Unity Test Framework is installed
2. Verify assembly definitions are correct
3. Ensure all scripts compile without errors

### Tests Fail
1. Check Unity version (2021.3+ recommended)
2. Verify Physics2D settings
3. Check for conflicting assemblies

### Slow Tests
1. EditMode tests should be fast (< 10s)
2. PlayMode tests may take 1-2 minutes
3. Reduce test iterations if needed

## Test Naming Convention

Format: `[Method]_[Scenario]_[Expected]`

Examples:
- `TakeDamage_WithZeroAmount_DoesNotChangeHp`
- `Heal_WhenAlreadyAtMaxHp_StaysAtMaxHp`
- `Launch_SetsVelocity`

## Modifying Tests

### Adding New Tests
1. Choose appropriate test file (or create new one)
2. Follow existing patterns
3. Add `[Test]` or `[UnityTest]` attribute
4. Write clear test name
5. Include Setup/TearDown if needed

### Updating Tests
1. Run tests before changes
2. Update test to match new behavior
3. Verify all tests still pass
4. Add regression test if fixing bug

## Test Structure

```csharp
[Test]
public void Method_Scenario_Expected()
{
    // Arrange
    var component = SetupComponent();

    // Act
    component.DoSomething();

    // Assert
    Assert.AreEqual(expected, actual);
}
```

## Useful Assertions

```csharp
Assert.AreEqual(expected, actual);
Assert.IsTrue(condition);
Assert.IsFalse(condition);
Assert.IsNull(obj);
Assert.IsNotNull(obj);
Assert.AreEqual(expected, actual, delta); // For floats
Assert.Throws<Exception>(() => code);
Assert.DoesNotThrow(() => code);
```

## Documentation

- **TestReport.md**: Comprehensive test documentation
- **TestSummary.txt**: High-level overview
- **QuickReference.md**: This file

## Contact

For issues or questions about tests, check the test comments or refer to TestReport.md for detailed information.