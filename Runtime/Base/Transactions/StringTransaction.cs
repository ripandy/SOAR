using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "StringTransaction", menuName = MenuHelper.DefaultTransactionMenu + "StringTransaction")]
    public sealed class StringTransaction : Transaction<string>
    {
    }
}