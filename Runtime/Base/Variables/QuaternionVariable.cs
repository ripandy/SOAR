using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "QuaternionVariable", menuName = MenuHelper.DefaultVariableMenu + "Quaternion")]
    public sealed class QuaternionVariable : VariableCore<Quaternion>
    {
    }
}