using System;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public struct FireBaseDefine
{
    public FirebaseStorage Storage => FirebaseStorage.GetInstance(AppSpot);

    public FireBaseDefine(string bucketName, string jsonVersion = null, string addressableVersion = null)
    {
        BucketName = bucketName;
        JsonVersion = jsonVersion;
        AddressableVersion = addressableVersion;
    }

    public void SetJsonVersion(string version)
    {
        JsonVersion = version;
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

    public string AppSpot => $"gs://{BucketName}.appspot.com/";

    public string CurrentJsonVersionStoragePath => $"CurrentVersion/{NameDefine.JsonVersionTxtName}";

    public string CurrentAddressableVersionStoragePath => $"CurrentVersion/{NameDefine.AddressableVersionTxtName}";

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

    public string AddressableBuildPath => $"{PathDefine.AddressableBuildPathByFlatform}/{AddressableVersion}";

    public string GetAddressableBuildStoragePath(string addressableBuildNameWithExtension)
    {
        return $"{AddressableBuildPath}/{addressableBuildNameWithExtension}";
    }

    public string AddressableListStoragePath => $"{AddressableBuildPath}/{NameDefine.AddressableListName}";

    public int MaxJsonSizeBytes => 10000;

    public async UniTask<string> GetDownloadUrlFromStoragePath(string storagePath)
    {
        StorageReference reference = Storage.GetReferenceFromUrl($"{AppSpot}{storagePath}");

        try
        {
            var downloadUrl = await reference.GetDownloadUrlAsync();
            return downloadUrl.ToString();
        }
        catch (Exception e)
        {
            Logger.Exception($"Failed to get firebase download url from path : {storagePath}", e);
            return null;
        }
    }
}
