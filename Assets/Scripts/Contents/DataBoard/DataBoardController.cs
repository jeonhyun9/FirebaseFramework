using System.Collections;
using System.Collections.Generic;
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
    }

    public void AddModelList<T>(T[] datas) where T : IBaseData
    {
        List<DataUnitModel> dataUnitModelList = new (datas.Length);

        foreach(T data in datas)
        {
            DataUnitModel model = new(data);
            dataUnitModelList.Add(model);
        }

        Model.SetDataBoardUnitList(typeof(T), dataUnitModelList);
    }
}
