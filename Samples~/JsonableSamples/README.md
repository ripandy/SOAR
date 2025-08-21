# Jsonable Variable Sample

## Overview

This sample demonstrates how to use `JsonableVariable<T>` and the `JsonableExtensions` to save and load data to and from a JSON file.

## Scene Setup

The `JsonableSample` scene contains a UI for editing the fields of a custom data structure, along with "Save" and "Load" buttons.

## How it Works

1.  **`CustomStruct`:** A simple `struct` containing a bool, int, float, and string.
2.  **`CustomVariable`:** A `JsonableVariable<CustomStruct>` asset is created to hold the data.
3.  **`DataEditor.cs`:** This component provides the UI to modify the values of the `CustomVariable` at runtime. It subscribes to the variable to keep the UI in sync with the data.
4.  **`JsonUtilityHandler.cs`:** This component handles the save/load logic.
    *   When the "Save" button is pressed, it calls the `SaveToJson()` extension method on the `CustomVariable`, serializing its data to a JSON file in the project's persistent data path (which varies by platform).
    *   When the "Load" button is pressed, it calls `LoadFromJson()` to overwrite the variable's current data with the data from the saved file.

## Key Concepts

*   **Data Persistence:** `JsonableVariable<T>` provides a simple way to save and load data, which is essential for features like game settings or player progress.
*   **Serialization:** The `JsonableExtensions` handle the serialization and deserialization of your data to and from JSON.
*   **Decoupling:** The data (`CustomVariable`) is decoupled from the UI (`DataEditor`) and the save/load logic (`JsonUtilityHandler`).
