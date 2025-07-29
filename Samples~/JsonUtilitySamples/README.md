# JsonUtility Sample

This sample demonstrates the use of the `JsonableVariable<T>` feature and the `JsonableExtensions` for saving and loading data.

## Scene

The `JsonUtilitySample` scene contains a UI for editing the fields of a custom data structure, along with "Save" and "Load" buttons.

## How it Works

1.  **`CustomStruct`:** A simple `struct` containing a bool, int, float, and string.
2.  **`CustomVariable`:** A `JsonableVariable<CustomStruct>` asset is created to hold the data.
3.  **`DataEditor.cs`:** This component provides the UI to modify the values of the `CustomVariable` at runtime. It subscribes to the variable to keep the UI in sync with the data.
4.  **`JsonUtilityHandler.cs`:** This component handles the save/load logic. 
    *   When the "Save" button is pressed, it calls the `SaveToJson()` extension method on the `CustomVariable`, serializing its data to a JSON file in the project's `Application.persistentDataPath`.
    *   When the "Load" button is pressed, it calls `LoadFromJson()` to overwrite the variable's current data with the data from the saved file.

This sample showcases a practical data persistence workflow, allowing game settings, player progress, or other data to be easily saved and loaded.
