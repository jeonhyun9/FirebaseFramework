using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseController<T,V> where T : BaseView where V : IBaseViewModel
{
    private string viewName;
    protected string ViewPrefabPath => $"{PathDefine.PrefabResourcesPath}/{viewName.Replace("View","")}/{viewName}";

    protected T View { get; private set; }

    protected V Model { get; private set; }

    protected Transform UITransform => UIManager.Instance.UITransform;

    /// <summary> 프리팹 네이밍은 View와 동일하게.. ex)DataBoardController => DataBoardView </summary>
    protected abstract string GetViewPrefabName();

    protected abstract void Enter();

    public void SetModel(V model)
    {
        Model = model;
    }

    public void StartProcess()
    {
        ProcessAsync().Forget();
    }

    public async UniTask ProcessAsync()
    {
        if (Model == null)
        {
            Logger.Error("Model is not initialized");
            return;
        }

        Enter();

        bool loadResult = await LoadViewAsync();

        if (!loadResult)
            return;

        View.Hide();

        await View.UpdateViewAsync();

        View.Show();

        await View.ShowAsync();
    }

    private async UniTask<bool> LoadViewAsync()
    {
        viewName = GetViewPrefabName();

        GameObject prefab = await AddressableManager.Instance.InstantiateAsync(viewName);
        
        if (prefab == null)
        {
            Logger.Null("ViewPrefabPath");
            return false;
        }

        View = Object.Instantiate(prefab, UITransform).GetComponent<T>();

        if (View == null)
        {
            Logger.Null("View");
            return false;
        }

        View.SetModel(Model);

        return true;
    }
}
