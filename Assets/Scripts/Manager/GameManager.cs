using Cysharp.Threading.Tasks;

public class GameManager : BaseManager<GameManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        InitializeGame().Forget();
    }

    private async UniTask InitializeGame()
    {
        //���̾�̽����� ��巹���� ���� �ε�
        bool result = await AddressableManager.Instance.LoadAddressableAsync();

        //��巹���� �� ��ȯ
        if (result)
            await SceneManager.Instance.ChangeSceneAsync(SceneState.LoadingScene);
    }
}