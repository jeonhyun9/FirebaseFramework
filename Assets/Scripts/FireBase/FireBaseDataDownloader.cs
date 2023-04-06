using Cysharp.Threading.Tasks;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FireBaseDataDownloader
{
    private readonly Dictionary<string, string> dicJsonByFileName = new ();

    private FireBaseDefine fireBaseDef;
    private readonly FirebaseStorage storage;

    public string Version => fireBaseDef.Version;

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
            StorageReference jsonDataRef = storage.RootReference.Child(fireBaseDef.GetJsonPath(jsonName));
            string jsonData = await LoadString(jsonDataRef);

            if (string.IsNullOrEmpty(jsonData))
                continue;

            dicJsonByFileName.Add(fileName, jsonData);
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
}
