using UnityEngine;

public struct FireBaseDefine
{
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

    public string CurrentVersionPath => "CurrentVersion/Version.txt";

    public string JsonDatasPath
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

    public string JsonListPath
    {
        get
        {
            if (string.IsNullOrEmpty(Version))
            {
                Logger.Warning("Version not initialized");
                return null;
            }
            return $"JsonDatas/{Version}/JsonList.txt";
        }
    }

    public string GetJsonPath(string jsonNameWithExtension)
    {
        if (string.IsNullOrEmpty(Version))
        {
            Logger.Warning("Version not initialized");
            return null;
        }
        return $"JsonDatas/{Version}/{jsonNameWithExtension}";
    }

    public int MaxJsonSizeBytes => 10000;
}
