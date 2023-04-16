using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseUnit<T> : MonoBehaviour where T : IBaseUnitModel
{
    public T Model { get; private set; }
    public void SetModel(T model)
    {
        Model = model;
    }

    public Vector2 Size { get; protected set; }

    public virtual void UpdateUI() { }

    public Action<Vector2> OnResize { get; protected set; }

    public void Show()
    {
        gameObject.SafeSetActive(true);
    }

    public void SetOnResize(Action<Vector2> func)
    {
        OnResize = func;
    }
}
