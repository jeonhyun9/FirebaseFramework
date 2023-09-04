#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Tools
{
    public class CurrentVersionUploaderEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 300f;
        private const float spacing = 5f;

        private string BucketName => GetParameter<string>("BucketName");
        private string JsonVersion => GetParameter<string>("JsonVersion");
        private string AddressableVersion => GetParameter<string>("AddressableVersion");

        [MenuItem("Tools/Current Version Upload to Firebase Storage")]
        public static void OpenDataUploaderWindow()
        {
            CurrentVersionUploaderEditorWindow window = (CurrentVersionUploaderEditorWindow)GetWindow(typeof(CurrentVersionUploaderEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            AddParameter("BucketName", NameDefine.BucketDefaultName);
            AddParameter("JsonVersion", "0.0.0");
            AddParameter("AddressableVersion", "0");
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Upload", GUILayout.Height(50)))
            {
                GameObject uploaderGo = new("DataUploader");
                FireBaseStorageUploader firebaseStorageUploader = uploaderGo.AddComponent<FireBaseStorageUploader>();

                if (firebaseStorageUploader == null)
                    return;

                if (firebaseStorageUploader.Initialize(null, new FireBaseStorage(BucketName, JsonVersion, AddressableVersion), true))
                    firebaseStorageUploader.StartUpload(FireBaseStorageUploader.Job.UploadCurrentVersion);

                Close();
            }
        }
    }
}
#endif
