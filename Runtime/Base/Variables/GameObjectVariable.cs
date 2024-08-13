using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "GameObjectVariable", menuName = MenuHelper.DefaultVariableMenu + "GameObject")]
    public sealed class GameObjectVariable : VariableCore<GameObject>
    {
    }
}