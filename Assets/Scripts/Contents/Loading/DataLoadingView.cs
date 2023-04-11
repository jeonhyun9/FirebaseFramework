using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataLoadingView : BaseView
{
    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private Slider progressBar;

    private BaseDataLoader Model => GetModel<BaseDataLoader>();

    public override async UniTask ShowAsync()
    {
        await ShowLoadProgress();
    }

    private async UniTask ShowLoadProgress()
    {
        while (Model.CurrentState != BaseDataLoader.State.Fail)
        {
            if (Model.CurrentProgressString != null)
            {
                progressText.SafeSetText(Model.CurrentProgressString);
                progressBar.value = Model.CurrentProgressValue;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
