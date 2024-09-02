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

            GUI.enabled = Application.isPlaying;

            var e = (Command) target;
            if (GUILayout.Button("Execute"))
            {
                e.Execute();
            }
        }
    }
}