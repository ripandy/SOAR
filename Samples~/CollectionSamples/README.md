# Collection Sample

## Overview

This sample demonstrates how to use a `SoarList<T>` to manage a collection of data and drive a dynamic UI.

## Scene Setup

The `CollectionSample` scene contains a grid of UI panels, each with a text label. There are buttons to add a new panel, remove a panel, and update the value of a random panel.

## How it Works

1.  **`IntCollection` (`SoarList<int>`):** An `IntCollection` asset stores a list of integers. Each integer corresponds to the value displayed on one of the UI panels.
2.  **`CollectionEventHandler.cs`:** This component listens to UI button events (`GameEvent`s) to modify the `IntCollection`. It handles adding new elements, removing the last element, and incrementing the value of a random element in the list.
3.  **`CollectionViewHandler.cs`:** This component is responsible for displaying the collection's data. It subscribes to the `IntCollection`'s `SubscribeOnAdd`, `SubscribeOnRemove`, and `SubscribeToValues` events.
    *   When an item is added or removed, it activates or deactivates the corresponding UI panel.
    *   When an element's value changes, it updates the text label of the correct panel.

## Key Concepts

*   **Reactive Data:** The UI automatically reacts to changes in the `SoarList<T>` data.
*   **Separation of Concerns:** The logic for modifying the collection (`CollectionEventHandler`) is separate from the logic for displaying it (`CollectionViewHandler`).
*   **Decoupling:** The components that modify and display the data do not have direct references to each other. They only share a reference to the `IntCollection` asset.
