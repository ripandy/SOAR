using System;
using System.Collections.Generic;
using UnityEngine;

namespace Soar.Samples.QuestSystem
{
    public class QuestUIManager : MonoBehaviour
    {
        [SerializeField] private QuestList questList;
        [SerializeField] private QuestView questViewPrefab;
        [SerializeField] private Transform questViewParent;

        private readonly List<QuestView> spawnedQuestViews = new();
        private readonly List<IDisposable> subscriptions = new();

        private void Start()
        {
            // Clear and populate UI with initial data
            ClearUI();
            foreach (var quest in questList)
            {
                AddQuestView(quest);
            }

            // Subscribe to collection add
            var sub = questList.SubscribeOnAdd(AddQuestView);
            subscriptions.Add(sub);
        }

        private void AddQuestView(Quest quest)
        {
            var valueSub = questList.SubscribeToValues(OnQuestUpdated);
            subscriptions.Add(valueSub);
            
            var questUIInstance = Instantiate(questViewPrefab, questViewParent);
            spawnedQuestViews.Add(questUIInstance);
        }

        private void OnQuestUpdated(int index, Quest quest)
        {
            var questUI = spawnedQuestViews[index];
            questUI.UpdateView(quest);
        }

        private void ClearUI()
        {
            foreach (var questView in spawnedQuestViews)
            {
                Destroy(questView);
            }
            spawnedQuestViews.Clear();
        }

        private void OnDestroy()
        {
            foreach (var subscription in subscriptions)
            {
                subscription?.Dispose();
            }
        }
    }
}
