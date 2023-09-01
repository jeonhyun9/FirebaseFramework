using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIManager : BaseMonoManager<UIManager>
{
    [SerializeField]
    private Transform uiTransform;

    [SerializeField]
    private Camera uiCamera;

    public Transform UITransform => uiTransform;

    public Camera UICamera => uiCamera;

    public GameObject CurrentView { get; private set; }

    public ViewType CurrentViewType { get; private set; }

    public async UniTask<T> LoadView<T>(ViewType viewType) where T : BaseView
    {
        GameObject prefab = await AddressableManager.Instance.InstantiateAsync(viewType.ToString(), UITransform);

        if (prefab == null)
        {
            Logger.Null($"{viewType}");
            return null;
        }

        if (CurrentView != null)
            Destroy(CurrentView);

        CurrentView = prefab;
        CurrentViewType = viewType;

        return prefab.GetComponent<T>();
    }
}
