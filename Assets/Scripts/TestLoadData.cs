using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TestLoadData : MonoBehaviour
{
    private enum LoadDataType
    {
        LocalPath,
    }

    [SerializeField]
    LoadDataType loadDataType;

    private void Awake()
    {
        LoadDataFromJson();
    }

    private void LoadDataFromJson()
    {
        switch (loadDataType)
        {
            case LoadDataType.LocalPath:
                LoadDataFromLocalJson();
                break;

            default:
                LoadDataFromLocalJson();
                break;
        }
    }

    private void LoadDataFromLocalJson()
    {
        DataContainerManager.Instance.AddDataContainerFromLocalJson<DataHumanContainer>();
        DataContainerManager.Instance.AddDataContainerFromLocalJson<DataAnimalContainer>();

        ShowLog();
    }

    private void ShowLog()
    {
        DataHuman human = DataContainerManager.Instance.GetDataContainer<DataHumanContainer>().GetById(1);
        Debug.Log($"Id가 1인 DataHuman {human.NameId}");

        DataAnimal animal = DataContainerManager.Instance.GetDataContainer<DataAnimalContainer>().GetById(3);
        Debug.Log($"Id가 3인 DataAnimal {animal.NameId}");
    }
}
