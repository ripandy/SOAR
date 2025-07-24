# R3 Integration: GameEvent and Variable

The integration of R3 brings powerful reactive and asynchronous capabilities to SOAR's `GameEvent` and `Variable` assets.

## `GameEvent` and `GameEvent<T>`

When R3 is available, `GameEvent`s are enhanced with the following methods:

-   **`AsObservable()`**: Returns the event as an R3 `Observable` stream.
    -   For a parameterless `GameEvent`, this returns an `Observable<Unit>`, which signals a notification without a value.
    -   For a `GameEvent<T>`, it returns an `Observable<T>`, which carries the event's data payload.
-   **`AsUnitObservable()`**: Specific to `GameEvent<T>`, this returns an `Observable<Unit>`. It's useful when you only need to know *that* an event was raised, but don't care about its value.
-   **`EventAsync()`**: Returns a `ValueTask` (or `ValueTask<T>`) that completes the next time the event is raised. This is perfect for `async/await` workflows.
-   **`ToAsyncEnumerable()`**: Converts the event stream into an `IAsyncEnumerable<T>`, allowing it to be consumed in an `await foreach` loop, which is useful for processing a sequence of events.

### Examples

**Using `AsObservable` with `GameEvent<T>`:**
```csharp
using R3;
using Soar.Events;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    [SerializeField] private StringGameEvent onImportantEvent;

    void Start()
    {
        onImportantEvent.AsObservable()
            .Subscribe(message => Debug.Log($"Event received: {message}"))
            .AddTo(this);
    }
}
```

**Awaiting a GameEvent:**
```csharp
using Soar.Events;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerReady;

    private async void Start()
    {
        Debug.Log("Waiting for player to be ready...");
        await onPlayerReady.EventAsync();
        Debug.Log("Player is ready! Starting level.");
    }
}
```

**Using `await foreach` to process multiple events:**
```csharp
using Soar.Events;
using UnityEngine;

public class PowerUpCollector : MonoBehaviour
{
    [SerializeField] private StringGameEvent onPowerUpCollected;

    private async void Start()
    {
        Debug.Log("Listening for power-ups...");
        await foreach (var powerUpName in onPowerUpCollected.ToAsyncEnumerable(destroyCancellationToken))
        {
            Debug.Log($"Collected power-up: {powerUpName}!");
        }
    }
}
```

## `Variable<T>`

`Variable<T>` inherits all the R3 features from `GameEvent<T>` and adds a crucial one for observing value changes:

-   **`Pairwise()` Operator**: While not a direct method on the `Variable`, the `AsObservable()` stream can be combined with R3's `Pairwise()` operator. This creates a new stream that emits a `PairwiseValue<T>`, which contains both the previous and the current value of the variable, making it easy to see how the data changed.

### Examples

**Observing old and new values:**
```csharp
using R3;
using Soar.Variables;
using UnityEngine;

public class HealthWatcher : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;

    void Start()
    {
        playerHealth.AsObservable()
            .Pairwise() // Get a stream of (oldValue, newValue)
            .Subscribe(pair =>
            {
                if (pair.NewValue < pair.OldValue)
                {
                    Debug.Log($"Player took {pair.OldValue - pair.NewValue} damage!");
                }
            })
            .AddTo(this);
    }
}
```

**Filtering a `Variable` stream:**
```csharp
using R3;
using Soar.Variables;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private IntVariable scoreVariable;

    void Start()
    {
        // Only show a message for scores higher than 100
        scoreVariable.AsObservable()
            .Where(score => score > 100)
            .Subscribe(highScore => Debug.Log($"High score alert: {highScore}"))
            .AddTo(this);
    }
}
```
