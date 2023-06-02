using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DataScene : MonoBehaviour
{
    [SerializeField]
    DataLoadingController.LoadDataType loadDataType;

    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = "jhgunity";

    private Type[] DataBoardUseTypes => DataManager.Instance.GetAllTypes();

    private void Awake()
    {
        ShowData();
    }

    private void ShowData()
    {
        ShowDataAsync().Forget();
    }

    private async UniTaskVoid ShowDataAsync()
    {
        bool result = await ShowDataLoading();

        if (result)
            await ShowDataBoard();
    }

    private async UniTask<bool> ShowDataLoading()
    {
        DataLoadingController dataLoadingController = new ();

        switch (loadDataType)
        {
            case DataLoadingController.LoadDataType.LocalPath:
                FireBaseDataLoader fireBaseDataLoader = new ();
                fireBaseDataLoader.SetBucketName(bucketName);
                dataLoadingController.SetModel(fireBaseDataLoader);
                break;

            case DataLoadingController.LoadDataType.FireBase:
                LocalDataLoader localDataLoader = new ();
                localDataLoader.SetLocalJsonDataPath(localJsonDataPath);
                dataLoadingController.SetModel(localDataLoader);
                break;
        }

        await dataLoadingController.ProcessAsync();
        return dataLoadingController.IsSuccess;
    }

    private async UniTask ShowDataBoard()
    {
        DataBoardController dataBoardController = new ();

        DataBoardViewModel model = new();
        model.SetUseTypes(DataBoardUseTypes);

        dataBoardController.SetModel(model);

        await dataBoardController.ProcessAsync();
    }
}
