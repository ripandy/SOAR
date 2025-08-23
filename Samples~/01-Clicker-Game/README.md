# 01: Clicker Game Sample

## Overview

This sample demonstrates the most fundamental pattern in SOAR: using a `GameEvent` to signal an action and a `Variable` to hold and react to a state change.

## Scene Setup

The scene will contain a single UI Button for the user to click and a TextMeshPro UI element to display the current score.

## How it Works

1.  **The Click:** A UI Button is configured to `Raise()` a `GameEvent` asset named `OnClickEvent` via its `OnClick` UnityEvent.
2.  **The Logic:** The `ClickManager.cs` component subscribes to `OnClickEvent`. When the event is raised, it increments the value of an `IntVariable` asset named `ScoreVariable`.
3.  **The Display:** The `ScoreDisplay.cs` component subscribes to `ScoreVariable`. When the variable's value changes, it updates the TextMeshPro UI element to show the new score.

## Key Concepts

*   **Decoupling Actions from State:** The button click (the action) is completely separate from the score management (the state). The `GameEvent` acts as the bridge between them.
*   **Reactive UI:** The UI (ScoreDisplay) automatically reacts to changes in the game's state (`ScoreVariable`) without the game logic needing a direct reference to the UI elements. This keeps the game logic clean and independent of the view.