using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "LongEvent", menuName = MenuHelper.DefaultGameEventMenu + "LongEvent")]
    public sealed class LongGameEvent : GameEvent<long>
    {
    }
}