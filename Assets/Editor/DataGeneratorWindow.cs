#if UNITY_EDITOR
using UnityEditor;

namespace Tools
{
    public class DataGeneratorWindow : EditorWindow
    {

        [MenuItem("Tools/Generate Data From Excel Folder")]
        public static void OpenGenerateDataWindow()
        {
            GetWindow(typeof(DataGeneratorWindow));
        }

        [MenuItem("Assets/Generate Data From Excel")]
        public static void GenerateDataFromExcel()
        {
            DataGenerator.GenerateDataFromExcel(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField($"엑셀 파일 경로 : {PathDefine.ExcelPath}");

            bool backupExcel = false;
            EditorGUILayout.Toggle("생성 후 엑셀파일 백업", backupExcel);
        }
    }
}
#endif
