using TMPro;
using UnityEngine;

namespace Soar.Samples.QuestSystem
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField] private TMP_Text questTitle;
        [SerializeField] private TMP_Text questDescription;
        [SerializeField] private GameObject unavailableIndicator;
        [SerializeField] private GameObject availableIndicator;
        [SerializeField] private GameObject inProgressIndicator;
        [SerializeField] private GameObject completedIndicator;
        
        public void UpdateView(Quest quest)
        {
            if (questTitle != null)
                questTitle.text = quest.title;
            if (questDescription != null)
                questDescription.text = quest.description;

            gameObject.SetActive(quest.status != QuestStatus.Unavailable);
            availableIndicator.SetActive(quest.status == QuestStatus.Available);
            inProgressIndicator.SetActive(quest.status == QuestStatus.InProgress);
            completedIndicator.SetActive(quest.status == QuestStatus.Completed);
        }
    }
}