using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Soar.Transactions
{
    [CustomEditor(typeof(Transaction), true)]
    [CanEditMultipleObjects]
    public class TransactionEditor : Editor
    {
        protected virtual string[] ExcludedProperties { get; } = { "m_Script" };

        private const float SpaceHeight = 15f;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AddCustomButtons();
        }

        protected void AddCustomButtons()
        {
            if (target is not Transaction transaction) return;
            
            GUILayout.Space(SpaceHeight);

            GUI.enabled = Application.isPlaying;

            if (!GUILayout.Button("Request")) return;

            Debug.Log($"{target.name} transaction requested.");
            transaction.Request(() => Debug.Log($"{target.name} transaction responded."));
        }
    }

    [CustomEditor(typeof(Transaction<>), true)]
    [CanEditMultipleObjects]
    public class ValueTransactionEditor : TransactionEditor
    {
        protected override string[] ExcludedProperties => base.ExcludedProperties
            .Concat(new[] {"requestValue", "responseValue"}).ToArray();
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawCustomProperties();
            AddCustomButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperty(string propertyName)
        {
            using var value = serializedObject.FindProperty(propertyName);
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
        }
        
        private void DrawCustomProperties()
        {
            DrawProperty("requestValue");
            DrawProperty("responseValue");
            
            DrawPropertiesExcluding(serializedObject, ExcludedProperties);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
