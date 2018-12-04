using UnityEngine;
using System;

public enum UGUICustomRaycastSubAreaType {
    combine,
    subtract
}

/// <summary>
/// 此脚本只为UGUICutomRaycastAreaImage提供数据组装
/// </summary>
public class UGUICustomRaycastSubArea : MonoBehaviour
{
    public UGUICustomRaycastSubAreaType Type;
    public Collider2D Collider2D;
    [Space(10)]
    public bool BoxCollider2DSizeBindByRectTransform;

    public Action<Rect> OnImageRectSizeChanged;

    //private void OnEnable()
    //{
    //    if (BoxCollider2DSizeBindByRectTransform) {
    //       // _BoxCollider2DSizeBindByRectTransformFunc(new Rect());
    //       // OnImageRectSizeChanged += _BoxCollider2DSizeBindByRectTransformFunc;
    //      //  _isBindedAction = true;
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (_isBindedAction)
    //    {
    //       // OnImageRectSizeChanged -= _BoxCollider2DSizeBindByRectTransformFunc;
    //       // _isBindedAction = false;
    //    }
    //}

    private void Update()
    {
        //强制RectTransform的pivot为中点
        if (_rectTransform)
        {
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);

            if (BoxCollider2DSizeBindByRectTransform)
            {
                BoxCollider2D bc = Collider2D as BoxCollider2D;
                if (bc)
                {
                    bc.size = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
                }
            }
        }
    }

    private RectTransform m_rectTransform;
    protected RectTransform _rectTransform {
        get {
            if (!m_rectTransform) m_rectTransform = GetComponent<RectTransform>();
            return m_rectTransform;
        }
    }

    private bool _isBindedAction = false;
    private void _BoxCollider2DSizeBindByRectTransformFunc(Rect rect)
    {
        if (!Collider2D) return;
        BoxCollider2D bc = Collider2D as BoxCollider2D;
        if (bc)
        {
            if (!_rectTransform) return;
            bc.size = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        }
    }

}
