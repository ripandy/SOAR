using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soar.Samples.SettingsMenu
{
    public class SettingsSaveLoad : MonoBehaviour
    {
        [SerializeField] private GameSettingsVariable settingsVariable;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private TMP_Text statusText;
        
        private void OnEnable()
        {
            saveButton.onClick.AddListener(SaveSettings);
            loadButton.onClick.AddListener(LoadSettings);
        }
        
        private void OnDisable()
        {
            saveButton.onClick.RemoveListener(SaveSettings);
            loadButton.onClick.RemoveListener(LoadSettings);
        }

        private void Start()
        {
            // Load settings on start to ensure the game begins with the correct state.
            LoadSettings();
        }

        private void SaveSettings()
        {
            settingsVariable.SaveToJson();
            statusText.text = $"Saved {settingsVariable.name}.json";
        }

        private void LoadSettings()
        {
            if (settingsVariable.IsJsonFileExist())
            {
                settingsVariable.LoadFromJson();
                statusText.text = $"Loaded {settingsVariable.name}.json";
            }
            else
            {
                statusText.text = "No settings file found. Loading default values.";
                // This ensures the UI is updated with the default values stored in the ScriptableObject asset.
                settingsVariable.Raise();
            }
        }
    }
}
