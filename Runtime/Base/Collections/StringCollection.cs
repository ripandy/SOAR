using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "StringCollection", menuName = MenuHelper.DefaultCollectionMenu + "StringCollection")]
    public sealed class StringCollection : List<string>
    {
    }
}