using System;
using UnityEngine;

namespace Soar.Variables.Sample
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private FloatVariable healthVariable;
        [SerializeField] private FloatVariable maxHealthVariable;
        [SerializeField] private RectTransform healthBar;

        private IDisposable subscription;

        private void Start()
        {
            subscription = healthVariable.Subscribe(UpdateHealth);
        }

        private void UpdateHealth(float value)
        {
            var scale = healthBar.localScale;
            scale.x = value / maxHealthVariable;
            healthBar.localScale = scale;
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}