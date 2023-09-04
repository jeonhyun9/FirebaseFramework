#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Tools
{
    public class AddressableUploaderEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 300f;
        private const float spacing = 5f;

        private string BucketName => GetParameter<string>("BucketName");
        private string AddressableBuildPath => GetParameter<string>("AddressableBuildPath");
        private string AddressableVersion => GetParameter<string>("AddressableVersion");
        private bool IsSetCurrentVersion => GetParameter<bool>("IsSetAddressableVersion");

        [MenuItem("Tools/Addressable/Upload Addressable Build to FireBase Storage")]
        public static void OpenDataUploaderWindow()
        {
            AddressableUploaderEditorWindow window = (AddressableUploaderEditorWindow)GetWindow(typeof(AddressableUploaderEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            EditorPrefs.SetBool("isSetCurrentVersion", false);

            AddParameter("AddressableBuildPath", PathDefine.AddressableBuildPathByPlatform);
            AddParameter("BucketName", NameDefine.BucketDefaultName);
            AddParameter("AddressableVersion", "0");
            AddParameter("IsSetAddressableVersion", false);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Upload", GUILayout.Height(50)))
            {
                GameObject uploaderGo = new("AddressableUploader");
                FireBaseStorageUploader firebaseUploader = uploaderGo.AddComponent<FireBaseStorageUploader>();

                if (firebaseUploader == null)
                    return;

                if (firebaseUploader.Initialize(AddressableBuildPath, new FireBaseStorage(BucketName, addressableVersion: AddressableVersion), IsSetCurrentVersion))
                    firebaseUploader.StartUpload(FireBaseStorageUploader.Job.UploadAddressable);

                Close();
            }
        }
    }
}
#endif
