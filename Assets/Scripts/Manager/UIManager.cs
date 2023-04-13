using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    [SerializeField]
    private Transform uiTransform;

    [SerializeField]
    private Camera uiCamera;

    public Transform UITransform => uiTransform;

    public Camera UICamera => uiCamera;
}
