using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataUnit : BaseUnit<DataUnitModel<IBaseData>>
{
    [SerializeField]
    TextMeshProUGUI labelId;

    [SerializeField]
    TextMeshProUGUI labelNameId;

    public override void UpdateUI()
    {
        gameObject.name = Model.Data.NameId;

        labelId.SafeSetText(Model.Data.Id.ToString());
        labelNameId.SafeSetText(Model.Data.NameId.ToString());
    }
}
