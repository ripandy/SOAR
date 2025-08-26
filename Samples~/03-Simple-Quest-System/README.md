# 03: Simple Quest System Sample

## Overview

This sample is a more advanced example that demonstrates how multiple SOAR features can be combined to create a simple, yet robust, quest system. It shows how to manage a list of data, handle a request/response interaction, and encapsulate a complex operation.

## Scene Setup

The scene will contain a UI to display a list of active quests, a button to attempt to complete a specific quest, and a "Save" button.

## How it Works

This sample is composed of several interacting parts:

1.  **The Data (`Quest.cs`, `QuestList.cs`):**
    *   A `Quest` struct holds the data for a single quest (ID, description, status).
    *   A `QuestList` (`SoarList<Quest>`) asset holds the list of all currently active quests.

2.  **The View (`QuestUIManager.cs`):**
    *   This component subscribes to the `QuestList` collection. It dynamically instantiates and destroys UI prefabs to visualize the list of active quests, reacting automatically when quests are added or removed.

3.  **The Completion Logic (`CompleteQuestTransaction.cs`, `QuestCompleter.cs`, `QuestCompletionHandler.cs`):
    *   A `CompleteQuestTransaction` (`Transaction<string, bool>`) defines the contract for completing a quest: it takes a quest ID string and returns a boolean indicating success.
    *   The `QuestCompleter` component acts as the **requester**. A UI button is wired to its `TryCompleteQuest` method, which calls `Request()` on the transaction.
    *   The `QuestCompletionHandler` component acts as the **responder**. It registers its logic with the transaction, finds the quest in the `QuestList`, updates its status, and returns `true` or `false`.

4.  **The Save Operation (`SaveQuestsCommand.cs`):**
    *   A `SaveQuestsCommand` (`Command`) asset encapsulates the logic for saving quest progress. In this sample, it simply logs the current quest data to the console, but it could be expanded to write to a file.
    *   A "Save" button in the scene calls the `Execute()` method on this command.

## Key Concepts

*   **Combining Features:** This sample shows the true power of SOAR, using a `Collection` for state, a `Transaction` for a request/response interaction, and a `Command` for a one-way operation, all working together.
*   **Request/Response Pattern:** The `Transaction` perfectly decouples the UI button that *requests* a quest to be completed from the game system that actually *processes* that completion.
*   **Encapsulating Operations:** The `Command` provides a clean, single entry point for a potentially complex operation like saving game data. Any part of the game can now trigger a save by simply executing this command, without needing to know the details of the save logic.
