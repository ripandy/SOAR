# GameEvent Sample

## Overview

This sample demonstrates the basic use of the `GameEvent` feature to create decoupled communication between objects.

## Scene Setup

The `GameEventSample` scene contains a UI with a button and a text label.

## How it Works

1.  **`GameEvent` Asset:** A parameterless `GameEvent` ScriptableObject acts as the communication channel.
2.  **Button as Publisher:** The UI Button is configured to call the `Raise()` method on the `GameEvent` asset whenever it is clicked.
3.  **`EventSubscriber.cs` as Subscriber:** This component subscribes to the `GameEvent`. When the event is raised, it updates a TextMeshPro label with a new word from a predefined array, forming a sentence one word at a time.

## Key Concepts


*   **Publish-Subscribe Pattern:** The `GameEvent` acts as a central hub where objects can publish (raise) events and other objects can subscribe (listen) to them.
*   **Decoupling:** The button (publisher) has no direct reference to the text label (subscriber). They only share a reference to the `GameEvent` asset, which allows them to communicate without being tightly coupled.
