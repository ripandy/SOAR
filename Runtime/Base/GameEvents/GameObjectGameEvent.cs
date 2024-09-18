using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "GameObjectEvent", menuName = MenuHelper.DefaultGameEventMenu + "GameObjectEvent")]
    public sealed class GameObjectGameEvent : GameEvent<GameObject>
    {
    }
}