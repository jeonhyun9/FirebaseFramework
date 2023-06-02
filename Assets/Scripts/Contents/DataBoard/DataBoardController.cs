using System;

public class DataBoardController : BaseController<DataBoardView,DataBoardViewModel>
{
    protected override string GetViewPrefabName()
    {
        return nameof(DataBoardController).Replace("Controller","View");
    }

    protected override void Enter()
    {
        Model.SetOnClickType(OnClickType);
        Model.SetPropertyNames(new string[] { "Id", "NameId", "IsInit" });
    }

    public void OnClickType(string value)
    {
        Type type = Type.GetType(value);

        if (type != null)
            Model.SetCurrentType(type);

        View.UpdateUnitItem();
        //View.UpdatePropertyNames();
    }
}
