using UnityEditor;
using UnityEngine;

namespace Soar.Variables
{
    [CustomEditor(typeof(JsonableVariable<>), true)]
    public class JsonableVariableEditor : VariableEditor
    {
        private bool showJsonFileOperation;
        private const string JsonOpLabel = "Json File Management";

        private readonly string[] jsonPathType = { "Data Path", "Persistent Data Path", "Custom" };
        private int selectedType;
        private bool defaultFilename = true;
        private string jsonPath;
        private string fileName;
        
        private void OnValidate()
        {
            showJsonFileOperation = false;
        }

        protected override void AddCustomButtons()
        {
            if (target is not IJsonable jsonable)
            {
                base.AddCustomButtons();
                return;
            }

            GUI.enabled = true;
            
            showJsonFileOperation = EditorGUILayout.Foldout(showJsonFileOperation, JsonOpLabel);

            if (showJsonFileOperation)
            {
                EditorGUI.indentLevel++;

                // Json path type dropdown
                selectedType = EditorGUILayout.Popup
                (
                    label: new GUIContent("Json Path Type"),
                    selectedIndex: selectedType,
                    displayedOptions: jsonPathType
                );

                jsonPath = selectedType switch
                {
                    2 => EditorGUILayout.TextField(jsonPath),
                    1 => Application.persistentDataPath,
                    _ => Application.dataPath
                };
                
                // Json file name input field
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Space(EditorGUI.indentLevel * 15);
                GUILayout.Label("File Name", GUILayout.ExpandWidth(false));
                
                GUI.enabled = !defaultFilename;
                
                fileName = EditorGUILayout.TextField(fileName);
                
                GUI.enabled = true;
                
                // Default file name toggle
                defaultFilename = EditorGUILayout.Toggle(defaultFilename, GUILayout.MaxWidth(30));
                GUILayout.Label("Default", GUILayout.ExpandWidth(false));
                
                if (defaultFilename)
                {
                    fileName = $"{target.name}.json";
                }
                
                EditorGUILayout.EndHorizontal();
                
                // Save and Load buttons
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(EditorGUI.indentLevel * 15);
                
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
                        serializedObject.Update();

                        jsonable.LoadFromJson(jsonPath, fileName);
                        Debug.Log($"Loaded {fileName} from {jsonPath}");
                        
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        Debug.Log($"Could not found file {jsonPath}/{fileName}");
                    }
                }

                GUILayout.EndHorizontal();
                
                EditorGUI.indentLevel--;
            }
            
            base.AddCustomButtons();
        }
    }
}