using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : BaseMonoManager<UIManager>
{
    [SerializeField]
    private Transform uiTransform;

    [SerializeField]
    private Transform simpleUITransform;

    [SerializeField]
    private Camera uiCamera;

    public Transform UITransform => uiTransform;

    //잠깐 쓰고 버리는 용도 .. View 바꾸면 없어짐
    public Transform SimpleUITransform => simpleUITransform;

    public Camera UICamera => uiCamera;

    public GameObject CurrentView { get; private set; }

    public UIType CurrentUIType { get; private set; }

    private Stack<UIType> uiHistory = new Stack<UIType>();

    public async UniTask<T> LoadView<T>(UIType uiType) where T : BaseView
    {
        string name = uiType.ToString();

        GameObject prefab = await LoadPrefabUI(name, uiTransform);

        if (CurrentView != null)
            Destroy(CurrentView);

        ClearSimpleUI();

        CurrentView = prefab;
        CurrentUIType = uiType;

        uiHistory.Push(uiType);

        return prefab.GetComponent<T>();
    }

    public async UniTask<T> LoadSimpleUI<T>(string name, bool useAddressable = true) where T : Object
    {
        GameObject prefab = await LoadPrefabUI(name, simpleUITransform, useAddressable);

        return prefab.GetComponent<T>();
    }

    private async UniTask<GameObject> LoadPrefabUI(string name, Transform tr, bool useAddressable = true)
    {
        GameObject prefab = null;

        if (useAddressable)
        {
            prefab = await AddressableManager.Instance.InstantiateAsync(name, tr);
        }
        else
        {
            prefab = await Resources.LoadAsync($"{PathDefine.DefaultPrefabPath}/{name}") as GameObject;

            if (prefab != null)
                prefab = Instantiate(prefab, tr);
        }

        if (prefab == null)
        {
            Logger.Null(name);
            return null;
        }

        return prefab;
    }

    private void ClearSimpleUI()
    {
        foreach (Transform child in simpleUITransform)
        {
            Logger.Log($"Clear Simple UI {child.gameObject.name}");
            Destroy(child.gameObject);
        }
    }
}
