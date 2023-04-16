using System.Collections.Generic;
using System;
using System.Linq;

public class DataBoardViewModel : IBaseViewModel
{
    public List<Type> DataBoardTypeList { get; private set; } = new();   
    
    public Dictionary<Type, List<DataUnitModel<IBaseData>>> DicDataBoardUnitModelList { get; private set; } = new();

    public Type CurrentType { get; private set; }

    public List<DataUnitModel<IBaseData>> CurrentModelList => DicDataBoardUnitModelList[CurrentType];

    public string[] PropertyNames { get; private set; }

    public Action<string> OnClickType { get; private set; }

    public void SetUseTypes(Type[] useTypes)
    {
        if (useTypes.Length == 0)
            return;

        CurrentType = useTypes[0];
        foreach (Type type in useTypes)
        {
            switch (type)
            {
                case Type t when t == typeof(DataAnimal):
                    AddDataBoardUnitModelList(DataManager.Instance.GetAllData<DataAnimal>());
                    break;

                case Type t when t == typeof(DataHuman):
                    AddDataBoardUnitModelList(DataManager.Instance.GetAllData<DataHuman>());
                    break;
            }
        }
    }

    public void SetPropertyNames(string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }

    private void AddDataBoardUnitModelList<T>(T[] datas) where T : IBaseData
    {
        List<DataUnitModel<IBaseData>> dataUnitModelList = new(datas.Length);

        foreach (IBaseData data in datas)
        {
            DataUnitModel<IBaseData> model = new(data);
            dataUnitModelList.Add(model);
        }

        DataBoardTypeList.Add(typeof(T));
        DicDataBoardUnitModelList.Add(typeof(T), dataUnitModelList);
    }

    public List<DataUnitModel<T>> GetModelList<T>() where T : IBaseData
    {
        return DicDataBoardUnitModelList[typeof(T)].OfType<DataUnitModel<T>>().ToList();
    }

    public void SetCurrentType(Type value)
    {
        CurrentType = value;
    }

    public void SetOnClickType(Action<string> onClickType)
    {
        OnClickType = onClickType;
    }
}
