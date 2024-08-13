using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = MenuHelper.DefaultVariableMenu + "Float")]
    public sealed class FloatVariable : VariableCore<float>
    {
    }
}