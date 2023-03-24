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
        ShowLog();
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
#if UNITY_EDITOR
        DataContainerManager.Instance.AddDataContainerFromLocalJson<DataHumanContainer>();
        DataContainerManager.Instance.AddDataContainerFromLocalJson<DataAnimalContainer>();
#endif
    }

    private void ShowLog()
    {
        DataHuman human = DataContainerManager.Instance.GetDataContainer<DataHumanContainer>().GetById(1);
        Debug.Log($"Id�� 1�� DataHuman : {human.NameId}");

        DataHuman winter = DataContainerManager.Instance.GetDataContainer<DataHumanContainer>().GetById(2);
        Debug.Log($"Id�� 2�� DataHuman {human.NameId}�� �� : {winter.Pet.NameId}");

        DataAnimal animal = DataContainerManager.Instance.GetDataContainer<DataAnimalContainer>().GetById(3);
        Debug.Log($"Id�� 3�� DataAnimal : {animal.NameId}");
    }
}
