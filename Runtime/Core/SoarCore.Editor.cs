#if UNITY_EDITOR

using UnityEditor;

namespace Soar
{
    public abstract partial class SoarCore
    {
        private static bool IsDomainReloadDisabled => EditorSettings.enterPlayModeOptionsEnabled &&
                                                      EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
        
        private void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            // Only handle special case when user decides to disable domain reload.
            if (!IsDomainReloadDisabled) return;

            if (playModeState == PlayModeStateChange.ExitingEditMode)
            {
                Initialize(); // need to call Initialize due to override calls.
            }
            else if (playModeState == PlayModeStateChange.ExitingPlayMode)
            {
                OnQuit(); // need to call OnQuit due to override calls.
            }
        }
        
        private void SubscribeToEditorEvents()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting += UnsubscribeEditorEvents;
        }

        private void UnsubscribeEditorEvents()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.quitting -= UnsubscribeEditorEvents;
        }
    }
}

#endif
