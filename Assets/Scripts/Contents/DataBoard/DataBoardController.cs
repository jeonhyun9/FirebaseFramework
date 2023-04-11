using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBoardController : BaseController
{
    private DataBoardViewModel Model => GetModel<DataBoardViewModel>();

    private readonly Type[] useTypes;

    public DataBoardController(Type[] useTypesValue)
    {
        useTypes = useTypesValue;
    }

    public override void InitContentsName()
    {
        ContentsName = nameof(DataBoardController).Replace("Controller","");
    }

    public override void InitModel()
    {
        Model.SetUseTypes(useTypes);
        Model.SetOnClickType(OnClickType);
    }

    public void OnClickType(string value)
    {
        Type type = Type.GetType(value);

        if (type != null)
            Model.SetCurrentType(type);
    }
}
