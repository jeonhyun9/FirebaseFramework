using System;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public struct FireBaseDefine
{
    public FirebaseStorage Storage => FirebaseStorage.GetInstance(AppSpot);

    public FireBaseDefine(string bucketName, string version = null)
    {
        BucketName = bucketName;
        Version = version;
    }

    public void SetVersion(string version)
    {
        Version = version;
    }

    public string BucketName
    {
        get; private set;
    }

    public string Version
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
            if (string.IsNullOrEmpty(Version))
            {
                Logger.Warning("Version not initialized");
                return null;
            }
            return $"JsonDatas/{Version}/";
        }
    }

    public string JsonListStoragePath => $"JsonDatas/{Version}/JsonList.txt";

    public string GetJsonStoragePath(string jsonNameWithExtension)
    {
        return $"{JsonDatasStoragePath}{jsonNameWithExtension}";
    }

    public string AddressableBuildPath => $"{PathDefine.AddressableBuildPathByFlatform}/{Version}";

    public string GetAddressableBuildStoragePath(string addressableBuildNameWithExtension)
    {
        return $"{AddressableBuildPath}/{addressableBuildNameWithExtension}";
    }

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
