using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "PoseList", menuName = MenuHelper.DefaultListMenu + "PoseList")]
    public sealed class PoseList : SoarList<Pose>
    {
    }
}