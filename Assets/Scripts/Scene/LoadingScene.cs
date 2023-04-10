using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    private enum LoadDataType
    {
        LocalPath,
        FireBase,
    }

    [SerializeField]
    private LoadDataType loadDataType;

    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private Slider progressBar;

    [Header("Local Json Path")]
    [SerializeField]
    private string localJsonDataPath = PathDefine.Json;

    [Header("FireBase Bucket Name")]
    [SerializeField]
    private string bucketName = "jhgunity";

    private LocalDataLoader localDataLoader;
    private FireBaseDataLoader fireBaseDownlader;

    private void Awake()
    {
        switch (loadDataType)
        {
            case LoadDataType.LocalPath:
                localDataLoader = new LocalDataLoader(localJsonDataPath);
                localDataLoader.SetOnLoadData(OnLoadData);
                localDataLoader.LoadData().Forget();
                break;

            case LoadDataType.FireBase:
                fireBaseDownlader = new FireBaseDataLoader(bucketName);
                fireBaseDownlader.SetOnLoadData(OnLoadData);
                fireBaseDownlader.LoadData().Forget();
                break;
        }

        ShowLoadProgress().Forget();
    }

    private void OnLoadData()
    {
        MoveToMainScene();
    }

    private void MoveToMainScene()
    {
        SceneManager.LoadScene((int)SceneType.Main, LoadSceneMode.Single);
    }

    private async UniTaskVoid ShowLoadProgress()
    {
        BaseDataLoader currentLoader = loadDataType == LoadDataType.LocalPath ? localDataLoader : fireBaseDownlader;

        while (currentLoader.CurrentState != BaseDataLoader.State.Fail)
        {
            if (currentLoader.CurrentProgressString != null)
            {
                progressText.SafeSetText(currentLoader.CurrentProgressString);
                progressBar.value = currentLoader.CurrentProgressValue;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
