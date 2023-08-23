using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DataScene : MonoBehaviour
{
    [SerializeField]
    private DataLoadingController.LoadDataType loadDataType;

    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = NameDefine.BucketDefaultName;

    [SerializeField]
    private TextAsset testText;

    private Type[] DataBoardUseTypes => DataManager.Instance.GetAllTypes();

    private void Awake()
    {
        try
        {
            Logger.Log(testText.ToString());
            AddressableManager.Instance.Initialize(testText.text);
            ShowData();
        }
        catch (Exception e)
        {
            Logger.Log(e.ToString());
        }
    }

    private void ShowData()
    {
        Logger.Log("Show Data");
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
