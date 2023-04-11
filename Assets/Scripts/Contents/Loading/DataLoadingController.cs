using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoadingController : BaseController
{
    public enum LoadDataType
    {
        LocalPath,
        FireBase,
    }

    private BaseDataLoader Model => GetModel<BaseDataLoader>();

    private LocalDataLoader LocalDataModel => GetModel<LocalDataLoader>();

    private FireBaseDataLoader FireBaseDataModel => GetModel<FireBaseDataLoader>();

    private DataLoadingView View => GetView<DataLoadingView>();

    private readonly LoadDataType loadDataType;

    private readonly string localJsonDataPath;

    private readonly string bucketName;

    public DataLoadingController(LoadDataType loadDataTypeValue, string localJsonDataPathValue, string bucketNameValue)
    {
        loadDataType = loadDataTypeValue;
        localJsonDataPath = localJsonDataPathValue;
        bucketName = bucketNameValue;
    }

    public override void InitContentsName()
    {
        ContentsName = nameof(DataLoadingController).Replace("Controller", "");
    }

    public override void InitModel()
    {
        Model.SetOnLoadJson(AddDataContainerToManager);

        switch (loadDataType)
        {
            case LoadDataType.LocalPath:
                LocalDataLoader localDataModel = Model as LocalDataLoader;
                localDataModel.SetLocalJsonDataPath(localJsonDataPath);
                localDataModel.LoadData().Forget();
                break;

            case LoadDataType.FireBase:
                FireBaseDataLoader fireBaseDataModel = Model as FireBaseDataLoader;
                fireBaseDataModel.SetBucketName(bucketName);
                fireBaseDataModel.SetOnChangeState(CleanUpFireBase);
                fireBaseDataModel.LoadData().Forget();
                break;
        }
    }

    private void CleanUpFireBase(BaseDataLoader.State state)
    {
        if (loadDataType != LoadDataType.FireBase)
            return;

        if (state == BaseDataLoader.State.Done || state == BaseDataLoader.State.Fail)
        {
            if (FireBaseDataModel.Storage.App != null)
                FireBaseDataModel.Storage.App.Dispose();

            Firebase.FirebaseApp.DefaultInstance.Dispose();

            GameObject go = GameObject.Find("Firebase Services");

            if (go != null)
                Object.Destroy(go);
        }
    }

    private bool AddDataContainerToManager(string fileName, string json)
    {
        fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);

        return DataManager.Instance.AddDataContainer(fileName, json);
    }
}
