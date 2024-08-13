using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "ByteVariable", menuName = MenuHelper.DefaultVariableMenu + "Byte")]
    public sealed class ByteVariable : VariableCore<byte>
    {
    }
}