# Troubleshooting & FAQ

This page contains solutions to common problems and answers to frequently asked questions.

## Common Problems

### My `UnityEventBinder` is not firing.

1.  **Check the `GameEvent` Reference:** Ensure that you have dragged the correct `GameEvent` asset into the `Game Event To Listen` field in the `UnityEventBinder`'s Inspector.
2.  **Check the Publisher:** Is another script actually calling `Raise()` on that *exact same* `GameEvent` asset? You can test this by selecting the `GameEvent` asset in your Project window and clicking the "Raise" button in its Inspector while in Play Mode. If the binder fires, the problem is with your publishing script. If it doesn't, the problem is with the binder's setup.
3.  **Check for `[ExecuteInEditMode]`:** If you are trying to get the binder to work in Edit Mode, ensure the listening component has the `[ExecuteAlways]` or `[ExecuteInEditMode]` attribute.

## Frequently Asked Questions

### What are the performance implications of using SOAR?

For most use cases, the performance overhead of SOAR is negligible. Raising a `GameEvent` is essentially a method call and a loop through a list of subscribers. While this is slightly slower than a direct method call, the architectural benefits of decoupling almost always outweigh the micro-optimization of direct references.

Performance can become a consideration if you are raising an event hundreds or thousands of times per frame. In such high-frequency scenarios, you might consider a more direct approach for that specific system, but these cases are rare.

### Can I use SOAR without the R3 (Reactive Extensions) library?

Yes. SOAR is designed to function independently. The core features (`GameEvent`, `Variable`, `Collection`, etc.) have a base implementation that uses standard C# events (`Action`).

However, the R3 integration provides significant advantages for asynchronous programming and complex event processing (e.g., filtering, merging, and throttling event streams). It is highly recommended for anything beyond simple events.

### How do I pass multiple parameters with a `GameEvent`?

A `GameEvent<T>` can only carry one parameter (`T`).
If you need to pass multiple values, you should encapsulate them in a custom `struct` or `class`.
This concept of encapsulation are also known as [Data Transfer Object (DTO)](https://en.wikipedia.org/wiki/Data_transfer_object).

**Example:**
```csharp
// Define a struct to hold the data
[System.Serializable]
public struct PlayerDamagedData
{
    public float DamageAmount;
    public DamageType Type;
    public GameObject Instigator;
}

// Create a GameEvent for that struct
[CreateAssetMenu(menuName = "SOAR/Game Events/Player Damaged Event")]
public class PlayerDamagedEvent : GameEvent<PlayerDamagedData> { }
```
