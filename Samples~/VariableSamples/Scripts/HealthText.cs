using System;
using TMPro;
using UnityEngine;

namespace Soar.Variables.Sample
{
    public class HealthText : MonoBehaviour
    {
        [SerializeField] private FloatVariable healthVariable;
        [SerializeField] private FloatVariable maxHealthVariable;
        [SerializeField] private TMP_Text healthText;

        private IDisposable subscription;
        
        private void Start()
        {
            subscription = healthVariable.Subscribe(UpdateHealthText);
        }

        private void UpdateHealthText(float value)
        {
            healthText.text = $"{Mathf.FloorToInt(value)}/{maxHealthVariable.Value}";
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}