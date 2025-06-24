using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "LongList", menuName = MenuHelper.DefaultListMenu + "LongList")]
    public sealed class LongList : SoarList<long>
    {
    }
}