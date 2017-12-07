using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RTSizeListener : MonoBehaviour
{

    public Action<Vector2, RectTransform, RectTransform> onTargetSizeChange;

    public bool AutoRefresh = true;
    public RectTransform Tartget;

    private RectTransform _rt;

    private void OnEnable()
    {
        _rt = GetComponent<RectTransform>();

        refresh();
    }

    private void OnDestroy()
    {
        Tartget = null;
        _rt = null;
    }

    private Vector2 _targetWHCache;
    private void Update()
    {
        if (AutoRefresh) refresh();
    }

    private void refresh()
    {
        if (!Tartget) return;

        Vector2 wh = new Vector2(Tartget.rect.width, Tartget.rect.height);
        if (_targetWHCache != wh)
        {
            if(onTargetSizeChange != null) onTargetSizeChange(wh, _rt, Tartget);
        }
    }

}
