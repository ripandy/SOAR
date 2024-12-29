using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "LongCollection", menuName = MenuHelper.DefaultCollectionMenu + "LongCollection")]
    public sealed class LongCollection : SoarList<long>
    {
    }
}