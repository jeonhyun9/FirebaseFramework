using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBoardController : BaseController<DataBoardView,DataBoardViewModel>
{
    private readonly Type[] useTypes;

    public DataBoardController(Type[] useTypesValue)
    {
        useTypes = useTypesValue;
    }

    protected override string GetViewPrefabName()
    {
        return nameof(DataBoardController).Replace("Controller","View");
    }

    protected override DataBoardViewModel CreateModel()
    {
        DataBoardViewModel model = new();
        model.SetUseTypes(useTypes);
        model.SetOnClickType(OnClickType);

        return model;
    }

    public void OnClickType(string value)
    {
        Type type = Type.GetType(value);

        if (type != null)
            Model.SetCurrentType(type);
    }
}
