using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUnitModel
{
    public IBaseData Data { get; private set; }

    public DataUnitModel(IBaseData data)
    {
        Data = data;
    }
}
