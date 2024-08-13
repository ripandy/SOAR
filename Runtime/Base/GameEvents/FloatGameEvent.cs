using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "FloatEvent", menuName = MenuHelper.DefaultGameEventMenu + "FloatEvent")]
    public sealed class FloatGameEvent : GameEvent<float>
    {
    }
}