using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "QuaternionList", menuName = MenuHelper.DefaultListMenu + "QuaternionList")]
    public sealed class QuaternionList : SoarList<Quaternion>
    {
    }
}