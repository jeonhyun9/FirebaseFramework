using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;

public class DataUnit : BaseUnit<DataUnitModel<IBaseData>>
{
    [SerializeField]
    private TextMeshProUGUI labelId;

    [SerializeField]
    private TextMeshProUGUI labelNameId;

    [SerializeField]
    private TextMeshProUGUI labelIsInit;

    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;

    [SerializeField]
    private RectTransform contentsRect;

    [SerializeField]
    private int minWidth = 1920;

    private RectTransform rect;

    private bool isResized = false;

    public override void UpdateUI()
    {
        gameObject.name = Model.Data.NameId;

        labelId.SafeSetText(Model.Data.Id.ToString());
        labelNameId.SafeSetText(Model.Data.NameId.ToString());
        labelIsInit.SafeSetText(Model.Data.IsInit.ToString());
    }

    private void LateUpdate()
    {
        if (!isResized)
            Resize();
    }

    //화면 범위 넘어갈만큼 유닛 크기가 커질경우..
    private void Resize()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        Size = rect.sizeDelta = contentsRect.sizeDelta = GetContentSize();

        OnResize?.Invoke(Size);

        isResized = true;
    }

    private Vector2 GetContentSize()
    {
        int childCount = gridLayoutGroup.transform.childCount;

        if (childCount == 0)
            return Vector2.zero;

        float width = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float height = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = gridLayoutGroup.transform.GetChild(i) as RectTransform;
            if (child == null || !child.gameObject.activeSelf) continue;

            float childWidth = child.rect.width + gridLayoutGroup.spacing.x;
            float childHeight = child.rect.height + gridLayoutGroup.spacing.y;
            width += childWidth;
            height = Mathf.Max(height, childHeight);
        }

        return new Vector2(Mathf.Max(width, minWidth), height);
    }
}
