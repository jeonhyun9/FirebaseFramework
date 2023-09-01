using Cysharp.Threading.Tasks;

public class GameManager : BaseMonoManager<GameManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        InitializeGame().Forget();
    }

    private async UniTask InitializeGame()
    {
        //파이어베이스에서 어드레서블 빌드 로드
        bool result = await AddressableManager.Instance.LoadAddressableAsync();

        //어드레서블 씬 전환
        if (result)
            await SceneManager.Instance.ChangeSceneAsync(SceneType.LoadingScene);
    }
}