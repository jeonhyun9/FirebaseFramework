using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseController<T,V> where T : BaseView where V : IBaseViewModel
{
    private string viewName;
    protected string ViewPrefabPath => $"{PathDefine.PrefabResourcesPath}/{viewName.Replace("View","")}/{viewName}";

    protected T View { get; private set; }

    protected V Model { get; private set; }

    protected Transform UITransform => UIManager.Instance.UITransform;

    /// <summary> Model 생성 </summary>
    protected abstract V CreateModel();

    /// <summary> 프리팹 네이밍은 View와 동일하게.. ex)DataBoardController => DataBoardView </summary>
    protected abstract string GetViewPrefabName();

    public void StartProcess()
    {
        ProcessAsync().Forget();
    }

    public async UniTask ProcessAsync()
    {
        InitModel();

        bool loadResult = await LoadViewAsync();

        if (!loadResult)
            return;

        View.Hide();

        await View.UpdateViewAsync();

        View.Show();

        await View.ShowAsync();
    }

    private void InitModel()
    {
        Model = CreateModel();
    }

    private async UniTask<bool> LoadViewAsync()
    {
        viewName = GetViewPrefabName();

        GameObject prefab = (GameObject)await Resources.LoadAsync<GameObject>(ViewPrefabPath);
        
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
