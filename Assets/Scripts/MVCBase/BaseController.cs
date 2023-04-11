using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController
{
    public string ContentsName { get; protected set; }
    public string PrefabPath => $"Prefab/UI/{ContentsName}/{ContentsName}View";

    protected BaseView BaseView;

    public T GetView<T>() where T : BaseView
    {
        return (T)BaseView;
    }

    public BaseViewModel BaseModel { get; protected set; } = new();

    public T GetModel<T>() where T : BaseViewModel
    {
        return (T)BaseModel;
    }

    /// <summary> Model 할당 </summary>
    public abstract void InitModel();

    /// <summary> ContentsName 지정.. ex)DataBoardController => DataBoard </summary>
    public abstract void InitContentsName();

    public async UniTaskVoid Process(Transform viewTransform)
    {
        await LoadAsync(viewTransform);
        await ShowAsync();
    }

    private async UniTask LoadAsync(Transform viewTransform)
    {
        InitModel();

        if (BaseView == null)
        {
            GameObject prefab = (GameObject)await Resources.LoadAsync<GameObject>(PrefabPath);

            if (prefab == null)
            {
                Logger.Error("prefab is null");
                return;
            }

            BaseView = Object.Instantiate(prefab, viewTransform).GetComponent<BaseView>();
            BaseView.SetModel(BaseModel);
        }
    }

    private async UniTask ShowAsync()
    {
        await BaseView.ShowAsync();
    }
}
