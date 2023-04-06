using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Linq;
using System;

public class DataDownLoader : MonoBehaviour
{
    private enum LoadDataType
    {
        LocalPath,
        FireBase,
    }

    [SerializeField]
    private LoadDataType loadDataType;

    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = "jhgunity";

    private void Awake()
    {
        //테스트
        LoadData(()=> { ShowTestLog(); }).Forget();
    }

    private async UniTask<bool> LoadData(Action sucessCallback)
    {
        bool loadDataResult;

        switch (loadDataType)
        {
            case LoadDataType.FireBase:
                loadDataResult = await LoadDataFromFireBase(bucketName);
                break;
            case LoadDataType.LocalPath:
            default:
                loadDataResult = LoadDataFromLocalPath(localJsonDataPath);
                break;
        }

        if (loadDataResult && sucessCallback != null)
            sucessCallback.Invoke();

        return loadDataResult;
    }

    private async UniTask<bool> LoadDataFromFireBase(string bucketNameValue)
    {
        FireBaseDataDownloader fireBaseDataDownloader = new(bucketNameValue);

        Dictionary<string, string> jsonDicByFileName = await fireBaseDataDownloader.LoadDataDicFromFireBase();

        if (jsonDicByFileName == null)
            return false;

        foreach(string fileName in jsonDicByFileName.Keys)
        {
            bool addContainerResult = AddDataContainerToManager(fileName, jsonDicByFileName[fileName]);

            if (!addContainerResult)
                return false;
        }

        Logger.Success($"Load Data Version : {fireBaseDataDownloader.Version}");

        return true;
    }

    private bool LoadDataFromLocalPath(string jsonPath)
    {
        if (!Directory.Exists(jsonPath))
            return false;

        string[] localJsonFileNames = Directory.GetFiles(jsonPath, $"*.json")
            .Select(Path.GetFileName)
            .ToArray();

        foreach (string fileName in localJsonFileNames)
        {
            string localJson = File.ReadAllText(Path.Combine(jsonPath, fileName));

            bool addContainerResult = AddDataContainerToManager(fileName, localJson);

            if (!addContainerResult)
                return false;
        }

        return true;
    }

    private bool AddDataContainerToManager(string fileName, string json)
    {
        fileName = Path.GetFileNameWithoutExtension(fileName);

        return DataContainerManager.Instance.AddDataContainer(fileName, json);
    }

    //테스트
    private void ShowTestLog()
    {
        DataHuman human = DataContainerManager.Instance.GetDataById<DataHuman>(1);
        Logger.Log($"Id가 1인 DataHuman : {human.NameId}");
        
        DataHuman winter = DataContainerManager.Instance.GetDataById<DataHuman>(2);
        Logger.Log($"Id가 2인 DataHuman {human.NameId}의 펫 : {winter.Pet.NameId}");
        
        DataAnimal animal = DataContainerManager.Instance.GetDataById<DataAnimal>(3);
        Logger.Log($"Id가 3인 DataAnimal : {animal.NameId}");
        
        DataAnimal Lion = DataContainerManager.Instance.FindData<DataAnimal>(x => x.AnimalType == AnimalType.Lion);
        Logger.Log($"AnimalType이 Lion인 DataAnimal {animal.NameId}");
    }
}
