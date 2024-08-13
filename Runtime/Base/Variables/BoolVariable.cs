using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "BoolVariable", menuName = MenuHelper.DefaultVariableMenu + "Bool")]
    public sealed class BoolVariable : VariableCore<bool>
    {
        public void ToggleValue() => Value = !Value;
    }
}