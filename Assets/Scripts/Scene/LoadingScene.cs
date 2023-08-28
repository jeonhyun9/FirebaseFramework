using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    private DataLoadingController.LoadDataType loadDataType;

    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = NameDefine.BucketDefaultName;
    private Type[] DataBoardUseTypes => DataManager.Instance.GetAllTypes();

    private void Awake()
    {
        Loading().Forget();
    }

    private async UniTask Loading()
    {
        bool addressableResult = await AddressableManager.Instance.LoadAddressableAsync();

        if (addressableResult)
        {
            await ShowDataAsync();
        }
        else
        {
            Logger.Error("Fail to Load Addressable");
            Application.Quit();
        }
    }

    private async UniTask ShowDataAsync()
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
            case DataLoadingController.LoadDataType.FireBase:
                FireBaseDataLoader fireBaseDataLoader = new ();
                fireBaseDataLoader.InitializeFireBaseDefine(bucketName);
                dataLoadingController.SetModel(fireBaseDataLoader);
                break;

            case DataLoadingController.LoadDataType.LocalPath:
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
