using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBoardScene : MonoBehaviour
{
    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = "jhgunity";

    private DataLoadingController dataLoadingController;
    
    private DataBoardController dataBoardController;

    private async UniTask InitializeLoadingController()
    {
        //warning방지 임시
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    private async UniTask InitializeDataBoardController()
    {
        dataBoardController = new DataBoardController();

        await dataBoardController.Initialize(DataManager.Instance.GetAllTypes().ToArray());

        dataBoardController.Show();
    }
}
