using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "ByteCollection", menuName = MenuHelper.DefaultCollectionMenu + "ByteCollection")]
    public sealed class ByteCollection : List<byte>
    {
    }
}