using UnityEditor;
using UnityEngine;

namespace Soar.Commands
{
    [CustomEditor(typeof(Command), editorForChildClasses: true)]
    public class CommandEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (target is not Command command) return;

            if (!GUILayout.Button("Execute")) return;
            
            command.Execute();
            
            Debug.Log($"{command.name} executed{(Application.isPlaying ? "." : " in Edit Mode. Note that some execution may not run properly in editor mode.")}");
            
            // Mark the object as dirty in Edit mode to ensure changes get saved
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(command);
            }
        }
    }
}