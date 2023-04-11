#pragma warning disable 1998

using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;

public abstract class BaseDataLoader : BaseViewModel
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

    protected Action<State> OnChangeState;

    protected Func<string, string, bool> OnLoadJson;

    public abstract UniTaskVoid LoadData();

    public void SetOnChangeState(Action<State> action)
    {
        OnChangeState = action;
    }

    public void SetOnLoadJson(Func<string, string, bool> function)
    {
        OnLoadJson = function;
    }

    protected void ChangeState(State state)
    {
        CurrentState = state;

        if (OnChangeState != null)
            OnChangeState(state);
    }
}
