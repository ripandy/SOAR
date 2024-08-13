using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "DoubleEvent", menuName = MenuHelper.DefaultGameEventMenu + "DoubleEvent")]
    public sealed class DoubleGameEvent : GameEvent<double>
    {
    }
}