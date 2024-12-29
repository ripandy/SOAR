using System.Linq;
using UnityEditor;

namespace Soar.Collections
{
    [CustomEditor(typeof(SoarList<>), true)]
    [CanEditMultipleObjects]
    public class CollectionEditor : Editor
    {
        private readonly string[] excludedProperties = { "m_Script", "list" };
        private readonly string[] instanceSettings = { "valueEventType", "autoResetValue" };
        private const string InstanceSettingsLabel = "Instance Settings";
        private bool showInstanceSettings;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            using var value = serializedObject.FindProperty("list");
            if (value.propertyType == SerializedPropertyType.Generic && !value.isArray)
            {
                foreach (var child in value.GetChildren())
                {
                    EditorGUILayout.PropertyField(child, true);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(value, true);
            }

            var toExclude = excludedProperties.Concat(instanceSettings).ToArray();
            DrawPropertiesExcluding(serializedObject, toExclude);
            
            showInstanceSettings = EditorGUILayout.Foldout(showInstanceSettings, InstanceSettingsLabel);

            if (showInstanceSettings)
            {
                EditorGUI.indentLevel++;
                
                foreach (var settingName in instanceSettings)
                {
                    using var prop = serializedObject.FindProperty(settingName);
                    
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
