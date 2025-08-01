
# Performance Best Practices

## A Balanced Perspective

SOAR is designed for clean, decoupled, and maintainable architecture. For the vast majority of game development scenarios—like UI interactions, inventory management, quest updates, and game state changes—the performance overhead of SOAR is negligible and the architectural benefits are immense.

This guide is not meant to discourage the use of SOAR's features. Instead, it aims to provide a transparent look at what is happening under the hood, so the framework can be used intelligently and potential performance-critical situations can be identified. The golden rule of optimization always applies: **profile first, then optimize.**

---

### The Single Most Important Rule: Avoid High-Frequency Events

As a rule of thumb, **events or reactive variables should not be used for logic that runs hundreds or thousands of times per frame.**

The pub/sub pattern has a small, fixed overhead for each `Raise()` call. While this is insignificant for a button click, it can become noticeable if used improperly.

**Bad Use Case: Real-time Character Position**

```csharp
// In a PlayerController.cs, inside Update()
// ANTI-PATTERN: This should be avoided!
void Update()
{
    // This raises an event and notifies all subscribers EVERY frame.
    playerPositionVariable.Value = transform.position; 
}
```

For high-frequency data like this, it is better for other components to pull the data when needed by holding a direct reference to the `PlayerController` or a relevant data provider. SOAR should be used for state *changes*, not continuous state updates.

---

### Feature-by-Feature Performance Breakdown

#### 1. `GameEvent` and `Variable`

*   **What Happens:** When `Raise()` is called, the `GameEvent` iterates through its internal list of subscribers and invokes each one. For a `Variable`, setting the `.Value` property does the same thing.
*   **Performance Cost:**
    *   The cost is proportional to the number of active subscribers. More listeners mean more method calls.
    *   There is a minor overhead compared to a direct C# method call, but it is extremely fast for most use cases.
*   **Best Practices:**
    *   **`ValueChanged` vs. `ValueAssign`:** When a `Variable` is created, the `valueEventType` defaults to `ValueChanged`. This is generally the desired behavior. An equality check (`.Equals()`) is performed before the event is raised, preventing notifications if the value has not actually changed. It should be noted that for complex `struct` types, a custom `.Equals()` implementation might be needed for this to be efficient. Using `ValueAssign` is slightly faster as it skips the check, but it can lead to unnecessary logic being executed by subscribers.
    *   **Unsubscribing:** Subscriptions should always be cleaned up. While SOAR's `CompositeDisposable` and R3's `AddTo(this)` make this easy, forgetting to do so can leave "zombie" listeners that continue to be invoked, needlessly consuming resources.

#### 2. `Collection` (Lists and Dictionaries)

*   **What Happens:** Modifying a `SoarList` or `SoarDictionary` can trigger multiple events. For example, `Add()` triggers `OnAdd`, `OnValueChanged`, and `OnCountChanged`.
*   **Performance Cost:** The cost is higher than modifying a standard C# `List<T>` or `Dictionary<>`.
*   **Best Practices:**
    *   **Batch Operations:** If many items need to be added or removed at once, doing so in a loop of individual `Add()` calls should be avoided. While SOAR does not have a true "batch" mode that raises a single event, grouping the logic makes the code cleaner and easier to profile.
    *   **Choosing Wisely:** If a list of data does not need to be reactive and is only used by a single system, a standard `List<T>` inside a `MonoBehaviour` is more performant. A `SoarList` should be used when multiple, decoupled systems need to be explicitly notified that the collection has changed.

#### 3. `JsonableVariable` and `autoResetValue`

This is the area with the most significant and non-obvious performance cost.

*   **What Happens:** To support the `autoResetValue` feature for complex class types, `Variable<T>` performs a "deep copy" by serializing the initial value to a JSON string and storing it. When `ResetValue()` is called, it deserializes this string back into an object.
*   **Performance Cost:**
    *   **GC Allocation:** `JsonUtility.ToJson()` allocates a string on the managed heap. This creates garbage that the Garbage Collector (GC) must later clean up, which can cause frame rate stutters.
    *   **CPU Overhead:** Serialization and deserialization are computationally more expensive than a simple value assignment.
    *   This cost is paid at **initialization time** (e.g., when the game starts or the editor recompiles) for every `Variable` that uses a class type.
*   **Best Practices:**
    *   **STRONGLY PREFER STRUCTS AND PRIMITIVES:** For any `Variable` that needs `autoResetValue`, primitive types (`int`, `float`, `bool`) or `struct`s should be used whenever possible. These are copied by value, avoiding the JSON serialization path entirely.
    *   **Disable `autoResetValue` for Classes:** If a class type must be used in a `Variable` (e.g., `MyDataClassVariable`), `autoResetValue` should be disabled if the reset behavior is not needed, or the reset should be handled manually.
    *   **Be Aware of Startup Cost:** If hundreds of these variables are used, a small hitch may be noticeable during application startup. This is why.

#### 4. R3 Integration

*   **What Happens:** Using R3's LINQ-style operators (`Where`, `Select`, `CombineLatest`, etc.) creates a chain of small observer objects.
*   **Performance Cost:** R3 is highly optimized, but every operator in a chain adds a small layer of allocation and indirection. A very long, complex chain will have more overhead than a simple `Subscribe()`.
*   **Best Practices:**
    *   **Use with Confidence:** This overhead is tiny and the expressive power of R3 is immense. This is not a reason to avoid using it.
    *   **Profile, Don't Guess:** If an extremely complex reactive stream is suspected of being a bottleneck, the Unity Profiler should be used to inspect it. In 99% of cases, it will not be the source of performance problems.

---

### Summary & Key Takeaways

*   ✅ SOAR should be used for UI, game state, and logic that responds to discrete events.
*   ❌ SOAR should not be used for high-frequency updates that happen every frame.
*   🧠 The `autoResetValue` feature with class types should be used with care; it uses JSON serialization and creates garbage. **Structs are preferred.**
*   ⚖️ Subscribing to a `Collection` is more expensive than modifying a standard C# `List`. It should be used when reactivity is needed.
*   🗑️ Subscriptions should always be cleaned up to prevent memory leaks and unnecessary work.
