# UnityEventBinder Sample

## Overview

This sample demonstrates how to use the `UnityEventBinder` component to bridge between SOAR's `GameEvent` system and Unity's `UnityEvent` system.

## Scene Setup

The `UnityEventBinderSample` scene contains a UI with a button and a text label.

## How it Works

1.  **`GameEvent` as a Trigger:** A `GameEvent` asset is raised by a UI Button's `OnClick` event.
2.  **`UnityEventBinder` as a Listener:** A `UnityEventBinder` component is placed on a GameObject. It is configured to listen to the `GameEvent`.
3.  **`EventHandler.cs`:** This script is invoked by the `UnityEventBinder`'s `UnityEvent`. When the event is raised, this script updates a `StringVariable` with a new word, forming a sentence.
4.  **UI Text:** A TextMeshPro component in the scene is bound to the `StringVariable` and updates automatically when the variable changes.

## Key Concepts

*   **Bridging Systems:** The `UnityEventBinder` acts as a bridge, allowing you to trigger `UnityEvent`s in response to `GameEvent`s.
*   **Codeless Wiring:** This allows you to connect SOAR events to other components in the Inspector without writing any code.
*   **Inspector-based Configuration:** You can configure the entire event handling flow in the Unity Inspector, making it easy to see how different parts of your system are connected.
