using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "DoubleTransaction", menuName = MenuHelper.DefaultTransactionMenu + "DoubleTransaction")]
    public sealed class DoubleTransaction : Transaction<double>
    {
    }
}