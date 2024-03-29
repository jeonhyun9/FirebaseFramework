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
using Cysharp.Threading.Tasks;

//Editor 폴더에 넣으면 에러 발생!

/// <summary>
/// 현재 연결된 FireBaseStorage의 보안 규칙의 읽기/쓰기 권한이 모두에게 허용되어 있다.
/// 실제로 사용할 때는 꼭 보안 규칙을 수정하고, 업로드 전 인증 단계를 추가해야한다.
/// </summary>
public class FireBaseStorageUploader : MonoBehaviour
{
    public enum Job
    {
        UploadData,
        UploadAddressable,
        UploadCurrentVersion,
    }

    private EditorCoroutine editorCoroutine;

    private string localFilePath;

    private bool initialized;
    private bool isSetCurrentVersion;

    private FireBaseStorage fireBaseStorage;

    private float progress;
    private float progressIncrementValue;

    private StringBuilder uploadHistory = new StringBuilder();

    public bool Initialize(string localFilePathValue, FireBaseStorage fireBaseDefValue, bool isSetCurrentVersionValue)
    {
        localFilePath = localFilePathValue;
        fireBaseStorage = fireBaseDefValue;
        progress = 0f;
        isSetCurrentVersion = isSetCurrentVersionValue;

        initialized = true;

        return true;
    }

    public void StartUpload(Job job)
    {
        if (!initialized)
        {
            Logger.Error("Uploader not initialized");
            return;
        }

        switch (job)
        {
            case Job.UploadData:
                editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadJsonDatas(), this);
                break;

            case Job.UploadAddressable:
                editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadAddressableBuild(), this);
                break;

