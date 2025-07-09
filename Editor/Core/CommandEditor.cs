using UnityEditor;
using UnityEngine;

namespace Soar.Commands
{
    [CustomEditor(typeof(Command), editorForChildClasses: true)]
    public class CommandEditor : Editor
    {
        private const float SpaceHeight = 15f;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (target is not Command command) return;
            
            GUILayout.Space(SpaceHeight);

            const string executeLabel = "Execute";
            if (!GUILayout.Button(executeLabel)) return;
            
            command.Execute();
            Debug.Log($"[{command.GetType().Name}:{command.name}] executed{(Application.isPlaying ? "." : " in Edit Mode. Note that some execution may not run properly in editor mode.")}");
            
            if (Application.isPlaying) return;
            
            // Mark the object as dirty in Edit mode to ensure changes get saved
            EditorUtility.SetDirty(command);
        }
    }
}