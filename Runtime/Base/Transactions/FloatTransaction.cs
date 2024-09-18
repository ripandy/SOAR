using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "FloatTransaction", menuName = MenuHelper.DefaultTransactionMenu + "FloatTransaction")]
    public sealed class FloatTransaction : Transaction<float>
    {
    }
}