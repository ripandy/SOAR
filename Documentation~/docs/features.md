# Features

SOAR is a modular framework for Unity that leverages ScriptableObjects for event-driven architecture.
This section provides an overview of SOAR's key features.


### [Command](features/command.md)

An implementation of [Command pattern](https://gameprogrammingpatterns.com/command.html), utilizing `ScriptableObject` as an alternative to an `Interface`.
The `Command` class is an abstract, requiring concrete implementation.
This pattern is useful for one-way executions, such as logging.


### [GameEvent](features/game-event.md)

An Event represents an occurrence within the program execution that requires a specific response.
Implemented as `GameEvent`, each event must have at least one publisher and one subscriber.


### [Variable](features/variable.md)

A Variable is data stored in a `ScriptableObject` that can be manipulated.
In SOAR, `Variable` is derived from `GameEvent` and triggers a value-changed event upon modification.


### [Collection](../3-soar-core/collection.md)

A Collection is a data structure that can contain multiple items.
The `Collection` class serves as a base for `SoarList` and `SoarDictionary`.
It provides value-changed event for each item, along with additional events triggered when an item is added, removed, updated, or when the collection is cleared.
`Collection` implements common interfaces like `ICollection`, `IList`, and `IDictionary` to ensure compatibility with LINQ.


### [Transaction](../3-soar-core/transaction.md)

A Transaction is a two-way event involving requests and responses.
When a request is sent, a registered response event processes it and returns a result to the requester.
Only one response handler can be registered at a time.
This is useful when an operation needs to wait for an event to complete.


### [Base Classes](../4-fundamentals/base-classes.md)

SOAR provides default base classes that are immediately usable.
You can access them from the `Create > SOAR` context menu or the `Assets > Create > SOAR` menu bar item.
Note that Base Classes use a different assembly definition file `(.asmdef)`.
If you manage your own `.asmdef` references, you may need to add a reference to `Soar.Base` in your project.


### [Unity Event Binder](../3-soar-core/gameevent.md#unity-event-binder)

A Unity Component that forwards events raised by a `GameEvent` into `UnityEvent`.
This is also known as an `EventListener` in Scriptable Object Architecture terminology.


### [Json Extension](../5-utilities/json-extension.md)

An Editor tool to convert SOAR Variable's data into a JSON string or a local JSON file.
You can access this functionality from a SOAR Variable's Inspector window.


[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
[R3]: https://github.com/Cysharp/R3
