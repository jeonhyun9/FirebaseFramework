using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUnitModel<T> : IBaseDataUnitModel where T : IBaseData
{
    public readonly T Data;

    public Action<Vector2> OnResize { get; private set; }

    public DataUnitModel(T data)
    {
        Data = data;
    }

    public void SetOnResize(Action<Vector2> func)
    {
        OnResize = func;
    }
}
