# Scriptable Object Architecture Reactive-extensible

<!-- [![openupm](https://img.shields.io/npm/v/com.ripandy.soar?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ripandy.soar/) -->

[日本語](./README_ja.md)

SOAR is an implementation of Scriptable Object Architecture.
Scriptable Object Architecture intends to provide clean and decoupled code architecture.
SOAR is implemented based on [Ryan Hipple's talk at Unite Austin 2017](https://youtu.be/raQ3iHhE_Kk).

SOAR is an event-based system that encourages the use of the [Pub/Sub pattern](#publishersubscriber-pattern).
Its fundamental principle involves treating the [ScriptableObject] instance (with its `name`/`guid` property) as a 'Channel' or 'Topic'.
The pairing between a publisher and a subscriber is established through references to each of SOAR's instance.

SOAR is developed and designed to be extensible with Reactive Extensions library [R3], a feature-rich, modern Reactive Extensions for C#.
SOAR wraps and utilizes R3's feature within the Scriptable Object Architecture.
SOAR can function independently, but its implementation provides only basic functionality.
It is highly recommended to use SOAR in conjunction with R3.

- [R3] - The new future of dotnet/reactive and [UniRx].
- [Kassets] - SOAR's predecessor. Scriptable Object Architecture extensible with [UniRx] and [UniTask].

__For further details, see [Documentation]__

### Unity Version
- Unity 6.0 or later

> [!NOTE]
> This repository is a Unity Package, not a standalone Unity Project. It cannot be opened directly in the Unity Editor. Please follow the [Installation](#installation) instructions to add it to your project.

# Getting Started

## Installation

<details>

<summary>Add from OpenUPM | <em>Import via scoped registry.</em></summary>

To add OpenUPM to Package Manager:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s):
  - com.ripandy
  - com.cysharp.r3 (Recommended)
```
- click <kbd>Save</kbd>
- Open `Window/Package Manager`
- Select ``My Registries`` in top left dropdown
- Select ``SOAR`` and click ``Install``
- Select ``R3`` and click ``Install`` (Recommended) (see: Note)

</details>

<details>

<summary>Add from GitHub | <em>Use GitHub link to import.</em></summary>

Add Package directly from GitHub.

- Open `Window/Package Manager`
- Click the `+` icon
- Select the `Install package from Git URL` option
- Paste the following URL: `https://github.com/ripandy/SOAR.git`
- Click `Add`

To install specific version, refer to SOAR's release tags.
For example: `https://github.com/ripandy/SOAR.git#1.0.0`

</details>

<details>

<summary>Clone to Local Folder | <em>Manages changes independently.</em></summary>

- Clone this repository to local directory.
- Open `Window/Package Manager`
- Click the `+` icon
- Select the `Install package from disk` option
- Select the `package.json` file from the cloned directory.
- Click `Add`

The package will be added to manifest.json as a local package (`file://`).
Source codes can then be modified from containing Unity Project.
Changes can be managed with git as usual.
Package path can be changed to relative path as alternative of the default absolute path.

```json
{
  "dependencies": {
    "com.ripandy.soar": "file:../path/to/your/local/SOAR"
  }
}
```

</details>

<details>

<summary>Clone to Packages Folder | <em>For development or contribution purpose.</em></summary>

Clone this repository to Unity Project's Packages directory: `YourUnityProject/Packages/`.

Unity will treat the project as a custom package.
Source codes can then be modified from containing Unity Project.
Changes can be managed with git as usual.
SOAR can also be cloned as Submodule of a git repository.

</details>

> [!NOTE]
> Installation of [R3] in Unity requires installation of its NuGet version. See [R3 Unity Installation](https://github.com/Cysharp/R3?tab=readme-ov-file#unity) for further detail.

## Quick Start

### Create a GameEvent Instance

SOAR's instances can be created from `Create` context menu or from `Assets/Create` on the menu bar.
Right-click on the Project window and select instance to create.
For GameEvent instances, select any of event types from `Create/SOAR/Game Events/` menu.

![SOAR_Create-GameEvent](https://github.com/user-attachments/assets/7fef75b6-995b-4195-a4c8-4d6548b017c6)

### Raise GameEvent from UnityEvents

To raise an event from Unity UI's Button, assign created GameEvent instance to the Button's OnClick event.
Every time the button is clicked, the event will be published and all subscribers will be notified.

![Screen Recording 2025-05-17 at 10 44 03 mov](https://github.com/user-attachments/assets/7354506f-13d6-45f9-90f2-9f920bac9964)

### Usage of UnityEventBinder

Unity Event Binder is a custom implemented Unity Component that forwards events raised by a `GameEvent` into `UnityEvent`.
This is also known as an `EventListener` in Scriptable Object Architecture terminology.

To use them, add the component to any GameObject and assign the GameEvent instance to the `Game Event To Listen` field.

![SOAR_UnityEventBinder_AssignGameEvent](https://github.com/user-attachments/assets/5b0604ed-28a9-41e6-9045-92f2d38314a4)

Upon raising the event, actions assigned to `On Game Event Raised` will be invoked by UnityEvent.

<img width="300" src="https://github.com/user-attachments/assets/d13742c0-a75d-4094-a1a6-f2596bea58ba" alt="SOAR_UnityEventBinder_AssignUnityEvent"/>

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

To subscribe to an event from script, use the `GameEvent` instance's `Subscribe()` method.
Upon subscribing, the provided callback will be invoked when the event is raised.

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

### Raise GameEvent from Inspector

To raise an event from the inspector window, select a `GameEvent` instance, then press the `Raise` button in the inspector.
This will invoke the event and notify all subscribers.
Due to the decoupled nature of SOAR, subscribers do not need to be aware of the source of the event.
This allows for easy testing and debugging of events without the need for a specific publisher.

![SOAR_EditModeRaise mov](https://github.com/user-attachments/assets/0a0cca97-8452-4fe5-8285-5f69548e275a)

> [!NOTE]
> By default, event listeners are not invoked in Edit Mode. Subscribers need to handle running in Edit Mode manually.

### Setting Up Subscriber to Run in Edit Mode

To set up a subscriber to run in Edit Mode, use the `ExecuteAlways` or `ExecuteInEditMode` attribute on the subscriber class.
This will allow the subscriber to be invoked in Edit Mode.

```csharp
// File: GameEventSubscriberExample.cs
using System;
using Soar.Events;
using UnityEngine;

[ExecuteAlways]
public class GameEventSubscriberExample : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;

    private IDisposable subscription;

    private void OnEnable()
    {
        // Subscribe in both Edit and Play modes
        SubscribeToEvent();
    }

    private void OnDisable()
    {
        // Unsubscribe in both Edit and Play modes
        UnsubscribeFromEvent();
    }

    private void SubscribeToEvent()
    {
        if (gameEvent == null) return;
        
        // Dispose previous subscription if it exists
        subscription?.Dispose();
        
        // Create new subscription
        subscription = gameEvent.Subscribe(OnGameEventRaised);
    }

    private void UnsubscribeFromEvent()
    {
        subscription?.Dispose();
        subscription = null;
    }

    private void OnGameEventRaised()
    {
        Debug.Log($"Game Event {gameEvent.name} Received on {(Application.isPlaying ? "Play" : "Edit")} mode.");
    }
}
```

# Features

### Command

`Command` is an implementation of [Command pattern](https://gameprogrammingpatterns.com/command.html), utilizing `ScriptableObject` as an alternative to an `Interface`.
The `Command` class is an abstract, requiring concrete implementation.
This pattern is useful for one-way executions, such as logging.


### GameEvent

Event represents an occurrence within the program execution that requires a specific response.
Each event must have at least one publisher and one subscriber.


### Variable

Variable is data stored in a `ScriptableObject` that can be manipulated.
In SOAR, `Variable` is derived from `GameEvent` and triggers a value-changed event upon modification.


### Collection

Collection is a data structure that can hold multiple items.
The `Collection` class serves as a base for `SoarList` and `SoarDictionary`.
`Collection` offers additional events, triggered when an item is added, removed, or updated, or when the collection is cleared.
`Collection` implements common interfaces like `ICollection`, `IList`, and `IDictionary` to ensure compatibility with LINQ.


### Transaction

Transaction is a two-way event involving requests and responses.
When a request is sent, a registered response event processes it and returns the result to the requester.
Only one response event can be registered at a time.
This is useful when an operation needs to wait for an event to complete.


### Base Classes

SOAR provides default base classes that are usable immediately.
They can be accessed from the `Create > SOAR` context menu or the `Assets > Create > SOAR` menu bar item.
Note that Base Classes use a different assembly definition file `(.asmdef)`.
A manual reference to Soar.Base may need to be added in the project's Assembly Definition (.asmdef) files.


### Unity Event Binder

Unity Event Binder is a custom implemented Unity Component that forwards events raised by a `GameEvent` into `UnityEvent`.
This is also known as an `EventListener` in Scriptable Object Architecture terminology.


### Json Extension

SOAR implements an Editor tool to convert SOAR Variable's data into a JSON string or a local JSON file.
This functionality can be accessed from SOAR Variable's Inspector window.
Available only for classes that derives from `JsonableVariable<T>`.


# Integration with Reactive Extensions Library R3

To use Reactive Extensions Library R3 with SOAR, simply import R3 to the project.
Upon import, SOAR will adjust its internal integration using `SOAR_R3` scripting define, which should be automatically defined when R3 is imported via package manager.
If the scripting define `SOAR_R3` somehow is undefined, add it to `Scripting Define Symbols` on Project Settings.

By importing R3, SOAR has additional features:

- __Internal Event Handling with R3__

  SOAR is developed and designed to be extensible with Reactive Extensions library R3.
  It simply wraps `Observable`, and utilizes them as internal event handlers.

- __Async Event handling with ValueTask__

  With R3 imported, SOAR's event handlers can be used with `ValueTask` and `IAsyncObservable`.

  ```csharp
  // File: GameEventAwaitExample.cs
  using System.Threading.Tasks;
  using Soar.Events;
  using UnityEngine;
  
  public class GameEventAwaitExample : MonoBehaviour
  {
      [SerializeField] private GameEvent gameEvent;
  
      private async void Start()
      {
          // Await the game event in Start
          await AwaitGameEvent();
      }
  
      private async ValueTask AwaitGameEvent()
      {
          Debug.Log("Waiting for game event...");
          await gameEvent.EventAsync();
          Debug.Log("Game event received!");
      }
  }
  ```

- Conversion Methods

  Importing R3 enables SOAR's instance to be convertible to `Observable` for [R3], `IObservable` for (Uni)Rx, and `IAsyncObservable` for (Uni)Task.
  This allows SOAR's instance to utilize the functionality of each respective library.
  After conversion, refer to the documentation of each library for more details.

  ```csharp
  // Implemented on SOAR's GameEvent<T>
  AsObservable()        // Converts SOAR's instance to R3's Observable<T>
  AsUnitObservable()    // Converts SOAR's instance to R3's Observable<Unit>
  AsSystemObservable()  // Converts SOAR's instance to System's IObservable<T>. Can be extensible further with UniRx.
  ToAsyncEnumerable()   // Converts SOAR's instance to System's IAsyncEnumerable<T>
  ```

# Publisher/Subscriber Pattern

The Publisher/Subscriber (Pub/Sub) pattern is a messaging pattern where:

- Publishers emit messages/events without direct knowledge of which components will receive them
- Subscribers register interest in certain events without knowing which components will produce them
- A message broker or event channel mediates between them, decoupling the components

In SOAR's implementation:

- SOAR's instances serve as the message channels or topics
- Publishers call the Raise() method on a GameEvent to broadcast messages
- Subscribers use Subscribe() to register callbacks that execute when events are raised

Though there's no explicit message broker implementation, the nature of ScriptableObject instances that persists across scenes, enables communication between components that might never directly reference each other.
This pattern provides excellent decoupling in Unity, allowing components to communicate without direct dependencies.

# Utilizing SOAR with MVP Pattern

SOAR fits well with the Model-View-Presenter (MVP) pattern.
SOAR's `GameEvent` or `Variable` instance can be utilized to wrap the Model.
As a wrapper, SOAR does not interact with the Model directly.
Model could be on a different namespace or assembly.
Then, use classes that inherits from MonoBehaviour as the Presenter or Controller.
These classes would subscribe to the `GameEvent` or `Variable` instance, then process the Model received from the event.
Finally, use Unity's components or UI elements as the View, which would be updated by presenter using the processed Model data.

![image](https://github.com/user-attachments/assets/99b63d4d-562d-43b8-9516-38e136772eda)

# References:
- [Unite2017 Scriptable Object Architecture sample project](https://github.com/roboryantron/Unite2017)
- [Unite2017 Game Architecture with Scriptable Objects on Slideshare](https://www.slideshare.net/RyanHipple/game-architecture-with-scriptable-objects)
- [R3 — A New Modern Reimplementation of Reactive Extensions for C#](https://neuecc.medium.com/r3-a-new-modern-reimplementation-of-reactive-extensions-for-c-cf29abcc5826)
- [Kassets (SOAR's predecessor). Scriptable Object Architecture extensible with UniRx and UniTask](https://github.com/kadinche/Kassets).

# LICENSE

- SOAR is Licensed under [MIT License](LICENSE)
- [R3] is Licensed under [MIT License](https://github.com/Cysharp/R3/blob/main/LICENSE)
- [Kassets] (SOAR's predecessor) is Licensed under [MIT License](https://github.com/kadinche/Kassets/blob/main/LICENSE)

[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[UniRx]: https://github.com/kadinche/UniRx
[UniTask]: https://github.com/Cysharp/UniTask
[Documentation]: https://ripandy.github.io/SOAR/
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html