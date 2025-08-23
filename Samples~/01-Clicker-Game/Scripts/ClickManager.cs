using Soar.Events;
using Soar.Variables;
using System;
using UnityEngine;

namespace Soar.Samples.ClickerGame
{
    public class ClickManager : MonoBehaviour
    {
        [SerializeField] private GameEvent onClickEvent;
        [SerializeField] private IntVariable scoreVariable;

        private IDisposable subscription;

        private void OnEnable()
        {
            subscription = onClickEvent.Subscribe(OnClicked);
        }

        private void OnClicked()
        {
            scoreVariable.Value++;
        }

        private void OnDisable()
        {
            subscription?.Dispose();
        }
    }
}
