using UnityEngine;

public class DataLoadingController : BaseController<DataLoadingView,BaseDataLoader>
{
    public enum LoadDataType
    {
        LocalPath,
        FireBase,
    }

    public bool IsSuccess => Model?.CurrentState == BaseDataLoader.State.Success;

    private LocalDataLoader LocalDataLoader => Model.GetLoader<LocalDataLoader>();

    private FireBaseDataLoader FireBaseDataLoader => Model.GetLoader<FireBaseDataLoader>();

    private readonly LoadDataType loadDataType;

    private readonly string localJsonDataPath;

    private readonly string bucketName;

    public DataLoadingController(LoadDataType loadDataTypeValue, string localJsonDataPathValue, string bucketNameValue)
    {
        loadDataType = loadDataTypeValue;
        localJsonDataPath = localJsonDataPathValue;
        bucketName = bucketNameValue;
    }

    protected override string GetViewPrefabName()
    {
        return nameof(DataLoadingController).Replace("Controller", "View");
    }

    protected override BaseDataLoader CreateModel()
    {
        switch (loadDataType)
        {
            case LoadDataType.LocalPath:
                LocalDataLoader localDataLoader = new();
                localDataLoader.SetLocalJsonDataPath(localJsonDataPath);
                localDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
                localDataLoader.LoadData().Forget();

                return localDataLoader;

            case LoadDataType.FireBase:
                FireBaseDataLoader fireBaseDataLoader = new();
                fireBaseDataLoader.SetBucketName(bucketName);
                fireBaseDataLoader.SetOnFinishLoadData(OnFinishFireBaseDataLoader);
                fireBaseDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
                fireBaseDataLoader.LoadData().Forget();

                return fireBaseDataLoader;
        }

        return null;
    }

    private void OnFinishFireBaseDataLoader()
    {
        if (FireBaseDataLoader.Storage.App != null)
            FireBaseDataLoader.Storage.App.Dispose();

        Firebase.FirebaseApp.DefaultInstance.Dispose();

        //씬에 남아있는 경우가 있음..
        GameObject go = GameObject.Find("Firebase Services");

        if (go != null)
        {
            Logger.Log("Firebase Services destroyed.");
            Object.Destroy(go);
        }
    }

    private void OnSuccessDataLoader()
    {
        DataManager.Instance.AddDataContainerByDataDic(Model.DicJsonByFileName);
    }
}
