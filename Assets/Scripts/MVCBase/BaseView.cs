using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseView : MonoBehaviour
{
    public BaseViewModel BaseModel { get; private set; }

    public void SetModel(BaseViewModel model)
    {
        BaseModel = model;
    }

    public T GetModel<T>() where T : BaseViewModel
    {
        return (T)BaseModel;
    }

    public abstract UniTask ShowAsync();
}
