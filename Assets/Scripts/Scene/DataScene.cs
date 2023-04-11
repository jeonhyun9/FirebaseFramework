using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataScene : MonoBehaviour
{
    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = "jhgunity";

    private Type[] DataBoardUseTypes => DataManager.Instance.GetAllTypes();

    private void ShowDataBoard()
    {
        UIManager.Instance.CreateUI(InitDataBoard(DataBoardUseTypes));
    }
    private DataBoardController InitDataBoard(Type[] useTypes)
    {
        return new DataBoardController(useTypes);
    }

    private void ShowDataLoading()
    {

    }

    private void ProcessLoadingController()
    {

    }
}
