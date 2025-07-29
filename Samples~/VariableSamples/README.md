# Variable Sample

This sample demonstrates the use of the `Variable<T>` feature.

## Scene

The `VariableSample` scene contains a simple health bar UI, a "Healer" object that restores health on hover, and an "Upgrade" button.

## How it Works

1.  **`FloatVariable` as Health:** A `FloatVariable` ScriptableObject is used to represent the player's current health. Another one represents the maximum health.
2.  **`HealthBar.cs` and `HealthText.cs`:** These components subscribe to the health `FloatVariable`. When the variable's value changes, they update the UI elements (a RectTransform's scale and a TextMeshPro label) to reflect the new health value.
3.  **`Healer.cs`:** This script increases the health `FloatVariable` over time when the mouse pointer is hovering over its GameObject.
4.  **`HealthUpgrade.cs`:** This script increases the maximum health `FloatVariable` when the button is clicked, and scales the current health accordingly.

This sample showcases how multiple, decoupled components can react to changes in a shared data asset (`FloatVariable`) without needing direct references to each other.
