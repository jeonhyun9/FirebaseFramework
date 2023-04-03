#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DataGeneratorWindow : EditorWindow
    {
        private const float windowWidth = 400.0f;
        private const float windowHeight = 200.0f;

        private string excelPath;
        private string jsonPath;
        private int version;
        

        [MenuItem("Tools/Generate Data From Excel Folder")]
        public static void OpenDataGeneratorWindow()
        {
            DataGeneratorWindow window = (DataGeneratorWindow)GetWindow(typeof(DataGeneratorWindow));
            window.minSize = new Vector2(windowWidth, windowHeight);
            window.maxSize = new Vector2(windowWidth * 2, windowHeight);
        }

        private void OnEnable()
        {
            GetParameter();
        }

        public void OnGUI()
        {
            DrawParameter();
            DrawButton();
        }

        private void OnDisable()
        {
            SetParameter();
        }

        private void GetParameter()
        {
            excelPath = EditorPrefs.GetString("ExcelPath", PathDefine.Excel);
            jsonPath = EditorPrefs.GetString("JsonPath", PathDefine.Json);
            version = EditorPrefs.GetInt("Version", 0);
        }

        private void SetParameter()
        {
            EditorPrefs.SetString("ExcelPath", excelPath);
            EditorPrefs.SetString("jsonPath", jsonPath);
            EditorPrefs.SetInt("Version", version);
        }

        private void DrawParameter()
        {
            EditorGUILayout.LabelField("Local Excel Path");
            excelPath = EditorGUILayout.TextField(excelPath);

            EditorGUILayout.LabelField("Local Json Path");
            jsonPath = EditorGUILayout.TextField(jsonPath);

            EditorGUILayout.LabelField("Set Version");
            version = EditorGUILayout.IntField(version);

            GUILayout.Space(5);
        }

        private void DrawButton()
        {
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
                DataGenerator.GenerateDataFromExcelFoler(excelPath, jsonPath, version);
        }
    }
}
#endif
