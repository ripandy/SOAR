using System.Linq;
using Soar.Events;
using UnityEditor;

namespace Soar.Variables
{
    [CustomEditor(typeof(VariableCore<>), true)]
    [CanEditMultipleObjects]
    public class VariableEditor : TypedGameEventEditor
    {
        private readonly string[] instanceSettings = { "valueEventType", "autoResetValue" };
        private const string InstanceSettingsLabel = "Instance Settings";
        private bool showInstanceSettings;

        protected override string[] ExcludedProperties => base.ExcludedProperties.Concat(instanceSettings).ToArray();

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
