using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "LongVariable", menuName = MenuHelper.DefaultVariableMenu + "Long")]
    public sealed class LongVariable : VariableCore<long>
    {
    }
}