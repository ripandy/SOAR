using UnityEditor;

namespace Soar.Collections
{
    [CustomEditor(typeof(JsonableList<>), true)]
    public class JsonableListEditor : CollectionEditor
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
