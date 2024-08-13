using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "BoolEvent", menuName = MenuHelper.DefaultGameEventMenu + "BoolEvent")]
    public sealed class BoolGameEvent : GameEvent<bool>
    {
        public void RaiseToggled() => Raise(!value);
    }
}