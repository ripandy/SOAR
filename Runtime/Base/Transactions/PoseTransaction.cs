using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "PoseTransaction", menuName = MenuHelper.DefaultTransactionMenu + "PoseTransaction")]
    public sealed class PoseTransaction : Transaction<Pose>
    {
    }
}