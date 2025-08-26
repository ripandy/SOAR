using UnityEngine;

namespace Soar.Samples.QuestSystem
{
    public class QuestCompletionHandler : MonoBehaviour
    {
        [SerializeField] private CompleteQuestTransaction completeQuestTransaction;
        [SerializeField] private QuestList activeQuests;

        private void OnEnable()
        {
            // Register this component as the one that will handle the transaction logic.
            completeQuestTransaction.RegisterResponse(CompleteQuest);
        }

        private bool CompleteQuest(string questId)
        {
            // Find the quest in the list.
            for (int i = 0; i < activeQuests.Count; i++)
            {
                if (activeQuests[i].questId == questId)
                {
                    if (activeQuests[i].isCompleted)
                    {
                        Debug.Log($"Quest '{questId}' is already completed.");
                        return false; // Quest was already complete
                    }

                    // Create a new struct with the updated data.
                    var completedQuest = activeQuests[i];
                    completedQuest.isCompleted = true;

                    // Replace the old quest with the updated one.
                    activeQuests[i] = completedQuest;
                    
                    // For this sample, we'll also remove it from the active list.
                    // In a real game, you might move it to a 'completedQuests' list instead.
                    activeQuests.RemoveAt(i);

                    Debug.Log($"Quest '{questId}' completed successfully!");
                    return true; // Quest successfully completed
                }
            }

            Debug.LogWarning($"Quest with ID '{questId}' not found.");
            return false; // Quest not found
        }
    }
}
