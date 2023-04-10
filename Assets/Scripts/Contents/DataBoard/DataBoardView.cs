using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataBoardView : MonoBehaviour
{
    [SerializeField]
    UIUnitList unitItemList;

    public readonly DataBoardViewModel Model;

    public DataBoardView(DataBoardViewModel model)
    {
        Model = model;
    }

    public void ShowDataBoard()
    {
        unitItemList.AddUnits(Model.CurrentModelList.ToArray());
    }
}
