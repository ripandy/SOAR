using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "LongTransaction", menuName = MenuHelper.DefaultTransactionMenu + "LongTransaction")]
    public sealed class LongTransaction : Transaction<long>
    {
    }
}