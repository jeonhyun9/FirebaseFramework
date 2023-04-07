using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataBoardViewModel
{
    public Dictionary<Type, List<DataUnitModel>> DicDataBoardUnitList { get; private set; } = new();

    public void SetDataBoardUnitList(Type type, List<DataUnitModel> list)
    {
        DicDataBoardUnitList.Add(type, list);
    }
}
