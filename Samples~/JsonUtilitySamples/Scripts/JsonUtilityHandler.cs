using Soar.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace Soar.Json.Sample
{
    public class JsonUtilityHandler : MonoBehaviour
    {
        [SerializeField] private CustomVariable _customVariable;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        private void OnEnable()
        {
            _saveButton.onClick.AddListener(SaveButtonPressed);
            _loadButton.onClick.AddListener(LoadButtonPressed);
        }
        
        private void OnDisable()
        {
            _saveButton.onClick.RemoveListener(SaveButtonPressed);
            _loadButton.onClick.RemoveListener(LoadButtonPressed);
        }

        private void SaveButtonPressed()
        {
            _customVariable.SaveToJson(Application.persistentDataPath, $"{_customVariable.name}.json");
            
            // uncomment to try the experimental non generic handling.
            // (_customVariable as IVariable).SaveToJson();
            
            Debug.Log($"[{GetType().Name}] {nameof(_customVariable)} saved as json named {_customVariable.name}.json");
        }
        
        private void LoadButtonPressed()
        {
            _customVariable.LoadFromJson(Application.persistentDataPath, $"{_customVariable.name}.json");
            
            // uncomment to try the experimental non generic handling.
            // (_customVariable as IVariable).LoadFromJson();
            
            Debug.Log($"[{GetType().Name}] loaded {_customVariable.name}.json into {nameof(_customVariable)}");
        }
    }
}