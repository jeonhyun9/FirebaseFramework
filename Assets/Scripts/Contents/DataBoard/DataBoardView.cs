using Cysharp.Threading.Tasks;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DataBoardView : BaseView
{
    [SerializeField]
    private UIUnitItemList unitItemList;

    [SerializeField]
    private UIUnitItemList propertyNamesItemList;

    [SerializeField]
    private TMP_Dropdown dropdownType;

    private List<TMP_Dropdown.OptionData> options;
    public DataBoardViewModel Model => GetModel<DataBoardViewModel>();

    public async override UniTask UpdateViewAsync()
    {
        if (options == null)
            InitDropdownOptions();

        UpdatePropertyNames();
        UpdateUnitItem();

        await UniTask.CompletedTask;
    }

    private void InitDropdownOptions()
    {
        if (dropdownType)
        {
            options = new(Model.DataBoardTypeList.Count);

            options.AddRange(Model.DataBoardTypeList.Select(type => new TMP_Dropdown.OptionData(type.Name)));

            dropdownType.AddOptions(options);
        }
    }

    public void UpdateUnitItem()
    {
        if (unitItemList == null)
        {
            Logger.Null("unitItemList");
            return;
        }

        unitItemList.ResetUseCount();
        unitItemList.AddUnitItemWithModels(Model.CurrentModelList.ToArray());
        propertyNamesItemList.ActiveOffNotUse();
    }

    public void UpdatePropertyNames()
    {
        if (propertyNamesItemList == null)
        {
            Logger.Null("propertyNamesItemList");
            return;
        }

        propertyNamesItemList.ResetUseCount();

        for(int i = 0; i < Model.PropertyNames.Length; i++)
        {
            TextMeshProUGUI label = propertyNamesItemList.AddUnitItem<TextMeshProUGUI>(i);
            label.SafeSetText(Model.PropertyNames[i]);
            label.gameObject.SafeSetActive(true);
        }

        propertyNamesItemList.ActiveOffNotUse();
    }

    //매개변수로 클릭한 옵션의 index를 받음
    public void OnClickType(int index)
    {
        if (options[index]?.text == null)
            return;

        Model.OnClickType(options[index].text);
    }
}
