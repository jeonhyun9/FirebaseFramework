using Cysharp.Threading.Tasks;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public class FireBaseDataDownloader
{
    private readonly Dictionary<string, string> dicJsonByFileName = new ();

    private FireBaseDefine fireBaseDef;
    private readonly FirebaseStorage storage;

    public string Version => fireBaseDef.Version;

    private string JsonSavePath => Application.persistentDataPath;

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

        if (jsonList.Length == 0)
            return null;

        foreach (string jsonName in jsonList)
        {
            string fileName = Path.GetFileName(jsonName);
            string localPath = Path.Combine(JsonSavePath, fileName);

            StorageReference jsonDataRef = storage.RootReference.Child(fireBaseDef.GetJsonPath(jsonName));

            byte[] jsonData = await LoadBytes(jsonDataRef);

            //불러온 데이터 예외처리
            if (!IsValidByteArray(jsonData))
                continue;

            byte[] localJsonData = await LoadLocalBytes(localPath);

            //로컬 데이터와 무결성 검사하고, 다르면 불러온 데이터로 덮어씌움
            if (!IntegrityCheck(localJsonData, jsonData))
                await SaveDataToLocalPath(localPath, jsonData);
            
            //컨테이너에 담길 데이터 추가
            dicJsonByFileName.Add(fileName, Encoding.UTF8.GetString(jsonData));
            Logger.Success($"Load Json From FireBase : {fileName}");
        }

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
            jsonListArray = Encoding.UTF8.GetString(jsonListBytes).Split(",");

            if (jsonListArray.Length == 0)
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Failed to load file : {storageRef.Name}");
            Logger.Exception(e);
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
            stringValue = Encoding.UTF8.GetString(loadedBytes);

            if (string.IsNullOrEmpty(stringValue))
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Failed to load file : {storageRef.Name}");
            Logger.Exception(e);
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

            if (loadedBytes.Length == 0)
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Failed to load file : {storageRef.Name}");
            Logger.Exception(e);
            return null;
        }

        return loadedBytes;
    }

    private async UniTask<byte[]> LoadLocalBytes(string localPath)
    {
        if (File.Exists(localPath))
        {
            byte[] localBytes = await UniTask.RunOnThreadPool(() => File.ReadAllBytes(localPath));
            return localBytes;
        }
        return null;
    }

    private async UniTask<bool> SaveDataToLocalPath(string localPath, byte[] loadedBytes)
    {
        try
        {
            await UniTask.RunOnThreadPool(() => File.WriteAllBytes(localPath, loadedBytes));
        }
        catch (System.Exception e)
        {
            Logger.Error($"Failed to save file : {localPath}");
            Logger.Exception(e);
            return false;
        }

        return true;
    }

    private bool IntegrityCheck(byte[] localBytes, byte[] loadedBytes)
    {
        return MD5.Create().ComputeHash(localBytes).SequenceEqual(MD5.Create().ComputeHash(loadedBytes));
    }

    private bool IsValidByteArray(byte[] bytes)
    {
        if (bytes == null)
            return false;

        if (bytes.Length == 0)
            return false;

        return true;
    }
}
