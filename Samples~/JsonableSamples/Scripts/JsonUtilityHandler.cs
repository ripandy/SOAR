using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soar.Variables.Sample
{
    public class JsonUtilityHandler : MonoBehaviour
    {
        [SerializeField] private CustomVariable customVariable;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private TMP_Text statusText;

        private void OnEnable()
        {
            saveButton.onClick.AddListener(SaveButtonPressed);
            loadButton.onClick.AddListener(LoadButtonPressed);
        }
        
        private void OnDisable()
        {
            saveButton.onClick.RemoveListener(SaveButtonPressed);
            loadButton.onClick.RemoveListener(LoadButtonPressed);
        }

        private void SaveButtonPressed()
        {
            customVariable.SaveToJson();
            statusText.text = $"Saved {customVariable.name}.json";
        }
        
        private void LoadButtonPressed()
        {
            customVariable.LoadFromJson();
            statusText.text = $"Loaded {customVariable.name}.json";
        }
    }
}