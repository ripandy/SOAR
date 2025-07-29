# GameEvent Sample

This sample demonstrates the use of the `GameEvent` feature.

## Scene

The `GameEventSample` scene contains a UI with a button and a text label.

## How it Works

1.  **`GameEvent` Asset:** A parameterless `GameEvent` ScriptableObject acts as the communication channel.
2.  **Button as Publisher:** The UI Button is configured to call the `Raise()` method on the `GameEvent` asset whenever it is clicked.
3.  **`EventSubscriber.cs`:** This component subscribes to the `GameEvent`. When the event is raised, it updates a TextMeshPro label with a new word from a predefined array, forming a sentence one word at a time.

This sample showcases a simple, powerful use of the publish-subscribe pattern. The button that raises the event has no direct reference to the text label that reacts to it, demonstrating decoupled communication.
