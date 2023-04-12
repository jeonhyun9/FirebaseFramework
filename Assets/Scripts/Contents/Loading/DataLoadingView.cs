using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataLoadingView : BaseView
{
    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private int waitingMilliSec = 1000;

    private BaseDataLoader Model => GetModel<BaseDataLoader>();

    public async override UniTask ShowAsync()
    {
        await ShowLoadProgress();
    }

    private async UniTask ShowLoadProgress()
    {
        while (true)
        {
            UpdateLoadingUI();

            if (Model.IsLoading == false)
            {
                Model.OnFinishLoadData?.Invoke();
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        if (Model.CurrentState == BaseDataLoader.State.Success)
        {
            Model.OnSuccessLoadData?.Invoke();

            await UniTask.Delay(waitingMilliSec);
            Hide();
        }
    }

    private void UpdateLoadingUI()
    {
        progressText.SafeSetText(Model.CurrentProgressString);
        progressBar.value = Model.CurrentProgressValue;
    }
}
