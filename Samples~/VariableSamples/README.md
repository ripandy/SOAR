# Variable Sample

## Overview

This sample demonstrates how to use a `Variable<T>` to share and observe data between different components.

## Scene Setup

The `VariableSample` scene contains a simple health bar UI, a "Healer" object that restores health on hover, and an "Upgrade" button.

## How it Works

1.  **`FloatVariable` as Health:** A `FloatVariable` ScriptableObject represents the player's current health. Another `FloatVariable` represents the maximum health.
2.  **`HealthBar.cs` and `HealthText.cs`:** These components subscribe to the health `FloatVariable`. When the variable's value changes, they update the UI elements (a RectTransform's scale and a TextMeshPro label) to reflect the new health value.
3.  **`Healer.cs`:** This script increases the health `FloatVariable` over time when the mouse pointer hovers over its GameObject.
4.  **`HealthUpgrade.cs`:** This script increases the maximum health `FloatVariable` when the button is clicked and scales the current health accordingly.

## Key Concepts

*   **Shared Data:** A `Variable<T>` asset acts as a shared data container that can be accessed by any number of objects.
*   **Data Binding:** UI elements are "bound" to the `Variable<T>`. They automatically update when the data changes, without needing to be explicitly told to do so.
*   **Decoupling:** The `Healer`, `HealthUpgrade`, `HealthBar`, and `HealthText` components all interact with the same data without needing direct references to each other.
