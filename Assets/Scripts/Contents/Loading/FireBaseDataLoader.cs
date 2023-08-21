using Cysharp.Threading.Tasks;
using Firebase.Storage;
using System;
using System.IO;
using System.Linq;

public class FireBaseDataLoader : BaseDataLoader
{
    private FireBaseDefine fireBaseDef;

    public FirebaseStorage Storage => fireBaseDef.Storage;
    
    public void InitializeFireBaseDefine(string bucketName)
    {
        fireBaseDef = new FireBaseDefine(bucketName);
    }

    public async override UniTaskVoid LoadData()
    {
        DicJsonByFileName.Clear();

        Logger.Log("Start Load Data");

        //FireBaseStorage에서 파일명과 json을 불러와서 Dictionary에 담는 과정
        bool loadDataResult = await LoadDataFromFireBase();

        Logger.Log($"Load Data Result {loadDataResult}");

        ChangeState(loadDataResult ? State.Success : State.Fail);
    }

    private async UniTask<bool> LoadDataFromFireBase()
    {
        if (!await LoadFireBaseDefVersion())
            return false;
        
        string[] jsonList = await LoadJsonList(fireBaseDef.JsonListPath);

        if (!jsonList.IsValidArray())
        {
            Logger.Error("jsonList is null or empty");
            return false;
        }

        float progressIncrementValue = 1f / jsonList.Length;

        return await LoadAllJsonToDic(jsonList, progressIncrementValue);
    }

    private async UniTask<bool> LoadFireBaseDefVersion()
    {
        ChangeState(State.LoadVersion);

        StorageReference versionRef = fireBaseDef.Storage.RootReference.Child(fireBaseDef.CurrentVersionPath);

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

        StorageReference storageRef = fireBaseDef.Storage.RootReference.Child(refPath);

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

    private async UniTask<bool> LoadAllJsonToDic(string[] jsonList, float progressIncrementValue)
    {
        UniTask<bool>[] tasks = jsonList.Select(json => LoadJsonToDic(json, progressIncrementValue)).ToArray();
        bool[] results = await UniTask.WhenAll(tasks);

        return results.All(x => true);
    }

    private async UniTask<bool> LoadJsonToDic(string jsonName, float progressIncrementValue)
    {
        string loadedString = await LoadJsonByName(jsonName);

        if (!string.IsNullOrEmpty(loadedString))
        {
            AddToDic(Path.GetFileName(jsonName), loadedString);
            CurrentProgressValue += progressIncrementValue;
            return true;
        }

        return false;
    }

    private async UniTask<string> LoadJsonByName(string jsonName)
    {
        Logger.Log($"Try load {jsonName}");

        StorageReference jsonDataRef = fireBaseDef.Storage.RootReference.Child(fireBaseDef.GetJsonPath(jsonName));

        byte[] loadedBytes = await LoadBytes(jsonDataRef);

        //불러온 데이터 예외처리
        if (loadedBytes.IsValidArray())
        {
            string loadedString = loadedBytes.GetStringUTF8();

            if (!string.IsNullOrEmpty(loadedString))
                return loadedString;
        }
       
        Logger.Error($"Invalid load json {jsonName}");
        return null;
    }
    
    private void AddToDic(string jsonName, string jsonContents)
    {
        string fileName = Path.GetFileName(jsonName);

        // 컨테이너에 담길 데이터 추가
        DicJsonByFileName.Add(fileName, jsonContents);
        Logger.Success($"Load Json From FireBase : {fileName}");
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

    private async UniTask<string> GetDownloadUrlFromStoragePath(string storagePath)
    {
        StorageReference reference = fireBaseDef.Storage.GetReferenceFromUrl($"{fireBaseDef.AppSpot}{storagePath}");

        try
        {
            var downloadUrl = await reference.GetDownloadUrlAsync();
            return downloadUrl.ToString();
        }
        catch (Exception e)
        {
            Logger.Exception($"Failed to get firebase download url from path : {storagePath}", e);
            return null;
        }
    }
}
