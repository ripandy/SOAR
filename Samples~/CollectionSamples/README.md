# Collection Sample

This sample demonstrates the use of the `Collection<T>` feature, specifically `SoarList<T>`.

## Scene

The `CollectionSample` scene contains a grid of UI panels, each with a text label. There are buttons to add a new panel, remove a panel, and update the value of a random panel.

## How it Works

1.  **`IntCollection` (`SoarList<int>`):** An `IntCollection` asset stores a list of integers. Each integer corresponds to the value displayed on one of the UI panels.
2.  **`CollectionEventHandler.cs`:** This component listens to UI button events (`GameEvent`s) to modify the `IntCollection`. It handles adding new elements, removing the last element, and incrementing the value of a random element in the list.
3.  **`CollectionViewHandler.cs`:** This component is responsible for visualizing the collection. It subscribes to the `IntCollection`'s `SubscribeOnAdd`, `SubscribeOnRemove`, and `SubscribeToValues` events.
    *   When an item is added or removed, it activates or deactivates the corresponding UI panel.
    *   When an element's value changes, it updates the text label of the correct panel.

This sample showcases how a reactive list can be used to drive a dynamic UI, with different components responsible for modifying and viewing the data, all without holding direct references to each other.
