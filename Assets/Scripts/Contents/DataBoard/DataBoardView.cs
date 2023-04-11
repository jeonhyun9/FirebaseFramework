using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataBoardView : BaseView
{
    [SerializeField]
    private UIUnitItemList unitItemList;

    public DataBoardViewModel Model => GetModel<DataBoardViewModel>();

    public async override UniTask UpdateViewAsync()
    {
        if (unitItemList == null)
        {
            Logger.Null(unitItemList);
            return;
        }

        unitItemList.AddUnits(Model.CurrentModelList.ToArray());

        //비동기 사용안하면 이것을 return하자
        await UniTask.CompletedTask;
    }
}
