using System.Collections.Generic;
using UnityEngine;

public class UIUnitItemList : MonoBehaviour
{
    [SerializeField]
    GameObject templateItem;

    [SerializeField]
    Transform contentsTransform;

    private int useCount = 0;
    private readonly List<GameObject> unitList = new();

    public void ResetUseCount()
    {
        useCount = 0;
    }

    public void AddUnitItemWithModels<T>(T[] models) where T : IBaseUnitModel
    {
        for (int i = 0; i < models.Length; i++)
        {
            useCount++;

            GameObject go;

            if (GetUnitListCount() < useCount)
            {
                go = Instantiate(templateItem, contentsTransform);
                go.SafeSetActive(false);
                unitList.Add(go);
            }
            else
            {
                go = GetUnit(i);
            }

            if (go == null)
                continue;

            SetUnitModel(go, models[i]);
        }

        ActiveOffNotUse();
    }

    private void SetUnitModel<T>(GameObject go, T model) where T : IBaseUnitModel
    {
        BaseUnit<T> unit = go.GetComponent<BaseUnit<T>>();

        if (unit == null)
        {
            unit.SetModel(model);
            unit.UpdateUI();
            unit.Show();
        }
        else
        {
            Logger.Null("unit");
        }
    }

    public GameObject GetUnit(int index)
    {
        if (index < 0 || index >= unitList.Count)
            return null;

        return unitList[index];
    }

    public int GetUnitListCount()
    {
        return unitList.Count;
    }

    public void ActiveOffNotUse()
    {
        for (int i = useCount; i < GetUnitListCount(); ++i)
            unitList[i].SetActive(false);
    }
}
