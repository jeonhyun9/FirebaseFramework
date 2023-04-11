using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseView : MonoBehaviour
{
    public enum AsyncType
    {
        Async,
        NonAsync,
    }

    public IBaseViewModel BaseModel { get; private set; }

    public void SetModel(IBaseViewModel model)
    {
        BaseModel = model;
    }

    public T GetModel<T>() where T : IBaseViewModel
    {
        return (T)BaseModel;
    }

    public virtual async UniTask UpdateViewAsync()
    {
        await UniTask.CompletedTask;
    }

    public virtual async UniTask ShowAsync()
    {
        await UniTask.CompletedTask;
    }

    public void Show()
    {
        gameObject.SafeSetActive(true);
    }

    public void Hide()
    {
        gameObject.SafeSetActive(false);
    }
}
