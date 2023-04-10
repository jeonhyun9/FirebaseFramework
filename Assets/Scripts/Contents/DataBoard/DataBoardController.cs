using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataBoardController
{
    private string PrefabName => "Prefab/UI/DataBoard/DataBoardView";

    private DataBoardView View;

    protected DataBoardViewModel Model { get; private set; }

    public async UniTask Initialize(Type[] useTypes)
    {
        Model = new(useTypes);
        Model.SetOnClickType(OnClickType);

        if (View == null)
        {
            GameObject prefab = (GameObject)await Resources.LoadAsync<GameObject>(PrefabName);

            if (prefab == null)
            {
                Logger.Error("prefab is null");
                return;
            }

            View = UnityEngine.Object.Instantiate(prefab).GetComponent<DataBoardView>();
            View.Initialize(Model);
        }
    }

    public void OnClickType(string value)
    {
        Type type = Type.GetType(value);

        if (type != null)
            Model.SetCurrentType(type);
    }

    public void Show()
    {
        View.Show();
    }
}
