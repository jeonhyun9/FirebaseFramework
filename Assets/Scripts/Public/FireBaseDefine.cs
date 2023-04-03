public struct FireBaseDefine
{
    public FireBaseDefine(string bucketName, string version)
    {
        BucketName = bucketName;
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

    public string AppSpot
    {
        get
        {
            return $"gs://{BucketName}.appspot.com/";
        }
    }

    public string CurrentVersionPath
    {
        get
        {
            return "CurrentVersion/Version.txt";
        }
    }

    public string JsonDatasPath
    {
        get
        {
            return $"JsonDatas/{Version}/";
        }
    }

    public string JsonListPath
    {
        get
        {
            return $"JsonDatas/{Version}/JsonList.txt";
        }
    }

    public const int MaxJsonSizeBytes = 10000;
}
