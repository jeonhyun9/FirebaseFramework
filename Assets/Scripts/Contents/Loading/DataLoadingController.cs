using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoadingController : BaseController<DataLoadingView,BaseDataLoader>
{
    public enum LoadDataType
    {
        LocalPath,
        FireBase,
    }

    public bool IsSuccess => Model?.CurrentState == BaseDataLoader.State.Success;

    private LocalDataLoader LocalDataLoader;

    private FireBaseDataLoader FireBaseDataLoader;

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
                LocalDataLoader = new();
                LocalDataLoader.SetLocalJsonDataPath(localJsonDataPath);
                LocalDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
                LocalDataLoader.LoadData().Forget();

                return LocalDataLoader;

            case LoadDataType.FireBase:
                FireBaseDataLoader = new();
                FireBaseDataLoader.SetBucketName(bucketName);
                FireBaseDataLoader.SetOnFailLoadData(OnFailFireBaseDataLoader);
                FireBaseDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
                FireBaseDataLoader.LoadData().Forget();

                return FireBaseDataLoader;
        }

        return null;
    }

    private void OnFailFireBaseDataLoader()
    {
        if (FireBaseDataLoader == null)
        {
            Logger.Null(FireBaseDataLoader);
            return;
        }

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
        if (Model == null)
        {
            Logger.Null(Model);
            return;
        }

        DataManager.Instance.AddDataContainerByDataDic(Model.DicJsonByFileName);
    }
}
