using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUnitModel<T> : IBaseUnitModel where T : IBaseData
{
    public readonly T Data;

    public DataUnitModel(T data)
    {
        Data = data;
    }
}
