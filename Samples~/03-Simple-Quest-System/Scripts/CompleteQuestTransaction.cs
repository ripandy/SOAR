using Soar.Transactions;
using UnityEngine;

namespace Soar.Samples.QuestSystem
{
    [CreateAssetMenu(fileName = "CompleteQuestTransaction", menuName = "SOAR/Samples/Quest System/Complete Quest Transaction")]
    public class CompleteQuestTransaction : Transaction<string, bool>
    {
    }
}
