using UnityEngine;
using UnityEngine.EventSystems;

namespace Soar.Variables.Sample
{
    public class Healer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private FloatVariable healthVariable;
        [SerializeField] private FloatVariable maxHealthVariable;
        [SerializeField] private float healPerSecond;

        private bool hoverFlag;

        private void Update()
        {
            if (hoverFlag)
            {
                healthVariable.Value = Mathf.Clamp(healthVariable.Value + Time.deltaTime * healPerSecond, 0, maxHealthVariable);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverFlag = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverFlag = false;
        }
    }
}