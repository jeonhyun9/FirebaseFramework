#pragma warning disable 1998

using System;
using System.IO;
using UnityEngine;

public abstract class BaseDataLoader
{
    public enum State
    {
        None,
        LoadVersion,
        LoadJsonList,
        LoadJson,
        Done,
        Fail,
    }

    public string CurrentProgressString
    {
        get
        {
            switch (CurrentState)
            {
                case State.LoadVersion:
                    return "Loading Version...";
                case State.LoadJsonList:
                    return "Loading JsonList...";
                case State.LoadJson:
                    return "Loading Json...";
                case State.Done:
                    return "Done!";
                case State.Fail:
                    return "Fail!";
                default:
                    return null;
            }
        }
    }
    public float CurrentProgressValue { get; protected set; } = 0f;

    public State CurrentState { get; protected set; }

    protected Action OnLoadData { get; private set; }
    protected Action<State> OnChangeState;

    public void SetOnLoadData(Action action)
    {
        OnLoadData = action;
    }

    public void SetOnChangeState(Action<State> action)
    {
        OnChangeState = action;
    }

    protected bool AddDataContainerToManager(string fileName, string json)
    {
        fileName = Path.GetFileNameWithoutExtension(fileName);

        return DataManager.Instance.AddDataContainer(fileName, json);
    }

    protected void ChangeState(State state)
    {
        CurrentState = state;

        if (OnChangeState != null)
            OnChangeState(state);
    }
}
