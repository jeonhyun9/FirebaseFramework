using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit<T> : MonoBehaviour where T : IBaseUnitModel
{
    public T Model { get; private set; }
    public void SetModel(T model) 
    {
        Model = model;
    }

    public virtual void UpdateUI() { }

    public void Show()
    {
        gameObject.SafeSetActive(true);
    }
}
