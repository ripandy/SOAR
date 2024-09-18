using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = MenuHelper.DefaultVariableMenu + "String")]
    public sealed class StringVariable : Variable<string>
    {
    }
}