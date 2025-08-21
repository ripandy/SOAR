using System;
using TMPro;
using UnityEngine;

namespace Soar.Transactions.Sample
{
    public class SubscribeToResponseSample : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;
        [SerializeField] private TMP_Text label;

        private IDisposable subscription;

        private void Start()
        {
            subscription = dummyProcessTransaction
                .SubscribeToResponse(value => label.text = $"Response received.\nResponse value: {value}");
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}