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
public class FireBaseStorageUploader : MonoBehaviour
{
    private EditorCoroutine editorCoroutine;

    private string localFilePath;

    private bool initialized;
    private FireBaseDefine fireBaseDef;

    private float progress;
    private float progressIncrementValue;

    public bool Initialize(string localFilePathValue, string bucketNameValue, string versionValue)
    {
        localFilePath = localFilePathValue;
        fireBaseDef = new FireBaseDefine(bucketNameValue, versionValue);
        progress = 0f;

        initialized = true;

        return true;
    }

    public void StartJsonUpload()
    {
        if (!initialized)
        {
            Logger.Error("DataUploader not initialized");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
    }

    public void StartAddressableBuildUpload()
    {
        if (!initialized)
        {
            Logger.Error("DataUploader not initialized");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadAddressableBuild(), this);
    }

    private IEnumerator UploadJsonDatas()
    {
        string[] jsonFiles = Directory.GetFiles(localFilePath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Logger.Warning($"There is no json files : {localFilePath}");
            yield break;
        }

        progressIncrementValue = 1f / jsonFiles.Length + 2;

        yield return UploadJson(jsonFiles);

        yield return UploadJsonList(Path.Combine(localFilePath, NameDefine.JsonListTxtName), fireBaseDef.JsonListStoragePath);

        yield return UploadVersionText(fireBaseDef.CurrentJsonVersionStoragePath);

        OnEndUpload();
    }

    private IEnumerator UploadAddressableBuild()
    {
        string[] addressableBuildFiles = Directory.GetFiles(PathDefine.AddressableBuildPathByFlatform);

        if (addressableBuildFiles.Length == 0)
        {
            Logger.Warning($"There is no addressable files : {localFilePath}");
            yield break;
        }

        progressIncrementValue = 1f / addressableBuildFiles.Length + 1;

        yield return UploadAddressable(addressableBuildFiles);

        yield return UploadVersionText(fireBaseDef.CurrentJsonVersionStoragePath);

        OnEndUpload();
    }

    private IEnumerator UploadJson(string[] jsonFiles)
    {
        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} ���ε� ��..", progress);

            yield return FireBaseUploadTask(fireBaseDef.GetJsonStoragePath(jsonName), File.ReadAllBytes(jsonPath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadAddressable(string[] addressableFiles)
    {
        foreach (string addressablePath in addressableFiles)
        {
            string addressableName = Path.GetFileName(addressablePath);

            EditorUtility.DisplayProgressBar(addressableName, $"{addressableName} ���ε� ��..", progress);

            yield return FireBaseUploadTask(fireBaseDef.GetAddressableBuildStoragePath(addressableName), File.ReadAllBytes(addressablePath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadJsonList(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar("JsonList.txt", $"JsonList.txt ���ε� ��..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, File.ReadAllBytes(localPath));
    }
    
    private IEnumerator UploadVersionText(string storagePath)
    {
        EditorUtility.DisplayProgressBar("Version.txt", $"Version.txt ���ε� ��..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(fireBaseDef.Version));

        Logger.Success($"Upload Current Version : {fireBaseDef.Version}");
    }

    //storagePath�� Path.Combine ����ϸ� �ȵ�
    private IEnumerator FireBaseUploadTask(string storagePath, byte[] bytes = null)
    {
        if (bytes == null)
            Logger.Error($"Upload Fail : {storagePath} - byte is null");

        StorageReference storageRef = fireBaseDef.Storage.RootReference.Child(storagePath);
        Task<StorageMetadata> task = storageRef.PutBytesAsync(bytes);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Logger.Error($"Upload Fail : {storagePath} - {task.Exception}");
        }
        else if (task.IsCompleted)
        {
            Logger.Success($"Upload Success : {storagePath}");
        }
    }

    private void OnEndUpload()
    {
        EditorUtility.ClearProgressBar();

        if (fireBaseDef.Storage?.App != null)
            fireBaseDef.Storage.App.Dispose();

        DestroyImmediate(gameObject);
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
#endif
