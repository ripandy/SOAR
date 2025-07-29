# UnityEventBinder Sample

This sample demonstrates the use of the `UnityEventBinder` component.

## Scene

The `UnityEventBinderSample` scene contains a UI with a button and a text label.

## How it Works

1.  **`GameEvent` as a Trigger:** A `GameEvent` asset is raised by a UI Button's `OnClick` event.
2.  **`UnityEventBinder` as a Listener:** A `UnityEventBinder` component is placed on a GameObject. It is configured to listen to the `GameEvent`.
3.  **`StringVariable` for Display:** A `StringVariable` holds the text that will be displayed on screen.
4.  **`EventHandler.cs`:** This script is invoked by the `UnityEventBinder`. When the event is raised, this script updates the `StringVariable` with a new word, forming a sentence.
5.  **UI Text:** A TextMeshPro component in the scene is bound to the `StringVariable` (likely via another `UnityEventBinder` listening to the variable) and updates automatically when the variable changes.

This sample showcases how `UnityEventBinder` acts as a bridge between SOAR's `GameEvent` system and Unity's `UnityEvent` system, allowing for codeless event wiring in the Inspector.
