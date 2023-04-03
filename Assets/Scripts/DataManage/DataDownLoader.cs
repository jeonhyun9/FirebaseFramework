using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Firebase.Storage;
using System.Text;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Linq;

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
        LoadDataFromJson().Forget();
    }

    private async UniTask<bool> LoadDataFromJson()
    {
        bool loadDataResult;

        switch (loadDataType)
        {
            case LoadDataType.FireBase:
                loadDataResult = await LoadDataFromFireBase(bucketName);
                break;

            default:
                loadDataResult = LoadDataFromLocalPath(localJsonDataPath);
                break;
        }

        if (loadDataResult)
            ShowTestLog();

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
            bool addContainerResult = AddDataContainerWithLog(fileName, jsonDicByFileName[fileName]);

            if (!addContainerResult)
                return false;
        }

        Debug.Log($"Data Version : {fireBaseDataDownloader.Version}");

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

            bool addContainerResult = AddDataContainerWithLog(fileName, localJson);

            if (!addContainerResult)
                return false;
        }

        return true;
    }

    private bool AddDataContainerWithLog(string fileName, string json)
    {
        if (DataContainerManager.Instance.AddDataContainer(fileName, json))
        {
            Debug.Log($"Data Load Success {fileName} ");
        }
        else
        {
            Debug.LogError($"Data Load Failed {fileName} ");
            return false;
        }

        return true;
    }


    private void ShowTestLog()
    {
        DataHuman human = DataContainerManager.Instance.GetDataContainer<DataHumanContainer>().GetById(1);
        Debug.Log($"Id가 1인 DataHuman : {human.NameId}");

        DataHuman winter = DataContainerManager.Instance.GetDataContainer<DataHumanContainer>().GetById(2);
        Debug.Log($"Id가 2인 DataHuman {human.NameId}의 펫 : {winter.Pet.NameId}");

        DataAnimal animal = DataContainerManager.Instance.GetDataContainer<DataAnimalContainer>().GetById(3);
        Debug.Log($"Id가 3인 DataAnimal : {animal.NameId}");
    }
}
