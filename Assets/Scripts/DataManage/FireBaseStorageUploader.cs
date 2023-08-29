#if UNITY_EDITOR
using Firebase.Storage;
using System.Collections;
using System.IO;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

//Editor 폴더에 넣으면 에러 발생!

/// <summary>
/// 현재 연결된 FireBaseStorage의 보안 규칙의 읽기/쓰기 권한이 모두에게 허용되어 있다.
/// 실제로 사용할 때는 꼭 보안 규칙을 수정하고, 업로드 전 인증 단계를 추가해야한다.
/// </summary>
public class FireBaseStorageUploader : MonoBehaviour
{
    private EditorCoroutine editorCoroutine;

    private string localFilePath;

    private bool initialized;
    private FireBaseStorage fireBaseStorage;

    private float progress;
    private float progressIncrementValue;

    public bool Initialize(string localFilePathValue, FireBaseStorage fireBaseDefValue)
    {
        localFilePath = localFilePathValue;
        fireBaseStorage = fireBaseDefValue;
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

        progressIncrementValue = 1f / (jsonFiles.Length + 2);

        yield return UploadJson(jsonFiles);

        yield return UploadJsonList(Path.Combine(localFilePath, NameDefine.JsonListTxtName), fireBaseStorage.JsonListStoragePath);

        yield return UploadVersionText(fireBaseStorage.CurrentJsonVersionStoragePath, fireBaseStorage.JsonVersion);

        OnEndUpload();
    }

    private IEnumerator UploadAddressableBuild()
    {
        string[] addressableBuildFiles = Directory.GetFiles(PathDefine.AddressableBuildPathByFlatform);
        string addressablePathFile = File.ReadAllText(PathDefine.AddressablePathJson);

        if (addressableBuildFiles.Length == 0)
        {
            Logger.Warning($"There is no addressable files : {localFilePath}");
            yield break;
        }

        progressIncrementValue = 1f / (addressableBuildFiles.Length + 2);

        yield return UploadAddressable(addressableBuildFiles);

        yield return UploadAddressableBuildInfo(fireBaseStorage.AddressableBuildInfoStoragePath, CreateAddressableBuildInfo(addressableBuildFiles, addressablePathFile));

        yield return UploadVersionText(fireBaseStorage.CurrentAddressableVersionStoragePath, fireBaseStorage.AddressableVersion);

        OnEndUpload();
    }

    private IEnumerator UploadJson(string[] jsonFiles)
    {
        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} 업로드 중..", progress);

            yield return FireBaseUploadTask(fireBaseStorage.GetJsonStoragePath(jsonName), File.ReadAllBytes(jsonPath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadAddressable(string[] addressableFiles)
    {
        foreach (string addressablePath in addressableFiles)
        {
            string addressableName = Path.GetFileName(addressablePath);

            EditorUtility.DisplayProgressBar(addressableName, $"{addressableName} 업로드 중..", progress);

            yield return FireBaseUploadTask(fireBaseStorage.GetAddressableBuildStoragePath(addressableName), File.ReadAllBytes(addressablePath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadJsonList(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar(NameDefine.JsonListTxtName, $"{NameDefine.JsonListTxtName} 업로드 중..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, File.ReadAllBytes(localPath));
    }

    private IEnumerator UploadAddressableBuildInfo(string storagePath, AddressableBuildInfo addressableBuildInfo)
    {
        yield return fireBaseStorage.GetDownloadUrl(storagePath);

        EditorUtility.DisplayProgressBar(NameDefine.AddressableBuildInfoName, $"{NameDefine.AddressableBuildInfoName} 업로드 중..", progress += progressIncrementValue);

        string addressableBuildInfoJson = JsonConvert.SerializeObject(addressableBuildInfo);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(addressableBuildInfoJson));
    }
    
    private IEnumerator UploadVersionText(string storagePath, string version)
    {
        EditorUtility.DisplayProgressBar("Version.txt", $"Version.txt 업로드 중..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(version));

        Logger.Success($"Upload Current Version : {version}");
    }

    //storagePath는 Path.Combine 사용하면 안됨
    private IEnumerator FireBaseUploadTask(string storagePath, byte[] bytes = null)
    {
        if (bytes == null)
            Logger.Error($"Upload Fail : {storagePath} - byte is null");

        StorageReference storageRef = fireBaseStorage.GetStoragePath(storagePath);
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

    private AddressableBuildInfo CreateAddressableBuildInfo(string[] addressableBuildFilesPath, string addressablePathJson)
    {
        Dictionary<string, byte[]> fileNameWithHash = new Dictionary<string, byte[]>();

        foreach(string filePath in addressableBuildFilesPath)
        {
            byte[] fileByte = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            fileNameWithHash.Add(fileName, fileByte);
        }

        AddressableBuildInfo addressableBuildInfo = new AddressableBuildInfo(fileNameWithHash,
            JsonConvert.DeserializeObject<Dictionary<Type, Dictionary<string, string>>>(addressablePathJson));

        return addressableBuildInfo;
    }

    private void OnEndUpload()
    {
        EditorUtility.ClearProgressBar();

        //fireBaseStorage.Dispose();

        DestroyImmediate(gameObject);
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
#endif
