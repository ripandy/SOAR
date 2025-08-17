using UnityEngine;
using UnityEngine.UI;

namespace Soar.Variables.Sample
{
    public class JsonUtilityHandler : MonoBehaviour
    {
        [SerializeField] private CustomVariable customVariable;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

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
            Debug.Log($"[{GetType().Name}] {nameof(customVariable)} saved as json named {customVariable.name}.json");
        }
        
        private void LoadButtonPressed()
        {
            customVariable.LoadFromJson();
            Debug.Log($"[{GetType().Name}] loaded {customVariable.name}.json into {nameof(customVariable)}");
        }
    }
}