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
        //���¸� ������ �⺻ �ؽ�Ʈ UI �ε� (From Resources)
        await LoadSimpleText();

        //���̾�̽����� ��巹���� ���� �ε�.. ������ ���� ������ ���� ǥ��
        bool result = await AddressableManager.Instance.LoadAddressableAsync(OnChangeAddressableSequenceCallback);

        //��巹���� �� ��ȯ
        if (result)
            await SceneManager.Instance.ChangeSceneAsync(SceneType.LoadingScene);
    }

    private async UniTask LoadSimpleText()
    {
        //��巹���� �ε� ���̹Ƿ� Resources���� �����´�.
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
