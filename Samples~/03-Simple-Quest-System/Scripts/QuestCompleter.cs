using Soar.Transactions;
using UnityEngine;
using UnityEngine.UI;

namespace Soar.Samples.QuestSystem
{
    public class QuestCompleter : MonoBehaviour
    {
        [SerializeField] private CompleteQuestTransaction completeQuestTransaction;
        [SerializeField] private string questIdToComplete;
        [SerializeField] private Button completeButton;

        private void OnEnable()
        {
            if (completeButton != null)
            {
                completeButton.onClick.AddListener(TryCompleteQuest);
            }
        }

        private void OnDisable()
        {
            if (completeButton != null)
            {
                completeButton.onClick.RemoveListener(TryCompleteQuest);
            }
        }

        public void TryCompleteQuest()
        {
            if (string.IsNullOrEmpty(questIdToComplete) || completeQuestTransaction == null)
            {
                Debug.LogWarning("Quest ID or Transaction not set.");
                return;
            }

            Debug.Log($"Attempting to complete quest: {questIdToComplete}...");
            if(completeButton != null) completeButton.interactable = false;

            completeQuestTransaction.Request(questIdToComplete, OnQuestCompleted);
        }

        private void OnQuestCompleted(bool success)
        {
            Debug.Log($"Quest completion response received. Success: {success}");
            if(completeButton != null) completeButton.interactable = true;
        }
    }
}
