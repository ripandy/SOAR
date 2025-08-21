# Command Sample

## Overview

This sample demonstrates how to use `Command` objects to encapsulate and trigger reusable logic.

## Scene Setup

The `CommandSample` scene contains several UI Buttons configured to trigger different `Command` assets.

## How it Works

This sample uses two custom `Command` assets:

1.  **`PrintLogCommand`:** A parameterless `Command` that prints a pre-configured, colorful message to the console when its `Execute()` method is called.
2.  **`StringLogCommand`:** A `Command<string>` that takes a string as a parameter and prints it to the console.

In the scene, different UI Buttons are configured to call these commands:

*   Some buttons call `Execute()` directly on the `PrintLogCommand` asset via a `UnityEvent`.
*   Another button is connected to a `UnityEventBinder`. This `UnityEventBinder` is configured to call `Execute()` on the `StringLogCommand` and pass a string value from the `UnityEvent`.

## Key Concepts

*   **Encapsulation:** Logic is contained within a `Command` asset, making it self-contained and reusable.
*   **Decoupling:** The UI Buttons that trigger the commands do not need direct references to the objects that perform the logic.
*   **Parameterization:** `Command<T>` allows you to pass data to your encapsulated logic.
