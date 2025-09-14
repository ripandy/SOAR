# Variable

The `Variable<T>` class in SOAR represents a data container stored in a `ScriptableObject`.
It extends the `GameEvent<T>` system, meaning that whenever the `Value` of a `Variable<T>` changes, it also raises an event, notifying all subscribers of the new value.
This makes `Variable<T>` a powerful tool for creating reactive data that can be easily shared and observed throughout application.

`Variable<T>` inherits from `GameEvent<T>` with Raise and Subscribe event features, and thus from `SoarCore`, gaining lifecycle management features.

## `Variable<T>`

The generic `Variable<T>` class is the base for all variable types. It holds a value of type `T` and provides mechanisms to access, modify, and observe this value.

![SOAR_Variable-Inspector](../assets/images/SOAR_Variable-Inspector.png)

- **Value Property**: The primary way to interact with a `Variable<T>` is through its `Value` property. Assigning a new value to the `Value` property will trigger the `Raise(T)` method, which in turn notifies subscribers.

- **Value Event Type**: The `valueEventType` field (configurable in the Inspector) determines when the value change event is raised:
  
    - `ValueAssign`: The event is raised every time the `Value` property is set, even if the new value is the same as the old one.
    - `ValueChanged`: The event is raised only if the new value is different from the current value.
    This is checked by the `IsValueEquals(T)` method.

- **Subscribing to Changes**: Since `Variable<T>` inherits from `GameEvent<T>`, subscription to value changes can be done by using the same `Subscribe` methods:
  
    - `Subscribe(Action<T> action)`: Receives the new value.
    - `Subscribe(Action<T, T> action)`: Receives both the old and new value. This is specific to `Variable<T>`.
    - `Subscribe(Action<PairwiseValue<T>> action)`: Receives a `PairwiseValue<T>` struct containing both old and new values. This is also specific to `Variable<T>`.

- **Initial Value and Reset**:
  
    - The `InitialValue` property stores the value of the variable at initialization time (e.g., when the game starts or the editor compiles).
    - The `ResetValue()` method allows reverting the variable's `Value` back to its `InitialValue`.

- **Auto Reset Value**: The `autoResetValue` boolean field (configurable in the Inspector) determines if the `Variable<T>` should automatically call `ResetValue()` when play mode ends. This is useful for ensuring variables return to a known state after testing.

!!! Note "Serialization and Deep Copy"
    Due to how Unity serializes class types, `autoResetValue` is more reliable with struct and primitive types. For complex class types, deep copying for `InitialValue` relies on JSON serialization internally, which can cause unnecessary overhead and might not cover all engine types like `Transform` or `GameObject`.

### Creating a Variable

SOAR provides several pre-defined `Variable` types for common Unity and C# types (e.g., `IntVariable`, `FloatVariable`, `StringVariable`, `Vector3Variable`). These can be created from the `Assets > Create > SOAR > Variables` menu.

!!! Note "Assembly Definition Reference"
    These common types are defined within `Soar.Base.asmdef` assembly definition files, which different from SOAR's core classes `Soar.asmdef` assembly definition files. Make sure `Soar.Base.asmdef` reference was added to the asmdef that requires it.

To create a variable for a custom data type:

1.  Define a custom data structure. In order to make the data viewable and modifiable in the Inspector, it is a necessity to set it as `[Serializable]`.
    
    ```csharp
    // File: MyCustomData.cs
    using System;

    [Serializable]
    public struct MyCustomData
    {
        public int score;
        public string playerName;
    }
    ```

2.  Create a new class that inherits from `Variable<T>`, specifying the custom type.

    ```csharp
    // File: MyCustomDataVariable.cs
    using Soar;
    using Soar.Variables;
    using UnityEngine;

    [CreateAssetMenu(fileName = "MyCustomDataVariable", menuName = MenuHelper.DefaultVariableMenu + "My Custom Data Variable")]
    public class MyCustomDataVariable : Variable<MyCustomData> { }
    ```

3.  Instances of `MyCustomDataVariable` can now be created from the `Assets > Create > SOAR > Variables > My Custom Data Variable` menu.

### Accessing and Modifying Value

The Value of a variable can be accessed from `Value` getter (property). Using the same property, a value of a variable can be set from `Value` setter (property). Modifying the value will Raise value-changed event.

```csharp
// File: VariableUserExample.cs
using Soar.Variables;
using UnityEngine;

public class VariableUserExample : MonoBehaviour
{
    [SerializeField] private IntVariable scoreVariable;

    void Start()
    {
        Debug.Log($"Initial Score: {scoreVariable.Value}");
    }

    public void AddScore(int amount)
    {
        scoreVariable.Value += amount;
    }
}
```

### Subscribing to Value Changes

