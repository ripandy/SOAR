using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "QuaternionEvent", menuName = MenuHelper.DefaultGameEventMenu + "QuaternionEvent")]
    public sealed class QuaternionGameEvent : GameEvent<Quaternion>
    {
    }
}