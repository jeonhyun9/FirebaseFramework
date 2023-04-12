using Cysharp.Threading.Tasks;
using System.Collections;
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

    public void AddUnits<T>(T[] datas) where T : IBaseUnitModel
    {
        for (int i = 0; i < datas.Length; i++)
        {
            useCount++;

            GameObject go;

            if (GetUnitCount() < useCount)
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
            {
                Logger.Null(go);
                continue;
            }

            SetUnitModel(go, datas[i]);
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
        if (index < 0 || index >= unitList.Count)
            return null;

        return unitList[index];
    }

    public int GetUnitCount()
    {
        return unitList.Count;
    }

    public void ActiveOffNotUse()
    {
        for (int i = useCount; i < GetUnitCount(); ++i)
            unitList[i].SetActive(false);
    }
}
