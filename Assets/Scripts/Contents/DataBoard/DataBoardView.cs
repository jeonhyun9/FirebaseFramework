using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataBoardView : MonoBehaviour
{
    [SerializeField]
    UIUnitList unitItemList;

    public DataBoardViewModel Model { get; private set; }

    public void Initialize(DataBoardViewModel model)
    {
        Model = model;
        CreateUIUnitList();
    }

    public void CreateUIUnitList()
    {
        if (unitItemList != null)
            unitItemList.AddUnits(Model.CurrentModelList.ToArray());
    }

    public void Show()
    {
        unitItemList.gameObject.SetActive(true);
    }
}
