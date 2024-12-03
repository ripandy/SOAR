using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "QuaternionCollection", menuName = MenuHelper.DefaultCollectionMenu + "QuaternionCollection")]
    public sealed class QuaternionCollection : List<Quaternion>
    {
    }
}