#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DataUploaderWindow : EditorWindow
{
    private const float windowWidth = 400.0f;
    private const float windowHeight = 300.0f;

    private string bucketName;
    private int version;
    private string localJsonDataPath;
    private bool setCurrentVersion;

    [MenuItem("Tools/Upload Data to FireBase Storage")]
    public static void OpenDataUploaderWindow()
    {
        DataUploaderWindow window = (DataUploaderWindow)GetWindow(typeof(DataUploaderWindow));
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
        localJsonDataPath = EditorPrefs.GetString("LocalJsonDataPath", PathDefine.Json);
        bucketName = EditorPrefs.GetString("BucketName", "jhgunity");
        version = EditorPrefs.GetInt("Version", 0);
        setCurrentVersion = EditorPrefs.GetBool("SetCurrentVersion", false);
    }

    private void SetParameter()
    {
        EditorPrefs.SetString("LocalJsonDataPath", localJsonDataPath);
        EditorPrefs.SetString("BucketName", bucketName);
        EditorPrefs.SetInt("Version", version);
        EditorPrefs.SetBool("SetCurrentVersion", setCurrentVersion);
    }

    private void DrawParameter()
    {
        EditorGUILayout.LabelField("Local Json Data Path");
        localJsonDataPath = EditorGUILayout.TextField(localJsonDataPath);

        EditorGUILayout.LabelField("FireBase Bucket Name");
        bucketName = EditorGUILayout.TextField(bucketName);

        EditorGUILayout.LabelField("Version");
        version = EditorGUILayout.IntField(version);

        GUILayout.Space(5);
        
        EditorGUILayout.LabelField("Set Current Version");
        setCurrentVersion = EditorGUILayout.Toggle(setCurrentVersion);

        GUILayout.Space(5);
    }

    private void DrawButton()
    {
        if (GUILayout.Button("Upload", GUILayout.Height(50)))
        {
            DataUploader dataUploader = new GameObject().AddComponent<DataUploader>();
            dataUploader.Initialize(localJsonDataPath, bucketName, version.ToString(), setCurrentVersion);
            dataUploader.StartUpload();

            Close();
        }
    }
}
#endif
