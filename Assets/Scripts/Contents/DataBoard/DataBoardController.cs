using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBoardController
{
    private string PrefabName => typeof(DataBoardController).Name.Replace("Controller", "View");

    private readonly DataBoardView View;

    private DataBoardViewModel Model;

    public DataBoardController(DataBoardViewModel model)
    {
        Model = model;
        View = new(Model);
    }

    private void Init()
    {
        Logger.Log(View.Model.GetType().Name);
    }
}
