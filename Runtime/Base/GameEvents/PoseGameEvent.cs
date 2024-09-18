using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "PoseEvent", menuName = MenuHelper.DefaultGameEventMenu + "PoseEvent")]
    public sealed class PoseGameEvent : GameEvent<Pose>
    {
    }
}