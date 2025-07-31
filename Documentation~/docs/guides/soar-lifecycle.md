# SOAR's Lifecycle (SoarCore)

Every asset in SOAR, from `GameEvent` to `Variable` to `Collection`, inherits from a single, crucial base class: `SoarCore`. This class is the heart of the framework, providing a managed lifecycle that ensures all SOAR assets behave predictably and reliably within the Unity editor and in builds. An understanding of this lifecycle is key to mastering SOAR.

The primary purpose of `SoarCore` is to bridge the gap between `ScriptableObject` assets, which live outside of any scene, and the runtime events of an application, like entering and exiting Play Mode.

## The Core Lifecycle Methods

The lifecycle is managed by `SoarCore` through three main methods: `OnEnable`, `Initialize`, and `OnQuit`.

1.  **`OnEnable()`**
    *   **When it's called:** This is a standard Unity message called when the `ScriptableObject` is loaded. This happens on application start, after a script re-compilation, or when the asset is first inspected.
    *   **What it does:** The primary job of `SoarCore` in `OnEnable` is to set up its internal state and subscribe to critical editor and application events. `Initialize()` is called immediately after.

2.  **`Initialize()`**
    *   **When it's called:** Called automatically by `OnEnable`.
    *   **What it does:** This is where the "setup" logic for each SOAR asset happens. For example, a `Variable` will store its current value as its `InitialValue` during this step. A `Transaction` will register its internal response handler. A callback is also registered to the `Application.exitCancellationToken`, ensuring that the `OnQuit` method is called when the application closes.

3.  **`OnQuit()`**
    *   **When it's called:** Called automatically when the application is closing (in a build) or when Play Mode is exited in the editor.
    *   **What it does:** This is the "cleanup" phase. It is responsible for resetting the state of SOAR assets to prevent data from one Play Mode session from "leaking" into the next. For example, a `Variable` with `autoResetValue` enabled will revert to its `InitialValue`. All active subscriptions within a `GameEvent` are disposed of to prevent memory leaks.

## The "Disable Domain Reload" Edge Case

Unity's "Enter Play Mode Options" allow for Domain Reloading to be disabled for faster iteration. SOAR is built to handle this scenario correctly, but its behavior is slightly different.

*   **With Domain Reload (Default):** When Play Mode is exited, all script states are wiped and reset by Unity. Upon re-entering Play Mode, `OnEnable` and `Initialize` are called on fresh objects.
*   **Without Domain Reload:** Script states are *not* wiped. Objects persist between Play Mode sessions. This requires `SoarCore` to manually manage the "setup" and "cleanup" process.

Here is the flow when Domain Reload is disabled:

1.  **Exiting Edit Mode (Entering Play Mode):** `SoarCore` calls `Initialize()` to ensure all assets are in a fresh state for the new session.
2.  **Exiting Play Mode (Re-entering Edit Mode):** `SoarCore` calls `OnQuit()` to clean up, reset variables, and dispose of subscriptions, just as it would if the application were closing.

This careful management ensures that SOAR behaves consistently and reliably, regardless of the project's settings.

## Why This Matters

An understanding of this lifecycle helps in diagnosing issues and building more robust systems.

*   **Initialization Order:** It can be assumed that a `Variable`'s `InitialValue` is set at the very start of the game, before any `MonoBehaviour` `Start()` methods are called.
*   **Clean State:** It is assured that when the game is stopped and started in the editor, the SOAR assets will be in a clean, predictable state, preventing frustrating bugs caused by stale data.
*   **Resource Management:** It can be trusted that all event subscriptions created by SOAR are automatically disposed of, preventing memory leaks.