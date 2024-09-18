using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "PoseCollection", menuName = MenuHelper.DefaultCollectionMenu + "PoseCollection")]
    public sealed class PoseCollection : Collection<Pose>
    {
    }
}