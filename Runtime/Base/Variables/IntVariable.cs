using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "IntVariable", menuName = MenuHelper.DefaultVariableMenu + "Int")]
    public sealed class IntVariable : VariableCore<int>
    {
    }
}