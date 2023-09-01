#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneManager : BaseManager<SceneManager>
{
    public SceneType State { get; private set; }

    public async UniTask ChangeSceneAsync(SceneType scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        await AddressableManager.Instance.LoadSceneAsync(scene, mode);
    }

    public async UniTaskVoid ChangeSceneAsyncForget(SceneType scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        ChangeSceneAsync(scene, mode).Forget();
    }
}