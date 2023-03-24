#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DataGeneratorWindow : EditorWindow
    {
        private const float windowWidth = 400.0f;
        private const float windowHeight = 100.0f;

        private string excelPath;

        [MenuItem("Tools/Generate Data From Excel Folder")]
        public static void OpenGenerateDataWindow()
        {
            DataGeneratorWindow window = (DataGeneratorWindow)GetWindow(typeof(DataGeneratorWindow));
            window.minSize = new Vector2(windowWidth, windowHeight);
            window.maxSize = new Vector2(windowWidth * 2, windowHeight);
        }

        [MenuItem("Assets/Generate Data From Excel")]
        public static void GenerateDataFromExcel()
        {
            DataGenerator.GenerateDataFromExcel(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        public void OnGUI()
        {
            excelPath = EditorGUILayout.TextField(excelPath);

            GUILayout.Space(10);

            if (GUILayout.Button("Generate", GUILayout.Height(50)))
                DataGenerator.GenerateDataFromExcelFoler(excelPath);
        }

        private void OnEnable()
        {
            excelPath = EditorPrefs.GetString("ExcelPath");

            if (string.IsNullOrEmpty(excelPath))
                excelPath = PathDefine.Excel;
        }

        private void OnDisable()
        {
            EditorPrefs.SetString("ExcelPath", excelPath);
        }
    }
}
#endif
