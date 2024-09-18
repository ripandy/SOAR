using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "BoolTransaction", menuName = MenuHelper.DefaultTransactionMenu + "BoolTransaction")]
    public sealed class BoolTransaction : Transaction<bool>
    {
    }
}