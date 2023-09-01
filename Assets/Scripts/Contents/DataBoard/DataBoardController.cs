using System;

public class DataBoardController : BaseController<DataBoardView,DataBoardViewModel>
{
    public override UIType UIType => UIType.DataBoardView; 

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
