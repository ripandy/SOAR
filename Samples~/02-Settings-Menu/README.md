# 02: Settings Menu Sample

## Overview

This sample demonstrates how to use a `JsonableVariable<T>` and the `JsonableExtensions` to create a settings menu where data can be easily saved to and loaded from a local file.

## Scene Setup

The scene will contain a UI with sliders for music and SFX volume, a toggle for enabling/disabling tutorials, and two buttons: "Save" and "Load". These buttons will be wired to the public methods on the `SettingsSaveLoad` component.

## How it Works

1.  **`GameSettings` Struct:** A simple `struct` is defined to hold all the settings data (`musicVolume`, `sfxVolume`, `showTutorials`) in one place.
2.  **`GameSettingsVariable`:** A `JsonableVariable<GameSettings>` asset is created. This asset holds the current state of the settings in memory.
3.  **`SettingsUIManager.cs`:** This component is responsible for the UI. It listens to the UI sliders and toggle and updates the `GameSettingsVariable`. It also subscribes to the variable to automatically update the UI display if the data changes from another source (like loading).
4.  **`SettingsSaveLoad.cs`:** This component is responsible for data persistence. It exposes public `SaveSettings()` and `LoadSettings()` methods that are called by the UI buttons. On `Start`, it automatically loads any existing settings.

## Key Concepts

*   **Single Responsibility Principle (SRP):** The UI logic (`SettingsUIManager`) is completely separate from the data persistence logic (`SettingsSaveLoad`). This makes each component simpler and easier to maintain.
*   **Data Persistence:** Demonstrates a practical, easy-to-implement solution for saving and loading data using SOAR.
*   **`JsonableVariable<T>`:** Shows how to make a custom data structure serializable to JSON by inheriting from this class.