using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FireBaseDataLoader : BaseDataLoader
{
    public FirebaseStorage Storage { get; private set; }

    private FireBaseDefine fireBaseDef;
    
    public void SetBucketName(string bucketName)
    {
        fireBaseDef = new FireBaseDefine(bucketName);
        Storage = FirebaseStorage.GetInstance(fireBaseDef.AppSpot);
    }

    public async override UniTaskVoid LoadData()
    {
        bool loadDataResult = await LoadDataDicFromFireBase();

        ChangeState(loadDataResult ? State.Success : State.Fail);
    }

    private async UniTask<bool> LoadDataDicFromFireBase()
    {
        DicJsonByFileName.Clear();

        if (!await LoadFireBaseDefVersion())
            return false;
        
        string[] jsonList = await LoadJsonList(fireBaseDef.JsonListPath);

        if (!jsonList.IsValidArray())
        {
            Logger.Error("jsonList is null or empty");
            return false;
        }

        float progressIncrementValue = 1f / jsonList.Length;

        UniTask[] tasks = jsonList.Select(json => AddJsonToDic(json, progressIncrementValue)).ToArray();

        await UniTask.WhenAll(tasks);

        Logger.Success($"Load Data Version : {fireBaseDef.Version}");

        return true;
    }

    private async UniTask<bool> LoadFireBaseDefVersion()
    {
        ChangeState(State.LoadVersion);

        StorageReference versionRef = Storage.RootReference.Child(fireBaseDef.CurrentVersionPath);

        string currentVersion = await LoadString(versionRef);

        if (!string.IsNullOrEmpty(currentVersion))
        {
            fireBaseDef.SetVersion(currentVersion);
            CurrentProgressValue += 0.1f;
            return true;
        }

        return false;
    }

    private async UniTask<string[]> LoadJsonList(string refPath)
    {
        ChangeState(State.LoadJsonList);

        StorageReference storageRef = Storage.RootReference.Child(refPath);

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

        CurrentProgressValue += 0.1f;
        return jsonListArray;
    }

    private async UniTask AddJsonToDic(string jsonName, float progressValue)
    {
        ChangeState(State.LoadJson);

        string fileName = Path.GetFileName(jsonName);

        StorageReference jsonDataRef = Storage.RootReference.Child(fireBaseDef.GetJsonPath(jsonName));

        byte[] loadedBytes = await LoadBytes(jsonDataRef);

        //불러온 데이터 예외처리
        if (loadedBytes.IsValidArray())
        {
            string loadedString = loadedBytes.GetStringUTF8();

            if (!string.IsNullOrEmpty(loadedString))
            {
                // 컨테이너에 담길 데이터 추가
                DicJsonByFileName.Add(fileName, loadedString);
                CurrentProgressValue += progressValue;
                Logger.Success($"Load Json From FireBase : {fileName}");
            }
        }
        else
        {
            Logger.Error($"Invalid load json {fileName}");
        }
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
        catch (Exception e)
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
        catch (Exception e)
        {
            Logger.Exception($"Failed to load file : {storageRef.Name}", e);
            return null;
        }

        return loadedBytes;
    }
}
