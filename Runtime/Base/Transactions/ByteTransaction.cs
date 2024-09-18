using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "ByteTransaction", menuName = MenuHelper.DefaultTransactionMenu + "ByteTransaction")]
    public sealed class ByteTransaction : Transaction<byte>
    {
    }
}