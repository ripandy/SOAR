# R3 Integration

SOAR is designed to be highly extensible with [R3 (Reactive Extensions for C#)](https://github.com/Cysharp/R3), a modern and feature-rich reactive programming library. When R3 is present in the project, SOAR automatically integrates with it, unlocking a powerful set of features for asynchronous programming, event handling, and data stream manipulation.

This integration enhances SOAR's core features by:

-   **Exposing `Observable` Streams**: SOAR objects like `GameEvent`s, `Variable`s, `Collection`s, and `Transaction`s expose their events as R3 `Observable` streams, allowing for complex event processing with LINQ-style operators.
-   **Providing `async/await` Support**: Awaiting events and transactions becomes seamless with `async/await`.
-   **Advanced Concurrency Control**: For `Transaction`s, R3 provides fine-grained control over how concurrent requests are handled.

## Enabling the Integration

To enable the R3 integration, simply install R3 into the Unity project. SOAR will automatically detect its presence via the `SOAR_R3` scripting define symbol, which is typically added by the R3 package installation.

If the `SOAR_R3` symbol is not automatically defined, it can be added manually in `Project Settings > Player > Other Settings > Scripting Define Symbols`.

## Core Concepts

### Observable Streams

With R3, most events in SOAR can be treated as `Observable` streams. This means that powerful R3 operators like `Where`, `Select`, `Merge`, `CombineLatest`, and `Throttle` can be used to create sophisticated event-driven logic in a declarative way.

### Async/Await Support

The integration provides `...Async()` methods for many operations, which return a `ValueTask` or `ValueTask<T>`. This allows for writing clean and efficient asynchronous code without manual callback management.

### Disposables and Lifetime Management

All subscriptions to SOAR events (whether through `Subscribe()` or R3's `Observable`s) return an `IDisposable`. It is crucial to manage the lifetime of these subscriptions to prevent memory leaks. R3 provides convenient extension methods like `AddTo(Component)` to automatically dispose of subscriptions when a `GameObject` is destroyed.

```csharp
using R3;
using Soar.Events;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    [SerializeField] private GameEvent myGameEvent;

    void Start()
    {
        myGameEvent.AsObservable()
            .Subscribe(_ => Debug.Log("Event raised!"))
            .AddTo(this); // Automatically disposes when this GameObject is destroyed
    }
}
```
