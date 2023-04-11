using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    [SerializeField]
    private Transform uiTransform;

    [SerializeField]
    private Camera uiCamera;

    public Transform UITransform => uiTransform;

    public Camera UICamera => uiCamera;

    public void CreateUI<T>(T baseController) where T : BaseController
    {
        baseController.Process(UITransform).Forget();
    }
}
