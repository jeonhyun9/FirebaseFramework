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
        private string Version => GetParameter<string>("Version");

        [MenuItem("Tools/Upload Data to FireBase Storage")]
        public static void OpenDataUploaderWindow()
        {
            DataUploaderEditorWindow window = (DataUploaderEditorWindow)GetWindow(typeof(DataUploaderEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            AddParameter("JsonPath", PathDefine.Json);
            AddParameter("BucketName", NameDefine.BucketDefaultName);
            AddParameter("Version", "0.0.0");
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Upload", GUILayout.Height(50)))
            {
                GameObject uploaderGo = new("DataUploader");
                FireBaseStorageUploader firebaseStorageUploader = uploaderGo.AddComponent<FireBaseStorageUploader>();

                if (firebaseStorageUploader == null)
                    return;

                if (firebaseStorageUploader.Initialize(JsonPath, new FireBaseStorage(BucketName, Version)))
                    firebaseStorageUploader.StartJsonUpload();

                Close();
            }
        }
    }
}
#endif
