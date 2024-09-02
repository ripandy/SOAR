using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "BoolVariable", menuName = MenuHelper.DefaultVariableMenu + "Bool")]
    public sealed class BoolVariable : Variable<bool>
    {
        public void ToggleValue() => Value = !Value;
    }
}