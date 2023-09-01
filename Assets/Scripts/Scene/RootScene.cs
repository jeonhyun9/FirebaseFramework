using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RootScene : BaseScene
{
    public override SceneType SceneType => SceneType.RootScene;

    private SimpleTextUnitModel simpleTextUnitModel;
    private SimpleTextUnit simpleTextUnit;

    private void Awake()
    {
        InitializeGame().Forget();
    }

    private async UniTask InitializeGame()
    {
        //상태를 보여줄 기본 텍스트 UI 로드 (From Resources)
        await LoadSimpleText();

        //파이어베이스에서 어드레서블 빌드 로드.. 시퀀스 갱신 때마다 상태 표시
        bool result = await AddressableManager.Instance.LoadAddressableAsync(OnChangeAddressableSequenceCallback);

        //어드레서블 씬 전환
        if (result)
            await SceneManager.Instance.ChangeSceneAsync(SceneType.LoadingScene);
    }

    private async UniTask LoadSimpleText()
    {
        //어드레서블 로드 전이므로 Resources에서 가져온다.
        simpleTextUnit = await UIManager.Instance.LoadSimpleUI<SimpleTextUnit>(NameDefine.SimpleTextUnitName, false);

        if (simpleTextUnit != null)
        {
            simpleTextUnitModel = new SimpleTextUnitModel();
            simpleTextUnitModel.SetAnchor(SimpleTextUnitModel.Anchor.MiddleCenter);
            simpleTextUnitModel.SetText(AddressableManager.Instance.CurrentSequenceMessage);

            simpleTextUnit.SetModel(simpleTextUnitModel);
            simpleTextUnit.Show();
        }
        else
        {
            Logger.Null("simpleTextUnit");
        }
    }

    private void OnChangeAddressableSequenceCallback()
    {
        if (simpleTextUnit != null)
        {
            simpleTextUnitModel.SetText(AddressableManager.Instance.CurrentSequenceMessage);
            simpleTextUnit.Refresh();
        }
        else
        {
            Logger.Null("simpleTextUnit");
        }
    }
}
