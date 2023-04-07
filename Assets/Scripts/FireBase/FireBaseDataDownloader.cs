using Cysharp.Threading.Tasks;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

public class FireBaseDataDownloader
{
    private readonly Dictionary<string, string> dicJsonByFileName = new ();

    private FireBaseDefine fireBaseDef;
    private readonly FirebaseStorage storage;

    public string Version => fireBaseDef.Version;

    private string JsonSavePath => Application.persistentDataPath;

    private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

    public FireBaseDataDownloader(string bucketName)
    {
        fireBaseDef = new FireBaseDefine(bucketName);
        storage = FirebaseStorage.GetInstance(fireBaseDef.AppSpot);
    }

    public async UniTask<Dictionary<string,string>> LoadDataDicFromFireBase()
    {
        dicJsonByFileName.Clear();

        if (!await LoadFireBaseDefVersion())
            return null;
        
        string[] jsonList = await LoadJsonList(fireBaseDef.JsonListPath);

        if (!jsonList.IsValidArray())
        {
            Logger.Error("jsonList is null or empty");
            return null;
        }

        UniTask[] tasks = jsonList.Select(async jsonName =>
        {
            string fileName = Path.GetFileName(jsonName);
            string localPath = Path.Combine(JsonSavePath, fileName);

            StorageReference jsonDataRef = storage.RootReference.Child(fireBaseDef.GetJsonPath(jsonName));

            byte[] loadedBytes = await LoadBytes(jsonDataRef);

            //불러온 데이터 예외처리
            if (loadedBytes.IsValidArray())
            {
                string loadedString = loadedBytes.GetStringUTF8();

                if (!string.IsNullOrEmpty(loadedString))
                {
                    // 컨테이너에 담길 데이터 추가
                    dicJsonByFileName.Add(fileName, loadedString);
                    Logger.Success($"Load Json From FireBase : {fileName}");
                }
            }
            else
            {
                Logger.Error($"Invalid load json {fileName}");
            }
        }).ToArray();

        await UniTask.WhenAll(tasks);

        if (storage?.App != null)
            storage.App.Dispose();

        return dicJsonByFileName;
    }

    private async UniTask<bool> LoadFireBaseDefVersion()
    {
        StorageReference versionRef = storage.RootReference.Child(fireBaseDef.CurrentVersionPath);

        string currentVersion = await LoadString(versionRef);

        if (!string.IsNullOrEmpty(currentVersion))
        {
            fireBaseDef.SetVersion(currentVersion);
            return true;
        }

        return false;
    }

    private async UniTask<string[]> LoadJsonList(string refPath)
    {
        StorageReference storageRef = storage.RootReference.Child(refPath);

        string[] jsonListArray;

        try
        {
            byte[] jsonListBytes = await storageRef.GetBytesAsync(fireBaseDef.MaxJsonSizeBytes);
            jsonListArray = jsonListBytes.GetStringUTF8()?.Split(",");

            if (!jsonListArray.IsValidArray())
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return jsonListArray;
    }
    private async UniTask<string> LoadString(StorageReference storageRef)
    {
        string stringValue;

        try
        {
            byte[] loadedBytes = await storageRef.GetBytesAsync(fireBaseDef.MaxJsonSizeBytes);
            stringValue = loadedBytes.GetStringUTF8();

            if (string.IsNullOrEmpty(stringValue))
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return stringValue;
    }

    private async UniTask<byte[]> LoadBytes(StorageReference storageRef)
    {
        byte[] loadedBytes;

        try
        {
            loadedBytes = await storageRef.GetBytesAsync(fireBaseDef.MaxJsonSizeBytes);

            if (!loadedBytes.IsValidArray())
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return loadedBytes;
    }
}
