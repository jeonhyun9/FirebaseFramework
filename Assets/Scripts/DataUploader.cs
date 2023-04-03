#if UNITY_EDITOR
using Firebase.Storage;
using System.Collections;
using System.IO;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

//Editor ������ ������ ���� �߻�!

/// <summary>
/// ���� ����� FireBaseStorage�� ���� ��Ģ�� �б�/���� ������ ��ο��� ���Ǿ� �ִ�.
/// ������ ����� ���� �� ���� ��Ģ�� �����ϰ�, ���ε� �� ���� �ܰ踦 �߰��ؾ��Ѵ�.
/// </summary>
public class DataUploader : MonoBehaviour
{
    private FirebaseStorage storage;
    private EditorCoroutine editorCoroutine;

    private bool setCurrentVersion;
    private string localJsonDataPath;

    private bool initialized = false;
    private FireBaseDefine fireBaseDef;

    private string JsonListTextName => Path.GetFileName(PathDefine.JsonListText);
    private string VersionTextName => Path.GetFileName(PathDefine.VersionText);
    private float progress;

    public bool Initialize(string localJsonPathValue, string bucketNameValue, bool setCurrentVersionValue)
    {
        localJsonDataPath = localJsonPathValue;
        string versionValue = File.ReadAllText(Path.Combine(localJsonDataPath, VersionTextName));

        if (string.IsNullOrEmpty(versionValue))
        {
            Debug.LogError("Version ������ ������ �� �����ϴ�.");
            return false;
        }

        fireBaseDef = new FireBaseDefine(bucketNameValue, versionValue);

        setCurrentVersion = setCurrentVersionValue;

        storage = FirebaseStorage.GetInstance(fireBaseDef.AppSpot);

        initialized = true;

        return true;
    }

    public void StartUpload()
    {
        if (!initialized)
        {
            Debug.LogError("DataUploader�� �ʱ�ȭ ���� ����.");
            return;
        }

        try
        {
            editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���ε� ���� : {e}");
        }
    }

    private IEnumerator UploadJsonDatas()
    {
        string[] jsonFiles = Directory.GetFiles(localJsonDataPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Debug.LogError("Json������ �����ϴ�.");
            yield break;
        }

        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} ���ε� ��..", progress);

            //���̾�̽� ��δ� Path.Combine X
            yield return UploadTask(jsonPath, fireBaseDef.JsonDatasPath + jsonName);
            progress += 1f / jsonFiles.Length;
        }

        EditorUtility.DisplayProgressBar("JsonList.txt", $"JsonList.txt ���ε� ��..", progress);

        yield return UploadTask(Path.Combine(localJsonDataPath, JsonListTextName), fireBaseDef.JsonListPath);

        if (setCurrentVersion)
        {
            EditorUtility.DisplayProgressBar("Version.txt", $"Version.txt ���ε� ��..", progress);
            yield return UploadTask(Path.Combine(localJsonDataPath, VersionTextName), fireBaseDef.CurrentVersionPath);
        }

        EditorUtility.ClearProgressBar();

        if (storage?.App != null)
            storage.App.Dispose();

        DestroyImmediate(gameObject);
    }

    private IEnumerator UploadTask(string localPath, string storagePath)
    {
        byte[] bytes = File.ReadAllBytes(localPath);
        StorageReference storageRef = storage.RootReference.Child(storagePath);
        Task<StorageMetadata> task = storageRef.PutBytesAsync(bytes);

        yield return new WaitUntil(() => task.IsCompleted);

        ShowTaskLog(Path.GetFileName(localPath), task);
    }

    private void ShowTaskLog(string fileName, Task<StorageMetadata> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("���ε� ���� : " + fileName + " - " + task.Exception);
        }
        else if (task.IsCompleted)
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
#endif
