using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DataBoardViewModel
{
    public Dictionary<Type, List<DataUnitModel<IBaseData>>> DicDataBoardUnitList { get; private set; } = new();

    public List<Type> DataBoardTypeList { get; private set; } = new();

    public Type CurrentType { get; private set; }

    public List<DataUnitModel<IBaseData>> CurrentModelList => DicDataBoardUnitList[CurrentType];

    public Action<string> OnClickType { get; private set; }

    public void SetDataBoardUnitList(Type type, List<DataUnitModel<IBaseData>> list)
    {
        DataBoardTypeList.Add(type);
        DicDataBoardUnitList.Add(type, list);
    }

    public List<DataUnitModel<T>> GetModelList<T>() where T : IBaseData
    {
        return DicDataBoardUnitList[typeof(T)].OfType<DataUnitModel<T>>().ToList();
    }

    public void SetCurrentType(Type value)
    {
        CurrentType = value;
    }

    public void SetOnClickType(Action<string> onClickType)
    {
        OnClickType = onClickType;
    }
}
