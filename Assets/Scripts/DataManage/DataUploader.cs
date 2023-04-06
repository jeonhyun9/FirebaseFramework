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

    private bool initialized;
    private FireBaseDefine fireBaseDef;

    private float progress;
    private float progressIncrementValue;

    private string JsonListTextName => Path.GetFileName(PathDefine.JsonListText);
    private string VersionTextName => Path.GetFileName(PathDefine.VersionText);

    public bool Initialize(string localJsonPathValue, string bucketNameValue, bool setCurrentVersionValue)
    {
        localJsonDataPath = localJsonPathValue;
        string versionValue = File.ReadAllText(Path.Combine(localJsonDataPath, VersionTextName));

        if (string.IsNullOrEmpty(versionValue))
        {
            Logger.Error("Failed to load Version.txt");
            return false;
        }

        fireBaseDef = new FireBaseDefine(bucketNameValue, versionValue);
        setCurrentVersion = setCurrentVersionValue;
        storage = FirebaseStorage.GetInstance(fireBaseDef.AppSpot);
        progress = 0f;

        initialized = true;

        return true;
    }

    public void StartUpload()
    {
        if (!initialized)
        {
            Logger.Error("DataUploader not initialized");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
    }

    private IEnumerator UploadJsonDatas()
    {
        string[] jsonFiles = Directory.GetFiles(localJsonDataPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Logger.Warning($"There is no json files : {localJsonDataPath}");
            yield break;
        }

        progressIncrementValue = 1f / (setCurrentVersion ? jsonFiles.Length + 2 : jsonFiles.Length + 1);

        yield return UploadJson(jsonFiles);

        yield return UploadJsonList(Path.Combine(localJsonDataPath, JsonListTextName), fireBaseDef.JsonListPath);

        if (setCurrentVersion)
            yield return UploadVersionText(Path.Combine(localJsonDataPath, VersionTextName), fireBaseDef.CurrentVersionPath);

        OnEndUpload();
    }

    private IEnumerator UploadJson(string[] jsonFiles)
    {
        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} ���ε� ��..", progress);

            yield return FireBaseUploadTask(jsonPath, fireBaseDef.GetJsonPath(jsonName));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadJsonList(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar("JsonList.txt", $"JsonList.txt ���ε� ��..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(localPath, storagePath);
    }
    
    private IEnumerator UploadVersionText(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar("Version.txt", $"Version.txt ���ε� ��..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(localPath, storagePath);

        Logger.Success($"Upload Current Version : {fireBaseDef.Version}");
    }

    //storagePath�� Path.Combine ����ϸ� �ȵ�
    private IEnumerator FireBaseUploadTask(string localPath, string storagePath)
    {
        byte[] bytes = File.ReadAllBytes(localPath);
        StorageReference storageRef = storage.RootReference.Child(storagePath);
        Task<StorageMetadata> task = storageRef.PutBytesAsync(bytes);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Logger.Error($"Upload Fail : {Path.GetFileName(localPath)} - {task.Exception}");
        }
        else if (task.IsCompleted)
        {
            Logger.Success($"Upload Success : {Path.GetFileName(localPath)}");
        }
    }

    private void OnEndUpload()
    {
        EditorUtility.ClearProgressBar();

        if (storage?.App != null)
            storage.App.Dispose();

        DestroyImmediate(gameObject);
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
#endif
