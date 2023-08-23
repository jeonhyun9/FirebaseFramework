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
    private FireBaseDefine fireBaseDef;

    private float progress;
    private float progressIncrementValue;

    public bool Initialize(string localFilePathValue, FireBaseDefine fireBaseDefValue)
    {
        localFilePath = localFilePathValue;
        fireBaseDef = fireBaseDefValue;
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

        yield return UploadJsonList(Path.Combine(localFilePath, NameDefine.JsonListTxtName), fireBaseDef.JsonListStoragePath);

        yield return UploadVersionText(fireBaseDef.CurrentJsonVersionStoragePath, fireBaseDef.JsonVersion);

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

        yield return UploadAddressableList(fireBaseDef.AddressableListStoragePath, CreateAddressableList(addressableBuildFiles, addressablePathFile));

        yield return UploadVersionText(fireBaseDef.CurrentAddressableVersionStoragePath, fireBaseDef.AddressableVersion);

        OnEndUpload();
    }

    private IEnumerator UploadJson(string[] jsonFiles)
    {
        foreach (string jsonPath in jsonFiles)
        {
            string jsonName = Path.GetFileName(jsonPath);

            EditorUtility.DisplayProgressBar(jsonName, $"{jsonName} 업로드 중..", progress);

            yield return FireBaseUploadTask(fireBaseDef.GetJsonStoragePath(jsonName), File.ReadAllBytes(jsonPath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadAddressable(string[] addressableFiles)
    {
        foreach (string addressablePath in addressableFiles)
        {
            string addressableName = Path.GetFileName(addressablePath);

            EditorUtility.DisplayProgressBar(addressableName, $"{addressableName} 업로드 중..", progress);

            yield return FireBaseUploadTask(fireBaseDef.GetAddressableBuildStoragePath(addressableName), File.ReadAllBytes(addressablePath));

            progress += progressIncrementValue;
        }
    }

    private IEnumerator UploadJsonList(string localPath, string storagePath)
    {
        EditorUtility.DisplayProgressBar("JsonList.txt", $"JsonList.txt 업로드 중..", progress += progressIncrementValue);

        yield return FireBaseUploadTask(storagePath, File.ReadAllBytes(localPath));
    }

    private IEnumerator UploadAddressableList(string storagePath, AddressableList addressableList)
    {
        EditorUtility.DisplayProgressBar("AddressableList.txt", $"AddressableList.txt 업로드 중..", progress += progressIncrementValue);

        string addressableListJson = JsonConvert.SerializeObject(addressableList);

        yield return FireBaseUploadTask(storagePath, Encoding.UTF8.GetBytes(addressableListJson));
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

    private AddressableList CreateAddressableList(string[] fileNameList, string addressablePathJson)
    {
        AddressableList addressableList = new AddressableList(fileNameList.ToList(),
            JsonConvert.DeserializeObject<Dictionary<Type, Dictionary<string, string>>>(addressablePathJson));

        return addressableList;
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
