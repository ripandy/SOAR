using UnityEditor;
using UnityEngine;

namespace Soar.Events
{
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class GameEventEditor : Editor
    {
        private const float SpaceHeight = 15f;
        
        protected virtual bool CanRaise => true;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AddCustomButtons();
        }

        protected virtual void AddCustomButtons()
        {
            if (target is not GameEvent gameEvent) return;
            
            GUILayout.Space(SpaceHeight);

            const string raiseLabel = "Raise";
            using (new EditorGUI.DisabledScope(!CanRaise))
            {
                if (!GUILayout.Button(raiseLabel)) return;
            }

            gameEvent.Raise();
            Debug.Log($"[{gameEvent.GetType().Name}:{gameEvent.name}] event raised{(Application.isPlaying ? "." : " in Edit Mode. Note that some event listeners may not be invoked in editor mode.")}");
            
            if (Application.isPlaying) return;
            
            // Mark the object as dirty in Edit mode to ensure changes get saved
            EditorUtility.SetDirty(gameEvent);
        }
    }
    
    [CustomEditor(typeof(GameEvent<>), true)]
    [CanEditMultipleObjects]
    public class TypedGameEventEditor : GameEventEditor
    {
        private const string ValuePropertyName = "value";

        protected virtual string[] ExcludedProperties { get; } = { "m_Script", ValuePropertyName };

        // MEMO: Unity does not serialize the value field when its type is not serializable,
        //       e.g. a class without [Serializable] attribute, an interface or a Dictionary.
        private bool isValueSerializable = true;

        protected override bool CanRaise => isValueSerializable;

        private string NonSerializableMessage =>
            $"Value of type '{ValueTypeName}' is not serializable. It cannot be shown in the Inspector and the Raise button is disabled. " +
            "The event still works when raised from code. To show it, mark the type with [Serializable] or use a type Unity can serialize.";

        private string ValueTypeName
        {
            get
            {
                for (var type = target.GetType(); type != null; type = type.BaseType)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GameEvent<>))
                    {
                        return type.GetGenericArguments()[0].Name;
                    }
                }
                return ValuePropertyName;
            }
        }

        protected virtual void OnEnable()
        {
            if (target == null) return;

            using var value = serializedObject.FindProperty(ValuePropertyName);
            isValueSerializable = value != null;

            if (isValueSerializable) return;
            Debug.LogError($"[{target.GetType().Name}:{target.name}] {NonSerializableMessage}", target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawCustomProperties();
            AddCustomButtons();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCustomProperties()
        {
            using var value = serializedObject.FindProperty(ValuePropertyName);
            if (value == null)
            {
                EditorGUILayout.HelpBox(NonSerializableMessage, MessageType.Error);
            }
            else if (value.propertyType == SerializedPropertyType.Generic && !value.isArray)
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

            DrawPropertiesExcluding(serializedObject, ExcludedProperties);
        }
    }
}
