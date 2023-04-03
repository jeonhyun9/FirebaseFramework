using UnityEngine;
using UnityEditor;
using System.IO;

public class DataUploaderWindow : EditorWindow
{
    private const float windowWidth = 400.0f;
    private const float windowHeight = 300.0f;

    private string bucketName;
    private int version;
    private string jsonPath;
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
        jsonPath = EditorPrefs.GetString("jsonPath", PathDefine.Json);
        bucketName = EditorPrefs.GetString("BucketName", "jhgunity");
        setCurrentVersion = EditorPrefs.GetBool("SetCurrentVersion", false);
    }

    private void SetParameter()
    {
        EditorPrefs.SetString("jsonPath", jsonPath);
        EditorPrefs.SetString("BucketName", bucketName);
        EditorPrefs.SetBool("SetCurrentVersion", setCurrentVersion);
    }

    private void DrawParameter()
    {
        EditorGUILayout.LabelField("Local Json Path");
        jsonPath = EditorGUILayout.TextField(jsonPath);

        EditorGUILayout.LabelField("FireBase Bucket Name");
        bucketName = EditorGUILayout.TextField(bucketName);

        GUILayout.Space(5);
        
        EditorGUILayout.LabelField("Set Current Version");
        setCurrentVersion = EditorGUILayout.Toggle(setCurrentVersion);

        GUILayout.Space(5);
    }

    private void DrawButton()
    {
        if (GUILayout.Button("Upload", GUILayout.Height(50)))
        {
            DataUploader dataUploader = new GameObject("DataUploader").AddComponent<DataUploader>();

            if (dataUploader.Initialize(jsonPath, bucketName, setCurrentVersion))
                dataUploader.StartUpload();

            Close();
        }
    }
}
