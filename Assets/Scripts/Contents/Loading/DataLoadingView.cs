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
        while (Model.IsLoading)
        {
            UpdateLoadingUI();
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        Model.OnFinishLoadData?.Invoke();

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
