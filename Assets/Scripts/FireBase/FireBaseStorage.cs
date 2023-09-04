using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public struct FireBaseStorage
{
    #region Storage Path Define
    public string AppSpot => $"gs://{BucketName}.appspot.com/";

    public string CurrentJsonVersionStoragePath => $"CurrentVersion/{NameDefine.JsonVersionTxtName}";

    public string CurrentAddressableVersionStoragePath => $"CurrentVersion/{NameDefine.AddressableVersionTxtName}";

    public string DataUploadHistoryStoargePath => $"{JsonDatasStoragePath}/{NameDefine.UploadHistory}";

    public string AddressableUploadHistoryStoragePath => $"{AddressableBuildPath}/{NameDefine.UploadHistory}";

    public string JsonDatasStoragePath
    {
        get
        {
            if (string.IsNullOrEmpty(JsonVersion))
            {
                Logger.Warning("Version not initialized");
                return null;
            }
            return $"JsonDatas/{JsonVersion}/";
        }
    }

    public string JsonListStoragePath => $"JsonDatas/{JsonVersion}/JsonList.txt";

    public string GetJsonStoragePath(string jsonNameWithExtension)
    {
        return $"{JsonDatasStoragePath}{jsonNameWithExtension}";
    }

    public string AddressableBuildPath => $"{PathDefine.AddressableBuildPathByPlatform}/{AddressableVersion}";

    public string GetAddressableBuildStoragePath(string addressableBuildNameWithExtension)
    {
        return $"{AddressableBuildPath}/{addressableBuildNameWithExtension}";
    }

    public string AddressableBuildInfoStoragePath => $"{AddressableBuildPath}/{NameDefine.AddressableBuildInfoName}";

    private int MaxJsonSizeBytes => 10000000;
    #endregion

    public FirebaseStorage Storage => FirebaseStorage.GetInstance(AppSpot);

    public FireBaseStorage(string bucketName, string jsonVersion = null, string addressableVersion = null)
    {
        BucketName = bucketName;
        JsonVersion = jsonVersion;
        AddressableVersion = addressableVersion;
    }

    public void SetJsonVersion(string version)
    {
        JsonVersion = version;
    }

    public void SetAddressableVersion(string addressableVersion)
    {
        AddressableVersion = addressableVersion;
    }

    public string BucketName
    {
        get; private set;
    }

    public string JsonVersion
    {
        get; private set;
    }

    public string AddressableVersion
    {
        get; private set;
    }

    private StorageReference GetStorageRef(string path)
    {
        return Storage.RootReference.Child(path);
    }

    public async UniTask<string> LoadString(string storagePath)
    {
        string stringValue;

        try
        {
            byte[] loadedBytes = await GetStorageRef(storagePath).GetBytesAsync(MaxJsonSizeBytes);
            stringValue = loadedBytes.GetStringUTF8();

            if (string.IsNullOrEmpty(stringValue))
            {
                Logger.Error($"Failed to load file : {storagePath}");
                return null;
            }
        }
        catch (Exception e)
        {
            Logger.Exception($"Failed to load file : {storagePath}", e);
            return null;
        }

        return stringValue;
    }

    public async UniTask<byte[]> LoadBytes(string storagePath)
    {
        byte[] loadedBytes;

        try
        {
            loadedBytes = await GetStorageRef(storagePath).GetBytesAsync(MaxJsonSizeBytes);

            if (!loadedBytes.IsValidArray())
            {
                Logger.Error($"Failed to load file : {storagePath}");
                return null;
            }
        }
        catch (Exception e)
        {
            Logger.Exception($"Failed to load file : {storagePath}", e);
            return null;
        }

        return loadedBytes;
    }

    public Task<byte[]> GetByteAsync(string storagePath)
    {
        return GetStorageRef(storagePath).GetBytesAsync(MaxJsonSizeBytes);
    }

    public Task<StorageMetadata> GetPutAsync(string storagePath, byte[] bytes)
    {
        return GetStorageRef(storagePath).PutBytesAsync(bytes);
    }

    public Task GetDeleteAsync(string storagePath)
    {
        return GetStorageRef(storagePath).DeleteAsync();
    }

    public void Dispose()
    {
        Firebase.FirebaseApp.DefaultInstance.Dispose();

        if (Storage?.App != null)
            Storage.App.Dispose();
    }
}
