
# Testing with SOAR

One of the primary advantages of a Scriptable Object Architecture is that it makes code significantly easier to test. By decoupling components, logic can be tested in isolation without needing to build complex scenes or have other systems present.

This guide demonstrates how to write unit tests for components that use SOAR assets using Unity's built-in Test Framework.

## The Component Under Test

Consider a simple `Player` component that listens to a `GameEvent` to take damage. The goal is to test this component's logic without needing a full game environment.

Here is the component that will be tested:

```csharp
// File: Player.cs
using Soar.Events;
using Soar.Variables;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private FloatGameEvent onPlayerDamaged;

    private void Start()
    {
        // Ensure health is full on start
        playerHealth.Value = 100f;

        // Subscribe to the damage event
        onPlayerDamaged.Subscribe(TakeDamage);
    }

    private void TakeDamage(float amount)
    {
        playerHealth.Value -= amount;
    }
}
```

## Writing the Unit Test

To test the `Player` component, a test script can be created inside an "Editor" folder (or an assembly configured for testing).

The key steps in the test are:
1.  **Arrange:** Create instances of the required SOAR assets (`FloatVariable`, `FloatGameEvent`) and the `Player` component itself.
2.  **Act:** Simulate the event that should be tested (raising the `onPlayerDamaged` event).
3.  **Assert:** Verify that the component behaved as expected (the `playerHealth` `Variable` was reduced).

Here is the corresponding test script:

```csharp
// File: PlayerTests.cs
using NUnit.Framework;
using Soar.Events;
using Soar.Variables;
using UnityEngine;

public class PlayerTests
{
    [Test]
    public void Player_TakesDamage_When_OnPlayerDamagedEventIsRaised()
    {
        // 1. Arrange
        // Create a GameObject to host the component
        var playerGameObject = new GameObject();
        var player = playerGameObject.AddComponent<Player>();

        // Create instances of the required SOAR assets in memory
        var healthVariable = ScriptableObject.CreateInstance<FloatVariable>();
        var damageEvent = ScriptableObject.CreateInstance<FloatGameEvent>();

        // Use reflection or a public setter to assign these assets to the Player component
        // (This avoids needing to serialize them into a prefab for the test)
        SetPrivateField(player, "playerHealth", healthVariable);
        SetPrivateField(player, "onPlayerDamaged", damageEvent);

        // Manually call Start() to trigger subscriptions and set initial health
        player.SendMessage("Start");

        // 2. Act
        // Simulate the damage event being raised
        damageEvent.Raise(25f);

        // 3. Assert
        // Check if the health variable was correctly updated
        Assert.AreEqual(75f, healthVariable.Value);

        // Cleanup the GameObject
        Object.DestroyImmediate(playerGameObject);
    }

    // Helper method to set private fields for testing purposes
    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
```

## Key Concepts in Testing

*   **In-Memory Assets:** `ScriptableObject.CreateInstance<T>()` is used to create temporary, in-memory versions of the SOAR assets needed for the test. These are not saved to the project and are garbage collected after the test runs.
*   **Manual Initialization:** Because the `Player` component is not in a scene, its `Start()` method must be called manually via `SendMessage("Start")` to ensure its logic (like setting initial health and subscribing to events) is executed.
*   **Simulating Events:** The test directly controls the SOAR assets. By calling `damageEvent.Raise(25f)`, the test plays the role of the "publisher," triggering the logic in the `Player` component.
*   **Decoupling in Action:** Notice that the test does not need to know *how* the player takes damage, only that listening to the `onPlayerDamaged` event should result in a change to the `playerHealth` variable. This demonstrates the power of testing decoupled components.

By following this pattern, any component that relies on SOAR for its inputs and outputs can be tested thoroughly and efficiently, leading to a more stable and maintainable codebase.
