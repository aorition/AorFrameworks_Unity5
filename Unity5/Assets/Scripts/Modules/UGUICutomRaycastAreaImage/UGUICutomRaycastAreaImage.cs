using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// 注: 目前版本 只处理 合并和减去的逻辑. 暂时不考虑相交等其他情况
/// 
/// 需要配合UGUICustomRaycastSubArea组件使用
/// 
/// </summary>


public class UGUICutomRaycastAreaImage : Image {

    public bool ShowImageGraphic = true;

    /// <summary>
    /// 通过编辑器挂在实现
    /// </summary>
    public List<UGUICustomRaycastSubArea> SubArea2DList = new List<UGUICustomRaycastSubArea>();

    private List<Collider2D> _conbineList = new List<Collider2D>();
    private List<Collider2D> _subtractList = new List<Collider2D>();

    /// <summary>
    /// 当此Image组件的RectTransform.Rect改变时触发
    /// </summary>
    public event Action<Rect> OnRectTransformChangedRectSize;

    protected UGUICutomRaycastAreaImage()
    {
        useLegacyMeshGeneration = true;
    }

    public override void Rebuild(CanvasUpdate update)
    {
        base.Rebuild(update);
        _updateCollider2dFormRectTransform();
    }
    private UGUICustomRaycastSubArea _subTmp;
    private void _updateCollider2dFormRectTransform()
    {
        if (OnRectTransformChangedRectSize != null) OnRectTransformChangedRectSize(rectTransform.rect);
        //
        for (int i = 0; i < SubArea2DList.Count; i++)
        {
            _subTmp = SubArea2DList[i];
            if(_subTmp && _subTmp.OnImageRectSizeChanged != null)
            {
                _subTmp.OnImageRectSizeChanged(rectTransform.rect);
            }
            _subTmp = null;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (ShowImageGraphic)
        {
            base.OnPopulateMesh(vh);
        }
        else
        {
            vh.Clear();
        }
    }



    protected override void Awake()
    {
        base.Awake();
        //
        if(SubArea2DList.Count > 0)
        {
            for (int i = 0; i < SubArea2DList.Count; i++)
            {
                UGUICustomRaycastSubArea subTmp = SubArea2DList[i];
                if(subTmp.Type == UGUICustomRaycastSubAreaType.combine)
                {
                    _conbineList.Add(subTmp.Collider2D);
                }
                if (subTmp.Type == UGUICustomRaycastSubAreaType.subtract)
                {
                    _subtractList.Add(subTmp.Collider2D);
                }
            }
        }

    }

    protected override void Start()
    {
        base.Start();
        //Start的时候也需要调用一次,来保证位置对的上
        _updateCollider2dFormRectTransform();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _conbineList.Clear();
        _subtractList.Clear();
        SubArea2DList.Clear();
        OnRectTransformChangedRectSize = null;
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (!this.raycastTarget) return false;

        bool result = false;
        int i, len = _conbineList.Count;
        for (i = 0; i < len; i++)
        {
            result = _conbineList[i].OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
        }
        //所有的合并单元都没有点到,则判定没点到
        if (!result) return false;
        len = _subtractList.Count;
        for (i = 0; i < len; i++)
        {
            result = !_subtractList[i].OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
            //如果此时有一个被点到了,则判定没点到
            if (!result) return false;
        }
        // return Collider2D.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
        return result;
    }


}