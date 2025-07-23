# R3 Integration: Core Concepts

SOAR was originally designed to be a wrapper of R3 within Scriptable Object Architecture. Beyond SOAR it is basically R3 functionalities. Refer to [R3's documentation](https://github.com/Cysharp/R3?tab=readme-ov-file#r3) for further details.

## Observable Streams

With R3, most events in SOAR can be treated as `Observable` streams. This means that powerful R3 operators like `Where`, `Select`, `Merge`, `CombineLatest`, and `Throttle` can be used to create sophisticated event-driven logic in a declarative way.

## Async/Await Support

The integration provides `...Async()` methods for many operations, which return a `ValueTask` or `ValueTask<T>`. This allows for writing clean and efficient asynchronous code without manual callback management.

## Disposables and Lifetime Management

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
