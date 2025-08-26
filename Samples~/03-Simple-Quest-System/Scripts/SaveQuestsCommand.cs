using System.Collections.Generic;
using Soar.Commands;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Soar.Samples.QuestSystem
{
    [CreateAssetMenu(fileName = "SaveQuestsCommand", menuName = "SOAR/Samples/Quest System/Save Quests Command")]
    public class SaveQuestsCommand : Command<IReadOnlyCollection<Quest>>
    {
        public override void Execute(IReadOnlyCollection<Quest> quests)
        {
            _ = ExecuteAsync(quests, Application.exitCancellationToken);
        }

        public override async ValueTask ExecuteAsync(IReadOnlyCollection<Quest> quests, CancellationToken cancellationToken = default)
        {
            if (quests == null || quests.Count == 0) return;

            // In a real application, this is where the quest data would be
            // serialized (e.g., to JSON) and sent to a server.
            foreach (var quest in quests)
            {
                Debug.Log($"Saving Quest ID: {quest.questId}, Completed: {quest.isCompleted}");
            }

            // Simulate network latency.
            Debug.Log("Sending data to server... Please wait.");
            await Task.Delay(1500, cancellationToken); // Wait for 1.5 seconds

            Debug.Log("--- Save Complete (Response from server received) ---");
        }
    }
}