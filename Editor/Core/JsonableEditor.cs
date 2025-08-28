using UnityEditor;
using UnityEngine;

namespace Soar
{
    public class JsonableEditor
    {
        private bool showJsonFileOperation;
        private const string JsonOpLabel = "Json File Management";

        private readonly string[] jsonPathType = { "Data Path", "Persistent Data Path", "Custom" };
        private int selectedType;
        private bool defaultFilename = true;
        private string jsonPath;
        private string fileName;

        public void DrawJsonFileManagementUI(IJsonable jsonable, Object targetObject)
        {
            EditorGUILayout.Space();
            showJsonFileOperation = EditorGUILayout.Foldout(showJsonFileOperation, JsonOpLabel, true);

            if (!showJsonFileOperation) return;
            
            EditorGUI.indentLevel++;

            selectedType = EditorGUILayout.Popup("Json Path Type", selectedType, jsonPathType);

            jsonPath = selectedType switch
            {
                2 => EditorGUILayout.TextField("Custom Path", jsonPath),
                1 => Application.persistentDataPath,
                _ => Application.dataPath
            };

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("File Name", GUILayout.ExpandWidth(false));
            GUI.enabled = !defaultFilename;
            fileName = EditorGUILayout.TextField(fileName);
            GUI.enabled = true;
            defaultFilename = EditorGUILayout.Toggle(defaultFilename, GUILayout.MaxWidth(15));
            if (defaultFilename)
            {
                fileName = targetObject.name + ".json";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save to Json"))
            {
                jsonable.SaveToJson(jsonPath, fileName);
                Debug.Log($"Saved {fileName} to {jsonPath}");
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Load from Json"))
            {
                if (jsonable.IsJsonFileExist(jsonPath, fileName))
                {
                    Undo.RecordObject(targetObject, "Load from Json");
                    jsonable.LoadFromJson(jsonPath, fileName);
                    Debug.Log($"Loaded {fileName} from {jsonPath}");
                    EditorUtility.SetDirty(targetObject);
                }
                else
                {
                    Debug.LogError($"Could not find file {jsonPath}/{fileName}");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }
}