            case Job.UploadCurrentVersion:
                editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadCurrentVersion(), this);
                break;

            default:
                break;
        }
    }

    private IEnumerator UploadJsonDatas()
    {
        string[] jsonFiles = Directory.GetFiles(localFilePath, "*.json");
        string[] jsonFileNames = jsonFiles.Select(x => Path.GetFileName(x)).ToArray();

        if (jsonFiles.Length == 0)
        {
            Logger.Warning($"There is no json files : {localFilePath}");
            yield break;
        }

        progressIncrementValue = 1f / (jsonFiles.Length + 2);

        yield return CleanStorage(fireBaseStorage.DataUploadHistoryStoargePath, jsonFileNames.Select(x => fireBaseStorage.GetJsonStoragePath(x)).ToArray());

        yield return UploadJson(jsonFiles);

        yield return UploadJsonList(Path.Combine(localFilePath, NameDefine.JsonListTxtName), fireBaseStorage.JsonListStoragePath);

        if (isSetCurrentVersion)
            yield return UploadVersionText(fireBaseStorage.CurrentJsonVersionStoragePath, fireBaseStorage.JsonVersion);

        yield return UploadHistory(fireBaseStorage.DataUploadHistoryStoargePath, uploadHistory.ToString());

        OnEndUpload();
    }

    private IEnumerator UploadAddressableBuild()
    {
        string[] addressableBuildFiles = Directory.GetFiles(PathDefine.AddressableBuildPathByPlatform);
        string[] addressableBuildFileNames = addressableBuildFiles.Select(x => Path.GetFileName(x)).ToArray();
        string addressablePathFile = File.ReadAllText(PathDefine.AddressablePathJson);

        if (addressableBuildFiles.Length == 0)
        {
            Logger.Warning($"There is no addressable files : {localFilePath}");
            yield break;
        }

        progressIncrementValue = 1f / (addressableBuildFiles.Length + 2);

        yield return CleanStorage(fireBaseStorage.AddressableUploadHistoryStoragePath, addressableBuildFileNames.Select(x => fireBaseStorage.GetAddressableBuildStoragePath(x)).ToArray());

        yield return UploadAddressable(addressableBuildFiles);

        yield return UploadAddressableBuildInfo(CreateAddressableBuildInfo(addressableBuildFiles, addressablePathFile));

        if (isSetCurrentVersion)
            yield return UploadVersionText(fireBaseStorage.CurrentAddressableVersionStoragePath, fireBaseStorage.AddressableVersion);

        yield return UploadHistory(fireBaseStorage.AddressableUploadHistoryStoragePath, uploadHistory.ToString());

        OnEndUpload();
    }

    private IEnumerator UploadCurrentVersion()
    {
        progressIncrementValue = 1f / 2;

        yield return UploadVersionText(fireBaseStorage.CurrentJsonVersionStoragePath, fireBaseStorage.JsonVersion);

        yield return UploadVersionText(fireBaseStorage.CurrentAddressableVersionStoragePath, fireBaseStorage.AddressableVersion);

        OnEndUpload();
    }

    private IEnumerator UploadJson(string[] jsonFiles)
    {
        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} 업로드 중..", progress);

            yield return FireBaseUploadTask(fireBaseStorage.GetJsonStoragePath(jsonName), File.ReadAllBytes(jsonPath), true);

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadAddressable(string[] addressableFiles)
    {
        foreach (string addressablePath in addressableFiles)
        {
            string addressableName = Path.GetFileName(addressablePath);

            EditorUtility.DisplayProgressBar(addressableName, $"{addressableName} 업로드 중..", progress);

            yield return FireBaseUploadTask(fireBaseStorage.GetAddressableBuildStoragePath(addressableName), File.ReadAllBytes(addressablePath), true);

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadJsonList(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar(NameDefine.JsonListTxtName, $"{NameDefine.JsonListTxtName} 업로드 중..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, File.ReadAllBytes(localPath));
    }

    private IEnumerator UploadAddressableBuildInfo(AddressableBuildInfo addressableBuildInfo)
    {
        EditorUtility.DisplayProgressBar(NameDefine.AddressableBuildInfoName, $"{NameDefine.AddressableBuildInfoName} 업로드 중..", progress += progressIncrementValue);

        string addressableBuildInfoJson = JsonConvert.SerializeObject(addressableBuildInfo);

        yield return FireBaseUploadTask(fireBaseStorage.AddressableBuildInfoStoragePath, Encoding.UTF8.GetBytes(addressableBuildInfoJson));
    }
    
    private IEnumerator UploadVersionText(string storagePath, string version)
    {
        EditorUtility.DisplayProgressBar($"Version : {version}", $"Version 업로드 중..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(version));

        Logger.Success($"Upload Set Current Version : {version}");
    }

    private IEnumerator UploadHistory(string storagePath, string history)
    {
        EditorUtility.DisplayProgressBar($"Upload History", $"UploadHistory 업로드 중..", progress);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(history));

        Logger.Success($"Upload History");
    }

    //storagePath는 Path.Combine 사용하면 안됨
    private IEnumerator FireBaseUploadTask(string storagePath, byte[] bytes = null, bool writeHistory = false)
    {
        if (bytes == null)
            Logger.Error($"Upload Fail : {storagePath} - byte is null");

        Task<StorageMetadata> task = fireBaseStorage.GetPutAsync(storagePath, bytes);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Logger.Error($"Upload Fail : {storagePath} - {task.Exception}");
        }
        else if (task.IsCompleted)
        {
            Logger.Success($"Upload Success : {storagePath}");

            if (writeHistory)
                uploadHistory.Append($"{storagePath},");
        }
    }

    private AddressableBuildInfo CreateAddressableBuildInfo(string[] addressableBuildFilesPath, string addressablePathJson)
    {
        Dictionary<string, byte[]> fileNameWithHash = new Dictionary<string, byte[]>();

        foreach(string filePath in addressableBuildFilesPath)
        {
            byte[] hash = File.ReadAllBytes(filePath).GetSHA256();
            string fileName = Path.GetFileName(filePath);

            fileNameWithHash.Add(fileName, hash);
        }

        AddressableBuildInfo addressableBuildInfo = new AddressableBuildInfo(fileNameWithHash,
            JsonConvert.DeserializeObject<Dictionary<Type, Dictionary<string, string>>>(addressablePathJson));

        return addressableBuildInfo;
    }

    private IEnumerator CleanStorage(string uploadHistoryPath, string[] namesToUpload)
    {
        Task<byte[]> task = fireBaseStorage.GetByteAsync(uploadHistoryPath);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && task.IsFaulted == false)
        {
            Logger.Success($"Load Success : {uploadHistoryPath}");

            string uploadHistoryJoin = task.Result.GetStringUTF8();

            string[] uploadHistories = uploadHistoryJoin.Split(",");

            foreach(string deletePath in uploadHistories)
            {
                if (string.IsNullOrEmpty(deletePath))
                    continue;

                if (namesToUpload.Contains(deletePath))
                    continue;

                Task deleteTask = fireBaseStorage.GetDeleteAsync(deletePath);

                yield return new WaitUntil(() => deleteTask.IsCompleted);

                if (deleteTask.IsCompleted)
                    Logger.Log($"Delete {deletePath}");
            }
        }
    }

    private void OnEndUpload()
    {
        EditorUtility.ClearProgressBar();

        DestroyImmediate(gameObject);
    }

    private void OnDestroy()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
    }
}
#endif
