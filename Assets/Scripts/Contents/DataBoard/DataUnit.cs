using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataUnit : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI labelId;

    [SerializeField]
    TextMeshProUGUI labelNameId;

    private DataUnitModel Model { get; }

    public DataUnit(DataUnitModel model)
    {
        Model = model;
    }

    public void SetUI()
    {
        labelId.SafeSetText(Model.Data.Id.ToString());
        labelNameId.SafeSetText(Model.Data.NameId);
    }

    public void Show()
    {
        gameObject.SafeSetActive(true);
    }
}
