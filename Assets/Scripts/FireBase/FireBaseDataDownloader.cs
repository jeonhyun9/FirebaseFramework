using Cysharp.Threading.Tasks;
using Firebase.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FireBaseDataDownloader
{
    public enum State
    {
        LoadVersion,
        LoadJsonList,
        LoadJson,
        Done,
        Fail,
    }

    public string ProgressString
    {
        get
        {
            switch (currentState)
            {
                case State.LoadVersion:
                    return "Loading version...";
                case State.LoadJsonList:
                    return "Loading JsonList...";
                case State.LoadJson:
                    return $"Loading {currentLoadingJsonName}";
                case State.Done:
                    return $"Done!";
                default:
                    return null;
            }
        }
    }

    public string Version => fireBaseDef.Version;
    public string JsonSavePath => Application.persistentDataPath;

    private readonly Dictionary<string, string> dicJsonByFileName = new ();
    private FireBaseDefine fireBaseDef;
    private readonly FirebaseStorage storage;
    private State currentState;
    private string currentLoadingJsonName;
    
    public FireBaseDataDownloader(string bucketName)
    {
        fireBaseDef = new FireBaseDefine(bucketName);
        storage = FirebaseStorage.GetInstance(fireBaseDef.AppSpot);
    }

    public async UniTask<Dictionary<string,string>> LoadDataDicFromFireBase()
    {
        dicJsonByFileName.Clear();

        if (!await LoadFireBaseDefVersion())
            return null;
        
        string[] jsonList = await LoadJsonList(fireBaseDef.JsonListPath);

        if (!jsonList.IsValidArray())
        {
            Logger.Error("jsonList is null or empty");
            return null;
        }

        UniTask[] tasks = jsonList.Select(json => AddJsonToDic(json)).ToArray();

        await UniTask.WhenAll(tasks);

        if (storage?.App != null)
            storage.App.Dispose();

        return dicJsonByFileName;
    }

    private async UniTask<bool> LoadFireBaseDefVersion()
    {
        ChangeState(State.LoadVersion);

        StorageReference versionRef = storage.RootReference.Child(fireBaseDef.CurrentVersionPath);

        string currentVersion = await LoadString(versionRef);

        if (!string.IsNullOrEmpty(currentVersion))
        {
            fireBaseDef.SetVersion(currentVersion);
            return true;
        }

        return false;
    }

    private async UniTask<string[]> LoadJsonList(string refPath)
    {
        ChangeState(State.LoadJsonList);

        StorageReference storageRef = storage.RootReference.Child(refPath);

        string[] jsonListArray;

        try
        {
            byte[] jsonListBytes = await storageRef.GetBytesAsync(fireBaseDef.MaxJsonSizeBytes);
            jsonListArray = jsonListBytes.GetStringUTF8()?.Split(",");

            if (!jsonListArray.IsValidArray())
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return jsonListArray;
    }

    private async UniTask<string> LoadString(StorageReference storageRef)
    {
        string stringValue;

        try
        {
            byte[] loadedBytes = await storageRef.GetBytesAsync(fireBaseDef.MaxJsonSizeBytes);
            stringValue = loadedBytes.GetStringUTF8();

            if (string.IsNullOrEmpty(stringValue))
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return stringValue;
    }

    private async UniTask<byte[]> LoadBytes(StorageReference storageRef)
    {
        byte[] loadedBytes;

        try
        {
            loadedBytes = await storageRef.GetBytesAsync(fireBaseDef.MaxJsonSizeBytes);

            if (!loadedBytes.IsValidArray())
            {
                Logger.Error($"Failed to load file : {storageRef.Name}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return loadedBytes;
    }

    private async UniTask AddJsonToDic(string jsonName)
    {
        currentLoadingJsonName = jsonName;
        ChangeState(State.LoadJson);

        string fileName = Path.GetFileName(jsonName);

        StorageReference jsonDataRef = storage.RootReference.Child(fireBaseDef.GetJsonPath(jsonName));

        byte[] loadedBytes = await LoadBytes(jsonDataRef);

        //불러온 데이터 예외처리
        if (loadedBytes.IsValidArray())
        {
            string loadedString = loadedBytes.GetStringUTF8();

            if (!string.IsNullOrEmpty(loadedString))
            {
                // 컨테이너에 담길 데이터 추가
                dicJsonByFileName.Add(fileName, loadedString);
                Logger.Success($"Load Json From FireBase : {fileName}");
            }
        }
        else
        {
            Logger.Error($"Invalid load json {fileName}");
        }
    }

    private void ChangeState(State state)
    {
        currentState = state;
    }
}
