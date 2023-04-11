using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        DataLoadingController dataLoadingController = InitDataLoading(loadDataType, localJsonDataPath, bucketName);
        await dataLoadingController.ProcessAsync();
        return dataLoadingController.IsSuccess;
    }

    private DataLoadingController InitDataLoading(DataLoadingController.LoadDataType loadDataTypeValue, string localJsonDataPathValue, string bucketNameValue)
    {
        return new DataLoadingController(loadDataTypeValue, localJsonDataPathValue, bucketNameValue);
    }

    private async UniTask ShowDataBoard()
    {
        DataBoardController dataBoardController = InitDataBoard(DataBoardUseTypes);
        await dataBoardController.ProcessAsync();
    }
    private DataBoardController InitDataBoard(Type[] useTypes)
    {
        return new DataBoardController(useTypes);
    }
}