Changes in a `Variable<T>` can be observed by establishing a subscription to it. This mechanism is analogous to subscribing to a `GameEvent<T>`. Upon a change in the `Value` of the `Variable<T>`, all subscribed actions are invoked. To prevent memory leaks and unintended behavior, the subscription should be disposed of when it is no longer required, commonly within `OnDisable` or `OnDestroy`.

```csharp
// File: ScoreDisplay.cs
using System;
using Soar.Variables;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private IntVariable scoreVariable;
    [SerializeField] private Text scoreText;
    private IDisposable scoreSubscription;

    private void Start()
    {
        // Subscribe to receive the new value
        scoreSubscription = scoreVariable.Subscribe(UpdateScoreText);

        // Optionally, subscribe to receive old and new values
        // scoreSubscription = scoreVariable.Subscribe(HandleScoreChangeDetailed);

        UpdateScoreText(scoreVariable.Value); // Update text with initial value
    }

    private void OnDestroy()
    {
        scoreSubscription?.Dispose();
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newScore}";
        }
    }

    // Example for old/new value subscription
    private void HandleScoreChangeDetailed(int oldValue, int newValue)
    {
        Debug.Log($"Score changed from {oldValue} to {newValue}");
        UpdateScoreText(newValue);
    }
}
```

## `JsonableVariable<T>`

For variables that need to be serialized to or deserialized from JSON, SOAR provides the `JsonableVariable<T>` base class. This class implements the `IJsonable` interface.

- **`ToJsonString()`**: Converts the variable's current `Value` into a JSON string.

  - Primitive types are wrapped in a `JsonableWrapper<T>` (e.g., `{"value": 10}`) for robust serialization.
  - Complex types are serialized directly.
  - Uses `JsonUtility.ToJson`, with pretty print enabled in the Unity Editor.

- **`FromJsonString(string jsonString)`**: Parses a JSON string and updates the variable's `Value`.

  - Handles unwrapping for both primitive and complex types.

custom jsonable variables can be created similarly to regular variables, but by inheriting from `JsonableVariable<T>`:

```csharp
// File: MyJsonableDataVariable.cs
using Soar.Variables;
using UnityEngine;

// Assuming MyCustomData is [Serializable]
[CreateAssetMenu(fileName = "MyJsonableDataVariable", menuName = "SOAR/Jsonable Variables/My Jsonable Data Variable")]
public class MyJsonableDataVariable : JsonableVariable<MyCustomData> { }
```

The [`Soar.JsonableExtensions`](Runtime/Utility/JsonableExtensions.cs) class provides helper methods like `SaveToJson` and `LoadFromJson` to easily save/load `IJsonable` objects to/from files.

## Editor Integration

`Variable<T>` and `JsonableVariable<T>` assets have custom editor inspectors to enhance usability.

### `Variable<T>` Inspector (`VariableEditor`)

- **Value Display**: The current `Value` of the variable is displayed. If `T` is a complex serializable type, its fields are shown.
- **Raise Button**: Inherited from `GameEventEditor`, this button calls `Raise(Value)` on the variable, useful for testing listeners.
- **Instance Settings**:
  - `Value Event Type`: Dropdown to select `ValueAssign` or `ValueChanged`.
  - `Auto Reset Value`: Checkbox to enable/disable automatic value reset on exiting play mode.

### `JsonableVariable<T>` Inspector (`JsonableVariableEditor`)

Inherits all features from the `Variable<T>` inspector and adds:

- **Json File Management**: A foldout section for saving and loading the variable's data to/from a JSON file.
  - **Json Path Type**: Choose between `Application.dataPath`, `Application.persistentDataPath`, or a custom path.
  - **File Name**: Specify the JSON file name. Can default to `[VariableName].json`.
  - **Save to Json Button**: Saves the current variable `Value` to the specified JSON file.
  - **Load from Json Button**: Loads the `Value` from the specified JSON file into the variable.

## Lifecycle and Disposal

Being derived from `GameEvent<T>` and `SoarCore`, `Variable<T>` instances:

- Are `ScriptableObject`s and implement `IDisposable`.
- Participate in SOAR's initialization and cleanup lifecycle, including handling for editor play mode state changes and domain reloads.
- The `value` field of a `GameEvent<T>` (and thus `Variable<T>`) is reset to `default(T)` when the application quits or when domain reload is disabled and play mode is exited, as part of `GameEvent<T>.ResetInternal()`. The `autoResetValue` feature in `Variable<T>` controls whether `InitialValue` is restored.

## Sample Usage

To test this feature, the relevant sample package can be imported from the Package Manager window.

![SOAR_ImportSamples_Variable](../assets/images/SOAR_ImportSamples_Variable.png)

The **Variable Sample** demonstrates how multiple components can react to changes in shared `FloatVariable` assets that represent a character's health. It includes components for damaging, healing, and upgrading health, all of which interact with the same data assets without being directly aware of each other.

For detailed setup and usage instructions, please refer to the `README.md` file inside the `VariableSamples` folder after importing.
