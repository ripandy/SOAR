using System.Linq;
using Soar.Events;
using UnityEditor;
using UnityEngine;

namespace Soar.Transactions
{
    [CustomEditor(typeof(Transaction<>), true)]
    [CanEditMultipleObjects]
    public class TransactionEditor : TypedGameEventEditor
    {
        private readonly string[] instanceSettings = { "registerResponseInternally" };
        private const string InstanceSettingsLabel = "Instance Settings";
        private bool showInstanceSettings;

        protected override string[] ExcludedProperties => base.ExcludedProperties.Concat(instanceSettings).ToArray();

        protected override void AddCustomButtons()
        {
            if (target is not GameEvent gameEvent) return;
            
            GUILayout.Space(SpaceHeight);

            GUI.enabled = Application.isPlaying;

            if (!GUILayout.Button("Request")) return;
            
            gameEvent.Raise();
            
            Debug.Log($"{target.name} Request sent.");
        }
        
        protected override void DrawCustomProperties()
        {
            base.DrawCustomProperties();
            
            showInstanceSettings = EditorGUILayout.Foldout(showInstanceSettings, InstanceSettingsLabel);

            if (showInstanceSettings)
            {
                EditorGUI.indentLevel++;
                
                foreach (var settingName in instanceSettings)
                {
                    using var prop = serializedObject.FindProperty(settingName);
                    if (prop == null) continue;
                    EditorGUILayout.PropertyField(prop);
                }
                
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
