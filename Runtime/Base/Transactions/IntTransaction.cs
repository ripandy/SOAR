using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "IntTransaction", menuName = MenuHelper.DefaultTransactionMenu + "IntTransaction")]
    public sealed class IntTransaction : Transaction<int>
    {
    }
}