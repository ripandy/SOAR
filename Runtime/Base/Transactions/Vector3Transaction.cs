using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "Vector3Transaction", menuName = MenuHelper.DefaultTransactionMenu + "Vector3Transaction")]
    public sealed class Vector3Transaction : Transaction<Vector3>
    {
    }
}