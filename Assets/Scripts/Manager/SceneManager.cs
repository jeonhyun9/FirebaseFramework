#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneManager : BaseStaticManager<SceneManager>
{
    public SceneState State { get; private set; }

    public async UniTask ChangeSceneAsync(SceneState scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        await AddressableManager.Instance.LoadSceneAsync(scene, mode);
    }

    public async UniTaskVoid ChangeSceneAsyncForget(SceneState scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        ChangeSceneAsync(scene, mode).Forget();
    }
}