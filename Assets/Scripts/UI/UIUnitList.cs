using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnitList : MonoBehaviour
{
    [SerializeField]
    GameObject templateItem;

    [SerializeField]
    Transform scrollViewContentsTransform;

    private int useCount = 0;
    private List<GameObject> unitList = new();

    public void ResetUseCount()
    {
        useCount = 0;
    }

    public void AddUnits<T>(T[] datas) where T : IBaseUnitModel
    {
        for(int i = 0; i < datas.Length; i++)
        {
            useCount++;

            GameObject go;

            if (GetUnitCount() <= useCount)
            {
                go = Instantiate(templateItem, scrollViewContentsTransform);
            }
            else
            {
                go = GetUnit(i);
            }

            if (go == null)
            {
                Logger.Null(go);
                continue;
            }

            SetUnitModel(go, datas[i]);

            unitList.Add(go);
        }

        ActiveOffNotUse();
    }

    private void SetUnitModel<T>(GameObject go, T model) where T : IBaseUnitModel
    {
        BaseUnit<T> unit = go.GetComponent<BaseUnit<T>>();
        unit.SetModel(model);
        unit.UpdateUI();
        unit.Show();
    }

    public GameObject GetUnit(int index)
    {
        if (unitList == null)
            return null;

        if (index < 0)
            return null;

        if (index >= GetUnitCount())
            return null;

        return unitList[index];
    }

    public int GetUnitCount()
    {
        if (unitList == null)
            return 0;

        return unitList.Count;
    }

    public void ActiveOffNotUse()
    {
        for (int i = useCount; i < GetUnitCount(); ++i)
            unitList[i].SetActive(false);
    }
}
