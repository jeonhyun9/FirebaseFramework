#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class DataUploaderEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 300f;
        private const float spacing = 5f;

        private string BucketName => GetParameter<string>("BucketName");
        private string JsonPath => GetParameter<string>("JsonPath");
        private string JsonVersion => GetParameter<string>("JsonVersion");
        private bool IsSetCurrentVersion => GetParameter<bool>("IsSetCurrentVersion");

        [MenuItem("Tools/Data/Upload Data to FireBase Storage")]
        public static void OpenDataUploaderWindow()
        {
            DataUploaderEditorWindow window = (DataUploaderEditorWindow)GetWindow(typeof(DataUploaderEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            EditorPrefs.SetBool("isSetCurrentVersion", false);

            AddParameter("JsonPath", PathDefine.Json);
            AddParameter("BucketName", NameDefine.BucketDefaultName);
            AddParameter("JsonVersion", "0.0.0");
            AddParameter("isSetAddressableVersion", false);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Upload", GUILayout.Height(50)))
            {
                GameObject uploaderGo = new("DataUploader");
                FireBaseStorageUploader firebaseStorageUploader = uploaderGo.AddComponent<FireBaseStorageUploader>();

                if (firebaseStorageUploader == null)
                    return;

                if (firebaseStorageUploader.Initialize(JsonPath, new FireBaseStorage(BucketName, JsonVersion), IsSetCurrentVersion))
                    firebaseStorageUploader.StartUpload(FireBaseStorageUploader.Job.UploadData);

                Close();
            }
        }
    }
}
#endif
