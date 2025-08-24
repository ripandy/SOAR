using System;
using UnityEngine;
using UnityEngine.UI;

namespace Soar.Samples.SettingsMenu
{
    public class SettingsUIManager : MonoBehaviour
    {
        [Header("SOAR Assets")]
        [SerializeField] private GameSettingsVariable settingsVariable;

        [Header("UI Elements")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle tutorialToggle;

        private IDisposable subscription;

        private void OnEnable()
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            tutorialToggle.onValueChanged.AddListener(OnTutorialsToggleChanged);
        }

        private void OnDisable()
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            tutorialToggle.onValueChanged.RemoveListener(OnTutorialsToggleChanged);
        }

        private void Start()
        {
            UpdateUI(settingsVariable.Value);
            subscription = settingsVariable.Subscribe(UpdateUI);
        }

        private void OnMusicVolumeChanged(float value)
        {
            var settings = settingsVariable.Value;
            settings.musicVolume = value;
            settingsVariable.Value = settings;
        }

        private void OnSfxVolumeChanged(float value)
        {
            var settings = settingsVariable.Value;
            settings.sfxVolume = value;
            settingsVariable.Value = settings;
        }

        private void OnTutorialsToggleChanged(bool value)
        {
            var settings = settingsVariable.Value;
            settings.showTutorials = value;
            settingsVariable.Value = settings;
        }

        private void UpdateUI(GameSettings newSettings)
        {
            musicSlider.SetValueWithoutNotify(newSettings.musicVolume);
            sfxSlider.SetValueWithoutNotify(newSettings.sfxVolume);
            tutorialToggle.SetIsOnWithoutNotify(newSettings.showTutorials);
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}
