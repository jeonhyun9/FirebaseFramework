using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataUnit<T> : BaseUnit<DataUnitModel<T>> where T : IBaseData
{
    [SerializeField]
    TextMeshProUGUI labelId;

    [SerializeField]
    TextMeshProUGUI labelNameId;

    public override void UpdateUI()
    {
        //labelId.ga
    }
}
