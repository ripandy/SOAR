using UnityEngine;

namespace Soar.Transactions
{
    [CreateAssetMenu(fileName = "Vector2Transaction", menuName = MenuHelper.DefaultTransactionMenu + "Vector2Transaction")]
    public sealed class Vector2Transaction : Transaction<Vector2>
    {
    }
}