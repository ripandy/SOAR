using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "QuaternionTransaction", menuName = MenuHelper.DefaultTransactionMenu + "QuaternionTransaction")]
    public sealed class QuaternionTransaction : Transaction<Quaternion>
    {
    }
}