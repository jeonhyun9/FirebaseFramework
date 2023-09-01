#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneManager : BaseManager<SceneManager>
{
    public SceneType CurrentScene { get; private set; } = SceneType.RootScene;

    public async UniTask ChangeSceneAsync(SceneType scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        Logger.Log($"Change Scene :: {scene}");

        await AddressableManager.Instance.LoadSceneAsync(scene, mode);

        CurrentScene = scene;
    }

    public async UniTaskVoid ChangeSceneAsyncForget(SceneType scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        ChangeSceneAsync(scene, mode).Forget();
    }
}