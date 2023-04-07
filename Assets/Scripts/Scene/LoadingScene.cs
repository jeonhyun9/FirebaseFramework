using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    DataDownLoader downloader = null;

    private void Awake()
    {
        if (downloader == null)
            return;

        downloader.LoadData(() =>{ OnLoadData(); }).Forget();
    }

    private void OnLoadData()
    {
        MoveToMainScene();
    }

    private void MoveToMainScene()
    {
        SceneManager.LoadScene((int)SceneType.Main, LoadSceneMode.Single);
    }
}
