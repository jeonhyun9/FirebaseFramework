using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class DataBoardView : BaseView
{
    [SerializeField]
    private UIUnitList unitItemList;

    public DataBoardViewModel Model => GetModel<DataBoardViewModel>();

    public override async UniTask ShowAsync()
    {
        if (unitItemList == null)
            return;

        await unitItemList.AddUnits(Model.CurrentModelList.ToArray());

        unitItemList.gameObject.SetActive(true);
    }
}
