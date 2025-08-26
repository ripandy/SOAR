using System;

namespace Soar.Samples.QuestSystem
{
    [Serializable]
    public struct Quest
    {
        public string questId;
        public string title;
        public string description;
        public QuestStatus status;
    }
    
    public enum QuestStatus
    {
        Unavailable,
        Available,
        InProgress,
        Completed
    }
}
