using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBoardController : MonoBehaviour
{
    private string PrefabName => typeof(DataBoardController).Name.Replace("Controller", "View");

    [SerializeField]
    private DataBoardView View;

    protected DataBoardViewModel Model { get; private set; }

    private void Awake()
    {
        Model = new();
        Model.SetOnClickType(OnClickType);
    }

    public void SetModelList<T>(IBaseData[] datas) where T : IBaseData
    {
        List<DataUnitModel<IBaseData>> dataUnitModelList = new(datas.Length);

        foreach (IBaseData data in datas)
        {
            DataUnitModel<IBaseData> model = new(data);
            dataUnitModelList.Add(model);
        }

        Model.SetDataBoardUnitList(typeof(T), dataUnitModelList);
    }

    public void OnClickType(string value)
    {
        Type type = Type.GetType(value);

        if (type != null)
            Model.SetCurrentType(type);
    }
}
