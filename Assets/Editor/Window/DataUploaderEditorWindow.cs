#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Tools
{
    public class DataUploaderEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 300f;
        private const float spacing = 5f;

        private string BucketName => GetParameter<string>("BucketName");
        private string JsonPath => GetParameter<string>("JsonPath");
        private bool SetCurrentVersion => GetParameter<bool>("SetCurrentVersion");

        [MenuItem("Tools/Upload Data to FireBase Storage")]
        public static void OpenDataUploaderWindow()
        {
            DataUploaderEditorWindow window = (DataUploaderEditorWindow)GetWindow(typeof(DataUploaderEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            AddParameter("JsonPath", PathDefine.Json);
            AddParameter("BucketName", "jhgunity");
            AddParameter("SetCurrentVersion", false);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Upload", GUILayout.Height(50)))
            {
                GameObject uploaderGo = new("DataUploader");
                DataUploader dataUploader = uploaderGo.AddComponent<DataUploader>();

                if (dataUploader == null)
                    return;

                if (dataUploader.Initialize(JsonPath, BucketName, SetCurrentVersion))
                    dataUploader.StartUpload();

                Close();
            }
        }
    }
}
#endif