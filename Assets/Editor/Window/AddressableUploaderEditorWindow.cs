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
        private string Version => GetParameter<string>("Version");

        [MenuItem("Tools/Upload Addressable Build to FireBase Storage")]
        public static void OpenDataUploaderWindow()
        {
            AddressableUploaderEditorWindow window = (AddressableUploaderEditorWindow)GetWindow(typeof(AddressableUploaderEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            AddParameter("AddressableBuildPath", PathDefine.AddressableBuildPathByFlatform);
            AddParameter("BucketName", "jhgunity");
            AddParameter("Version", "0");
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Upload", GUILayout.Height(50)))
            {
                GameObject uploaderGo = new("AddressableUploader");
                FireBaseStorageUploader dataUploader = uploaderGo.AddComponent<FireBaseStorageUploader>();

                if (dataUploader == null)
                    return;

                if (dataUploader.Initialize(AddressableBuildPath, BucketName, Version))
                    dataUploader.StartAddressableBuildUpload();

                Close();
            }
        }
    }
}
#endif
