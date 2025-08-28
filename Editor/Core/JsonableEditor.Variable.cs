using UnityEditor;

namespace Soar.Variables
{
    [CustomEditor(typeof(JsonableVariable<>), true)]
    public class JsonableVariableEditor : VariableEditor
    {
        private readonly JsonableEditor jsonableEditor = new();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (target is not IJsonable jsonable) return;
            jsonableEditor.DrawJsonFileManagementUI(jsonable, target);
        }
    }
}
