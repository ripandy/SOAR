using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "IntEvent", menuName = MenuHelper.DefaultGameEventMenu + "IntEvent")]
    public sealed class IntGameEvent : GameEvent<int>
    {
    }
}