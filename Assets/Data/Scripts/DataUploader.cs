using Firebase.Storage;
using System.Collections;
using System.IO;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ���� ����� FireBaseStorage�� ���� ��Ģ�� �б�/���� ������ ��ο��� ���Ǿ� �ִ�.
/// ������ ����� ���� �� ���� ��Ģ�� �����ϰ�, ���ε� �� ���� �ܰ踦 �߰��ؾ��Ѵ�.
/// </summary>
public class DataUploader : MonoBehaviour
{
    private FirebaseStorage storage;
    private EditorCoroutine editorCoroutine;

    private string bucketName;
    private string version;
    private bool setCurrentVersion;
    private string localJsonDataPath;

    private bool initialized = false;

    private string AppSpot
    {
        get
        {
            return $"gs://{bucketName}.appspot.com";
        }
    }

    private string CurrentVersionPath
    {
        get
        {
            return "CurrentVersion/version.txt";
        }
    }
    
    private string JsonDatasPath
    {
        get
        {
            return $"JsonDatas/{version}/";
        }
    }

    public void Initialize(string localJsonPathValue, string bucketNameValue, string versionValue, bool setCurrentVersionValue)
    {
        localJsonDataPath = localJsonPathValue;
        bucketName = bucketNameValue;
        version = versionValue;
        setCurrentVersion = setCurrentVersionValue;

        storage = FirebaseStorage.GetInstance(AppSpot);

        initialized = true;
    }

    public void StartUpload()
    {
        if (!initialized)
        {
            Debug.LogError("DataUploader�� �ʱ�ȭ ���� ����.");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
    }

    private IEnumerator UploadJsonDatas()
    {
        if (setCurrentVersion)
        {
            byte[] versionBytes = Encoding.UTF8.GetBytes(version);
            StorageReference storageReference = storage.RootReference.Child(CurrentVersionPath);
            Task<StorageMetadata> task = storageReference.PutBytesAsync(versionBytes);
            yield return new WaitUntil(() => task.IsCompleted);

            ShowTaskLog("version.txt", ref task);
        }

        string[] jsonFiles = Directory.GetFiles(localJsonDataPath, "*.json");

        foreach (string filePath in jsonFiles)
        {
            string fileName = Path.GetFileName(filePath);

            byte[] fileBytes = File.ReadAllBytes(filePath);
            StorageReference storageReference = storage.RootReference.Child(JsonDatasPath + fileName);
            Task<StorageMetadata> task = storageReference.PutBytesAsync(fileBytes);
            yield return new WaitUntil(() => task.IsCompleted);

            ShowTaskLog(fileName, ref task);
        }

        storage.App.Dispose();
        DestroyImmediate(gameObject);
    }

    private void ShowTaskLog(string fileName, ref Task<StorageMetadata> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("���ε� ���� : " + fileName + " - " + task.Exception);
        }
        else
        {
            Debug.Log("���ε� �Ϸ� : " + fileName);
        }
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
