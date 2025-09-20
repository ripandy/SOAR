# Architectural Principles

SOAR (Scriptable Object Architecture Reactive-extensible) is not just a collection of tools, but a philosophy for building clean, decoupled, and scalable applications in Unity. Understanding the core architectural principles behind SOAR will help you write more maintainable and robust code.

## The Publisher/Subscriber Pattern

The most fundamental principle in SOAR is the **Publisher/Subscriber (Pub/Sub) pattern**. This is a messaging pattern where:

-   **Publishers** (or Broadcasters) emit messages or events without direct knowledge of which components will receive them.
-   **Subscribers** (or Listeners) register interest in certain events without knowing which components will produce them.
-   A **message broker** or **event channel** mediates between them, decoupling the components.

In SOAR's implementation:

-   `GameEvent` and `Variable` assets serve as the **event channels** or topics.
-   Any script can act as a **publisher** by calling the `Raise()` method on a `GameEvent` or changing the `Value` of a `Variable`.
-   Any script can act as a **subscriber** by calling `Subscribe()` on an event or using a `UnityEventBinder` to listen for it.

This pattern is the key to decoupling. A UI button that raises an event doesn't need a reference to the audio system, the analytics system, or the game logic that needs to react to its click. They all just listen to the same shared event asset.

## Dependency Inversion

By using `ScriptableObject` assets as the communication channel, SOAR achieves **Dependency Inversion**. Instead of high-level components having direct references to low-level components (e.g., a `GameManager` knowing about a specific `PlayerHealth` script), both components reference the same, shared `ScriptableObject` asset.

**Traditional (Coupled) Approach:**
```
GameManager ---> PlayerHealth
```

**SOAR (Decoupled) Approach:**
```
GameManager --+
              |
              v
          PlayerHealthVariable
              ^
              |
PlayerHealth --+
```

This inversion means components can be developed, tested, and modified in isolation. You can test your `PlayerHealth` logic by simply changing the `PlayerHealthVariable` in the Inspector, without needing a `GameManager` at all.

## Scene-to-Scene Communication

Because `ScriptableObject`s are assets that live outside of any specific scene, they are the perfect tool for communication between scenes. A `GameEvent` raised in your "MainMenu" scene can be easily listened to by a component in your "Level01" scene, as long as both have a reference to the same event asset. This solves a common Unity problem without resorting to complex `DontDestroyOnLoad` managers or static singletons.

## Model-View-Presenter (MVP) with SOAR

SOAR fits naturally with the **Model-View-Presenter (MVP)** pattern, a common way to structure UI-heavy applications.

-   **Model:** The raw data and business logic of your application. In a pure implementation, the Model should have no knowledge of Unity.
-   **View:** The Unity components that display the data (e.g., `Text`, `Slider`, `Image`). The View is "dumb" and only knows how to display what it's told.
-   **Presenter:** The mediator between the Model and the View. It listens for changes in the Model, formats the data, and tells the View how to display it. It also listens for input from the View (like button clicks) and updates the Model.

SOAR assets act as the perfect layer between the Model and the Presenter:

-   A `Variable` can wrap a piece of Model data.
-   The Presenter subscribes to this `Variable`.
-   When the Model changes, it updates the `Variable`.
-   The Presenter is notified and updates the View components.

![SOAR MVP Diagram](https://github.com/user-attachments/assets/99b63d4d-562d-43b8-9516-38e136772eda)
