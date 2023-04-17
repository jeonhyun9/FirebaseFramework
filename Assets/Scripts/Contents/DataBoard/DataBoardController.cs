using System;

public class DataBoardController : BaseController<DataBoardView,DataBoardViewModel>
{
    private readonly Type[] useTypes;

    public DataBoardController(Type[] useTypesValue)
    {
        useTypes = useTypesValue;
    }

    protected override string GetViewPrefabName()
    {
        return nameof(DataBoardController).Replace("Controller","View");
    }

    protected override DataBoardViewModel CreateModel()
    {
        DataBoardViewModel model = new();
        model.SetUseTypes(useTypes);
        model.SetOnClickType(OnClickType);
        model.SetPropertyNames(new string[] { "Id", "NameId", "IsInit" });

        return model;
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
