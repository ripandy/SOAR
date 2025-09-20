# Creating Custom SOAR Assets

While SOAR comes with a rich set of "Base" assets for primitive types (like `IntVariable`, `StringList`, etc.), the framework's true potential is unlocked by creating assets tailored to a project's specific data structures. This guide demonstrates the process of creating a complete set of custom SOAR assets for a common use case: managing player data.

The process is simple, consistent, and requires very little boilerplate code. Once created, the custom assets are seamlessly integrated into Unity's `Assets > Create` menu, just like the built-in types.

## The Example: `PlayerData`

For this guide, a set of SOAR assets will be created to work with the following `PlayerData` struct. This struct holds all the essential information about a player.

### Step 1: Define the Data Structure

First, a new C# script for the data must be created. The most important step is to mark the `struct` or `class` with the `[System.Serializable]` attribute. This allows Unity to serialize it and display it in the Inspector.

```csharp
// File: PlayerData.cs
using System;

[Serializable]
public struct PlayerData
{
    public string playerName;
    public int level;
    public float health;
}
```

With this `PlayerData` struct, a `Variable`, `GameEvent`, `Collection`, and `Transaction` can now be created for it.

## Step 2: Create a Custom `Variable`

A `Variable` holds the state of a single piece of data. A `PlayerDataVariable` can be created to store the current player's information.

A new script named `PlayerDataVariable.cs` should be created:

```csharp
// File: PlayerDataVariable.cs
using Soar;
using Soar.Variables;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataVariable", menuName = MenuHelper.DefaultVariableMenu + "Player Data")]
public class PlayerDataVariable : Variable<PlayerData> { }
```

The `[CreateAssetMenu]` attribute makes it available in the editor. `MenuHelper.DefaultVariableMenu` is used to ensure it appears in the same sub-menu as all other variables (`SOAR > Variables`).

**Result:** A new `PlayerDataVariable` asset can now be created from `Assets > Create > SOAR > Variables > Player Data`.

## Step 3: Create a Custom `GameEvent`

A `GameEvent` is used to broadcast that something has happened. A `PlayerDataGameEvent` could be used to announce when a player has connected or their data has been updated.

A new script named `PlayerDataGameEvent.cs` should be created:

```csharp
// File: PlayerDataGameEvent.cs
using Soar;
using Soar.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataEvent", menuName = MenuHelper.DefaultGameEventMenu + "Player Data")]
public class PlayerDataGameEvent : GameEvent<PlayerData> { }
```

**Result:** This event can now be created from `Assets > Create > SOAR > Game Events > Player Data`.

## Step 4: Create a Custom `Collection` (List)

A `Collection` manages a list or dictionary of items. A `PlayerDataList` can be created to store a list of all players in a multiplayer lobby.

A new script named `PlayerDataList.cs` should be created:

```csharp
// File: PlayerDataList.cs
using Soar;
using Soar.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataList", menuName = MenuHelper.DefaultListMenu + "Player Data")]
public class PlayerDataList : SoarList<PlayerData> { }
```

**Result:** This list can now be created from `Assets > Create > SOAR > Lists > Player Data`.

## Step 5: Create a Custom `Transaction`

A `Transaction` handles a request-response cycle. A `FetchPlayerDataTransaction` can be created that takes a `string` (the player's ID) as a request and returns the corresponding `PlayerData` as a response.

A new script named `FetchPlayerDataTransaction.cs` should be created:

```csharp
// File: FetchPlayerDataTransaction.cs
using Soar;
using Soar.Transactions;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFetchPlayerDataTransaction", menuName = MenuHelper.DefaultTransactionMenu + "Fetch Player Data")]
public class FetchPlayerDataTransaction : Transaction<string, PlayerData> { }
```

**Result:** This transaction can now be created from `Assets > Create > SOAR > Transactions > Fetch Player Data`.

## Step 6: Create a Custom `UnityEventBinder`

To easily wire up the new `PlayerDataGameEvent` in the editor, a custom `UnityEventBinder` can be created.

A new script named `PlayerDataUnityEventBinder.cs` should be created:

```csharp
// File: PlayerDataUnityEventBinder.cs
using Soar.Events;

public class PlayerDataUnityEventBinder : UnityEventBinder<PlayerData> { }
```

**Result:** The `PlayerDataUnityEventBinder` component can now be added to any `GameObject`. It allows for listening to a `PlayerDataGameEvent` and invoking a `UnityEvent<PlayerData>`, passing the event data directly to other components in the Inspector.

## Conclusion

By following this simple, consistent pattern, a robust, type-safe, and editor-friendly architecture can be created for any data type in a project. This makes the codebase more modular, easier to test, and empowers the entire team to wire up complex logic directly in the Unity Editor without writing additional code.