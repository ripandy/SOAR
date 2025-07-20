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


### [Collection](features/collection.md)

A Collection is a data structure that can contain multiple items.
The `Collection` class serves as a base for `SoarList` and `SoarDictionary`.
It provides value-changed event for each item, along with additional events triggered when an item is added, removed, updated, or when the collection is cleared.
`Collection` implements common interfaces like `ICollection`, `IList`, and `IDictionary` to ensure compatibility with LINQ.


### [Transaction](features/transaction.md)

A Transaction is a two-way event involving requests and responses.
When a request is sent, a registered response event processes it and returns a result to the requester.
Only one response handler can be registered at a time.
This is useful when an operation needs to wait for an event to complete.


### [Base Classes](features/base-class.md)

SOAR provides default base classes that are immediately usable.
They can be accessed from the `Create > SOAR` context menu or the `Assets > Create > SOAR` menu bar item.
Note that Base Classes use a different assembly definition file `(.asmdef)`.
If `.asmdef` references are managed manually, a reference to `Soar.Base` may need to be added.


### [Unity Event Binder](features/unity-event-binder.md)

A Unity Component that forwards events raised by a `GameEvent` into `UnityEvent`.
This is also known as an `EventListener` in Scriptable Object Architecture terminology.


### [Jsonable Extension](features/jsonable-extension.md)

An Editor tool to convert SOAR Variable's data into a JSON string or a local JSON file.
You can access this functionality from a SOAR Variable's Inspector window.


[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
[R3]: https://github.com/Cysharp/R3
