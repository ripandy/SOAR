using Soar.Variables;
using UnityEngine;

namespace Soar.Samples.SettingsMenu
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "SOAR/Samples/Settings Menu/Game Settings Variable")]
    public class GameSettingsVariable : JsonableVariable<GameSettings>
    {
        private void Reset()
        {
            Value = new GameSettings
            {
                musicVolume = 0.8f,
                sfxVolume = 0.8f,
                showTutorials = true
            };
        }
    }
}
