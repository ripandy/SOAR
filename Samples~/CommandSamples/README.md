# Command Sample

This sample demonstrates the use of the `Command` feature.

## Scene

The `CommandSample` scene contains several UI Buttons.

## How it Works

This sample uses two custom `Command` assets:

1.  **`PrintLogCommand`:** A parameterless `Command` that prints a pre-configured, colorful message to the console when its `Execute()` method is called.
2.  **`StringLogCommand`:** A `Command<string>` that takes a string as a parameter and prints it to the console.

In the scene, different UI Buttons are wired to these commands:

*   Some buttons call `Execute()` directly on the `PrintLogCommand` asset via a `UnityEvent`.
*   Another button is wired to a `UnityEventBinder` which in turn calls `Execute()` on the `StringLogCommand`, demonstrating how to pass a value from a `UnityEvent` to a command.

This sample showcases how to encapsulate logic into a reusable `Command` asset and trigger it from various sources.
