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
    private TMP_Dropdown dropdownType;

    private List<TMP_Dropdown.OptionData> options;
    public DataBoardViewModel Model => GetModel<DataBoardViewModel>();

    public async override UniTask UpdateViewAsync()
    {
        if (dropdownType)
        {
            options = new (Model.DataBoardTypeList.Count);

            options.AddRange(Model.DataBoardTypeList.Select(type => new TMP_Dropdown.OptionData(type.Name)));

            dropdownType.AddOptions(options);
        }

        UpdateUnitItem();

        //비동기 사용안하면 이것을 return하자
        await UniTask.CompletedTask;
    }

    //현재 선택된 타입으로 업데이트
    public void UpdateUnitItem()
    {
        if (unitItemList == null)
        {
            Logger.Null("unitItemList");
            return;
        }

        unitItemList.ResetUseCount();
        unitItemList.AddUnitItemWithModels(Model.CurrentModelList.ToArray());
    }

    //매개변수로 클릭한 옵션의 index를 받음
    public void OnClickType(int index)
    {
        if (options[index]?.text == null)
            return;

        Model.OnClickType(options[index].text);
    }
}
