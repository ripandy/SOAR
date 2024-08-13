using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "StringEvent", menuName = MenuHelper.DefaultGameEventMenu + "StringEvent")]
    public sealed class StringGameEvent : GameEvent<string>
    {
    }
}