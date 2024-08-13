using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "ColorVariable", menuName = MenuHelper.DefaultVariableMenu + "Color")]
    public sealed class ColorVariable : VariableCore<Color>
    {
    }
}