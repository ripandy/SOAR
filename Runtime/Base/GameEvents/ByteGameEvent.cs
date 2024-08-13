using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "ByteEvent", menuName = MenuHelper.DefaultGameEventMenu + "ByteEvent")]
    public sealed class ByteGameEvent : GameEvent<byte>
    {
    }
}