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

    public virtual void Refresh() { }

    public virtual void Show()
    {
        gameObject.SafeSetActive(true);
    }
}
