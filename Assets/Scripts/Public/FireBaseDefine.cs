public struct FireBaseDefine
{
    public FireBaseDefine(string bucketName)
    {
        BucketName = bucketName;
    }

    public string BucketName
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
            return AppSpot + "CurrentVersion/version.txt";
        }
    }

    public const int MaxJsonSizeBytes = 10000;
}
