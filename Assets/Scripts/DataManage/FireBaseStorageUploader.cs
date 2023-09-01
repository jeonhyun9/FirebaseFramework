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
    private bool isSetCurrentVersion;

    private FireBaseStorage fireBaseStorage;

    private float progress;
    private float progressIncrementValue;

    public bool Initialize(string localFilePathValue, FireBaseStorage fireBaseDefValue, bool isSetCurrentVersionValue)
    {
        localFilePath = localFilePathValue;
        fireBaseStorage = fireBaseDefValue;
        progress = 0f;
        isSetCurrentVersion = isSetCurrentVersionValue;

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

    public void StartCurrentVersionUpload()
    {
        if (!initialized)
        {
            Logger.Error("DataUploader not initialized");
            return;
        }

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(UploadCurrentVersion(), this);
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

        if (isSetCurrentVersion)
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

        if (isSetCurrentVersion)
            yield return UploadVersionText(fireBaseStorage.CurrentAddressableVersionStoragePath, fireBaseStorage.AddressableVersion);

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

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} ���ε� ��..", progress);

            yield return FireBaseUploadTask(fireBaseStorage.GetJsonStoragePath(jsonName), File.ReadAllBytes(jsonPath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadAddressable(string[] addressableFiles)
    {
        foreach (string addressablePath in addressableFiles)
        {
            string addressableName = Path.GetFileName(addressablePath);

            EditorUtility.DisplayProgressBar(addressableName, $"{addressableName} ���ε� ��..", progress);

            yield return FireBaseUploadTask(fireBaseStorage.GetAddressableBuildStoragePath(addressableName), File.ReadAllBytes(addressablePath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadJsonList(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar(NameDefine.JsonListTxtName, $"{NameDefine.JsonListTxtName} ���ε� ��..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, File.ReadAllBytes(localPath));
    }

    private IEnumerator UploadAddressableBuildInfo(string storagePath, AddressableBuildInfo addressableBuildInfo)
    {
        EditorUtility.DisplayProgressBar(NameDefine.AddressableBuildInfoName, $"{NameDefine.AddressableBuildInfoName} ���ε� ��..", progress += progressIncrementValue);

        string addressableBuildInfoJson = JsonConvert.SerializeObject(addressableBuildInfo);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(addressableBuildInfoJson));
    }
    
    private IEnumerator UploadVersionText(string storagePath, string version)
    {
        EditorUtility.DisplayProgressBar($"Version : {version}", $"Version ���ε� ��..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(version));

        Logger.Success($"Upload Set Current Version : {version}");
    }

    //storagePath�� Path.Combine ����ϸ� �ȵ�
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
            byte[] hash = File.ReadAllBytes(filePath).GetSHA256();
            string fileName = Path.GetFileName(filePath);

            fileNameWithHash.Add(fileName, hash);
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
