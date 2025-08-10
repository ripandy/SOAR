using UnityEngine;
using UnityEngine.UI;

namespace Soar.Variables.Sample
{
    public class HealthUpgrade : MonoBehaviour
    {
        [SerializeField] private FloatVariable healthVariable;
        [SerializeField] private FloatVariable maxHealthVariable;
        [SerializeField] private float upgradeValue = 50;
        [SerializeField] private Button button;

        private void Start()
        {
            button.onClick.AddListener(UpgradeMaxHealth);
        }

        private void UpgradeMaxHealth()
        {
            var upgradedVal = maxHealthVariable.Value + upgradeValue;
            if (upgradedVal < Mathf.Abs(upgradeValue))
                return;
            
            var prevVal = maxHealthVariable.Value;
            maxHealthVariable.Value = upgradedVal;
            healthVariable.Value *= maxHealthVariable / prevVal;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(UpgradeMaxHealth);
        }
    }
}