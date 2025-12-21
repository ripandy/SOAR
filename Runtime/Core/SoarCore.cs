using System;
using UnityEngine;

namespace Soar
{
    public abstract partial class SoarCore : ScriptableObject, IDisposable
    {
        private void OnEnable()
        {
            // NOTE: Prevents instance reset (unload) when all references have been released.
            // Upon scene load, or upon calling UnloadUnusedAssets, if this instance has no reference, it would be replaced by other instance in another scene.
            // This would cause an unexpected behavior due to data loss by instance resets/replacements.
            hideFlags = HideFlags.DontUnloadUnusedAsset;
            
#if UNITY_EDITOR
            // NOTE : Unsubscribe then Subscribe ensures the subscription to the editor events only once.
            // Using flags does not work as expected when Domain Reload is disabled.
            // Reference : https://stackoverflow.com/a/7065833
            UnsubscribeEditorEvents();
            SubscribeToEditorEvents();
#endif
            Initialize();
        }

        internal virtual void Initialize()
        {
#if UNITY_EDITOR
            if (IsDomainReloadDisabled) return;
#endif
            Application.exitCancellationToken.Register(OnQuit);
        }

        internal virtual void OnQuit()
        {
#if UNITY_EDITOR
            OnQuitEditor();
            if (IsDomainReloadDisabled) return;
#endif
            // NOTE : Do not call Dispose when Domain Reload is disabled.
            // When Domain Reload is disabled, Disposed object doesn't get re-instantiated.
            Dispose(); 
        }

        public abstract void Dispose();
    }
}