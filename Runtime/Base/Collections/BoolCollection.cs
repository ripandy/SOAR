using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "BoolCollection", menuName = MenuHelper.DefaultCollectionMenu + "BoolCollection")]
    public sealed class BoolCollection : SoarList<bool>
    {
    }
}