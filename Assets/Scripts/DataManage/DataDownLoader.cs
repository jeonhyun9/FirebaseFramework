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

    public FireBaseDataDownloader FireBaseDataDownloader { get; private set; }

    public async UniTask<bool> LoadData(Action sucessCallback)
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
        FireBaseDataDownloader = new(bucketNameValue);

        Dictionary<string, string> jsonDicByFileName = await FireBaseDataDownloader.LoadDataDicFromFireBase();

        if (jsonDicByFileName == null || jsonDicByFileName.Count == 0)
            return false;

        foreach(string fileName in jsonDicByFileName.Keys)
        {
            bool addContainerResult = AddDataContainerToManager(fileName, jsonDicByFileName[fileName]);

            if (!addContainerResult)
                return false;
        }

        Logger.Success($"Load Data Version : {FireBaseDataDownloader.Version}");

        return true;
    }

    private bool LoadDataFromLocalPath(string jsonPath)
    {
        if (!Directory.Exists(jsonPath))
            return false;

        string[] localJsonFileNames = Directory.GetFiles(jsonPath, $"*.json")
            .Select(Path.GetFileName)
            .ToArray();

        if (!localJsonFileNames.IsValidArray())
            return false;

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
}
