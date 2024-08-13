using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "TransformVariable", menuName = MenuHelper.DefaultVariableMenu + "Transform")]
    public sealed class TransformVariable : VariableCore<Transform>
    {
    }
}