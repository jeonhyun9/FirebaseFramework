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

        //�񵿱� �����ϸ� �̰��� return����
        await UniTask.CompletedTask;
    }
}
