using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Linq;

public class LocalDataLoader : BaseDataLoader
{
    private string localJsonDataPath;

    public void SetLocalJsonDataPath(string localJsonDataPathValue)
    {
        localJsonDataPath = localJsonDataPathValue;
    }

    public async override UniTaskVoid LoadData()
    {
        bool loadDataResult = await LoadDataFromLocalPath(localJsonDataPath);

        ChangeState(loadDataResult ? State.Success : State.Fail);
    }

    private async UniTask<bool> LoadDataFromLocalPath(string jsonPath)
    {
        if (!Directory.Exists(jsonPath))
        {
            ChangeState(State.Fail);
            return false;
        }

        string[] localJsonFileNames = Directory.GetFiles(jsonPath, $"*.json")
            .Select(Path.GetFileName)
            .ToArray();

        if (!localJsonFileNames.IsValidArray())
        {
            ChangeState(State.Fail);
            return false;
        }

        float progressIncrementValue = 1f / localJsonFileNames.Length;

        ChangeState(State.LoadJson);

        foreach (string fileName in localJsonFileNames)
        {
            string localJson = null;

            await UniTask.RunOnThreadPool(() => { localJson = File.ReadAllText(Path.Combine(jsonPath, fileName)); });

            if (string.IsNullOrEmpty(localJson))
            {
                ChangeState(State.Fail);
                return false;
            }

            DicJsonByFileName.Add(fileName, localJson);

            CurrentProgressValue += progressIncrementValue;
        }

        return true;
    }
}
