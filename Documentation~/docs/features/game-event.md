# GameEvent

`GameEvent` is a core feature in SOAR that implements a simple publish-subscribe pattern using `ScriptableObject` assets.
This allows for decoupled communication between different parts of an application.
An event can be published (raised) by one system and listened to by multiple other systems without them needing direct references to each other.

## `GameEvent` (Parameterless)

The base `GameEvent` represents an event that does not carry any data.

### Creating a GameEvent

Create a `GameEvent` asset from `Create` context menu or from `Assets > Create` on the menu bar.
Right-click on the Project window and select `Create > SOAR > Game Events > GameEvent`.

![SOAR_Create-GameEvent](../../assets/images/SOAR_Create-GameEvent.gif)

### Raise GameEvent from Script

To raise an event from script, use the `GameEvent` instance's `Raise()` method.
Upon raising the event, all subscribers will be notified.

```csharp
// File: GameEventPublisherExample.cs
using Soar.Events;
using UnityEngine;

public class GameEventPublisherExample : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameEvent.Raise();
            Debug.Log($"Game Event {gameEvent.name} Raised.");
        }
    }
}
```

### Subscribe to GameEvent from Script

To listen for an event, call the `Subscribe()` method, providing an `Action` to be executed when the event is raised. The `Subscribe` method returns an `IDisposable` which should be disposed of when the listener no longer needs to receive events (e.g., in `OnDestroy`).

```csharp
// File: GameEventSubscriberExample.cs
using System;
using Soar.Events;
using UnityEngine;

public class GameEventSubscriberExample : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    
    private IDisposable subscription;
    
    private void Start()
    {
        subscription = gameEvent.Subscribe(OnGameEventRaised);
    }

    private void OnGameEventRaised()
    {
        Debug.Log($"Game Event {gameEvent.name} Received.");
    }
    
    private void OnDestroy()
    {
        subscription?.Dispose();
    }
}
```

## `GameEvent<T>` (With Data)

The generic `GameEvent<T>` class allows creating events that carry data of a specific type `T`.

### Creating a Typed GameEvent

SOAR provides some common typed `GameEvent`s (e.g., `IntGameEvent`, `StringGameEvent`) that can be created from the menu.
Custom typed `GameEvent`s can be created by inheriting from `GameEvent<T>`:

```csharp
// File: MyCustomDataGameEvent.cs
using System;
using Soar;
using Soar.Events;
using UnityEngine;

// Define custom data structure
[Serializable]
public struct MyCustomData
{
    public int id;
    public string message;
}

// Create a new GameEvent asset type for MyCustomData
[CreateAssetMenu(fileName = "MyCustomDataGameEvent", menuName = MenuHelper.DefaultGameEventMenu + "My Custom Data GameEvent")]
public class MyCustomDataGameEvent : GameEvent<MyCustomData> { }
```

Then, an asset of this type can be created from `Create > SOAR > Game Events > My Custom Data GameEvent`.

### Raising a Typed Event

Call `Raise(T value)` with the data payload.

```csharp
// File: MyCustomDataGameEvent.cs
using UnityEngine;

public class MyTypedPublisher : MonoBehaviour
{
    [SerializeField] private MyCustomDataGameEvent onDataPublished;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var data = new MyCustomData { id = 1, message = "Hello World" };
            onDataPublished.Raise(data);
        }
    }
}
```

### Subscribing to a Typed Event

Subscribe with an `Action<T>` to receive the data.

```csharp
// File: MyTypedSubscriber.cs
using System;
using UnityEngine;

public class MyTypedSubscriber : MonoBehaviour
{
    [SerializeField] private MyCustomDataGameEvent gameEvent;
    
    private IDisposable subscription;
    
    private void Start()
    {
        subscription = gameEvent.Subscribe(HandlePublishedData);
    }

    private void HandlePublishedData(MyCustomData data)
    {
        Debug.Log($"Data received: ID={data.id}, Message='{data.message}'");
    }
    
    private void OnDestroy()
    {
        subscription?.Dispose();
    }
}
```

The `GameEvent<T>` also has a `value` field that stores the last raised value. This value is reset to `default(T)` when the application quits or when domain reload is disabled and play mode is exited.

## UnityEventBinder

`UnityEventBinder` is a `MonoBehaviour` component that allows for listening to `GameEvents` and invoking `UnityEvents` in response, directly from the Unity Inspector.
This is useful for designers or for quickly wiring up responses without writing code.

### `UnityEventBinder` (for parameterless GameEvents)

1. Add the `UnityEventBinder` component to a GameObject.
2. Assign a `GameEvent` asset to the `Game Event To Listen` field.

   ![SOAR_UnityEventBinder_AssignGameEvent](../../assets/images/SOAR_UnityEventBinder_AssignGameEvent.gif)

3. Configure the `On Game Event Raised` `UnityEvent` in the Inspector to call methods on other components.

   ![SOAR_UnityEventBinder_AssignUnityEvent](../../assets/images/SOAR_UnityEventBinder_AssignUnityEvent.gif)

### `UnityEventBinder<T>` (for GameEvents with data)

Typed `GameEvent<T>` requires the creation of a specific binder, or the use of a generic one if available that can handle the type `T`.
The provided `UnityEventBinder<T>` abstract class can be inherited from to create concrete typed binders.

When using a typed `UnityEventBinder<T>`:

1. Add custom typed `UnityEventBinder` component to a GameObject.
2. Assign a `GameEvent<T>` asset (e.g., `IntGameEvent`) to the `Game Event To Listen` field.
3. Configure the `On Typed Game Event Raised (T)` `UnityEvent<T>` in the Inspector. This `UnityEvent` will pass the event's data to the listening methods.

```csharp
// Example of a concrete typed UnityEventBinder
// File: MyTypedEventBinder.cs
using Soar.Events;

// This class is not strictly necessary if GameEvent<T> and UnityEvent<T>
// are serializable and work with the base UnityEventBinder<T> directly.
// However, creating a concrete class can make it easier to find in the "Add Component" menu.
public class MyTypedEventBinder : UnityEventBinder<MyCustomData> {}
```

## Editor Integration

`GameEvent` assets have a custom editor inspector.

- **Raise Button**: A "Raise" button is available in the Inspector for all `GameEvent`s. Clicking this button will call the `Raise()` method on the event, which is useful for debugging and testing event responses in both Edit Mode and Play Mode.

- **Value Display (for `GameEvent<T>`)**: For typed `GameEvent<T>`, the current `value` (or the properties of the value if it's a complex type) is displayed in the Inspector, which allows for viewing and sometimes modifying the data that will be raised if the "Raise" button is pressed without providing a new value.

## Lifecycle and Disposal

`GameEvent`s are `ScriptableObject`s and implement `IDisposable`.

- Subscriptions are managed internally. When a `GameEvent` is disposed (which happens as part of the `SoarCore` lifecycle, typically on application quit), its subscriptions are cleared.
- For the independent implementation (when `SOAR_R3` is not defined), subscriptions are stored in a list and disposed of.
- When `SOAR_R3` is defined, `GameEvent`s use R3 `Subject`s internally, and disposal is handled by disposing of the `Subject`.
- The `SoarCore` base class handles initialization and cleanup, including calling `Dispose()` during `OnQuit`. It also manages behavior related to Unity's "Enter Play Mode Options" (like Disable Domain Reload).

This ensures that resources are cleaned up correctly and event listeners do not attempt to operate on disposed objects.
