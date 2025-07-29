using System;
using TMPro;
using UnityEngine;

namespace Soar.Transactions.Sample
{
    public class SubscribeToRequestSample : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;
        [SerializeField] private TMP_Text label;

        private IDisposable subscription;

        private void Start()
        {
            subscription = dummyProcessTransaction
                .SubscribeToRequest(value => label.text = $"Request sent. Request value: {value}");
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}