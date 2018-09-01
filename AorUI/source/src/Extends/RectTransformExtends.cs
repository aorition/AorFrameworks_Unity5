using System;
using System.Collections.Generic;
using Framework.Extends;
using UnityEngine;

namespace Framework.UI
{

    public static class RectTransformExtends
    {

        public static void Dispose(this RectTransform t)
        {
            t.gameObject.Dispose();
        }

        public static void DisposeChildren(this RectTransform t)
        {
            t.transform.DisposeChildren();
        }

        /// <summary>
        /// 获取该RectTransform隶属的主Canvas
        /// </summary>
        public static Canvas FindRootCanvas(this RectTransform t)
        {
            Canvas c = t.GetComponent<Canvas>();
            if (c && c.isRootCanvas)
            {
                return c;
            }
            if (t.parent)
            {
                RectTransform parentRt = t.parent.GetComponent<RectTransform>();
                if (parentRt)
                {
                    return parentRt.FindRootCanvas();
                }
            }
            return null;
        }

        /// <summary>
        /// 对应Transform.Find方法返回指定节点的RectTransform;
        /// </summary>
        public static RectTransform FindRectTransform(this Transform t, string childName)
        {
            Transform sub = t.Find(childName);
            if (sub)
            {
                return sub.GetComponent<RectTransform>();
            }
            return null;
        }
        public static RectTransform FindRectTransform(this RectTransform t, string childName)
        {
            Transform sub = t.Find(childName);
            if (sub)
            {
                return sub.GetComponent<RectTransform>();
            }
            return null;
        }

        /// <summary>
        /// 归置为基本相对定位
        /// </summary>
        public static RectTransform ResetRect(this RectTransform rt)
        {
            rt.pivot = new Vector2(.5f, .5f);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);

            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;

            return rt;
        }

        /// <summary>
        /// 获取通过对pivot,anchorMin,anchorMax将anchoredPosition,sizeDelta参数转换成一组更容易理解的Rect(通常在Inspector中看到的前5个数据)
        /// </summary>
        public static RectTransform GetSmartRect(this RectTransform rt, out Vector3 smartAnchoredPos, out Vector2 smartSizeDelta)
        {
            if (rt.parent == null)
            {
                smartAnchoredPos = rt.anchoredPosition3D;
                smartSizeDelta = rt.sizeDelta;
                return rt;
            }

            bool absX = rt.anchorMin.x.Equals(rt.anchorMax.x);
            bool absY = rt.anchorMin.y.Equals(rt.anchorMax.y);

            float ax, ay, sx, sy;

            if (absX)
            {
                sx = rt.sizeDelta.x;
                ax = rt.anchoredPosition.x;
            }
            else
            {
                sx = -rt.offsetMax.x;
                ax = rt.offsetMin.x;
            }

            if (absY)
            {
                sy = rt.sizeDelta.y;
                ay = rt.anchoredPosition.y;
            }
            else
            {
                sy = rt.offsetMin.y;
                ay = -rt.offsetMax.y;
            }

            smartAnchoredPos = new Vector3(ax, ay, rt.anchoredPosition3D.z);
            smartSizeDelta = new Vector2(sx, sy);

            return rt;
        }

        /// <summary>
        /// 重设pivot数据但不改变当前UI位置/长宽
        /// </summary>
        public static RectTransform SmartPivot(this RectTransform rt, Vector2 pivot)
        {
            Vector3 smartAnchoredPos;
            Vector2 smartSizeDelta;
            rt.GetSmartRect(out smartAnchoredPos, out smartSizeDelta);
            rt.pivot = pivot;
            SmartResize(rt, smartAnchoredPos, smartSizeDelta);
            return rt;
        }

        /// <summary>
        /// 按Inspector设置习惯操作此RectTransform
        /// </summary>
        public static RectTransform SmartResize(this RectTransform rt, Vector3 smartAnchoredPos, Vector2 smartSizeDelta)
        {

            if (rt.parent == null) return rt;

            bool absX = rt.anchorMin.x.Equals(rt.anchorMax.x);
            bool absY = rt.anchorMin.y.Equals(rt.anchorMax.y);

            float ax, ay, sx, sy;

            if (absX)
            {
                sx = smartSizeDelta.x;
                ax = smartAnchoredPos.x;
            }
            else
            {
                sx = -smartAnchoredPos.x - smartSizeDelta.x;
                ax = sx * rt.pivot.x + smartAnchoredPos.x;
            }

            if (absY)
            {
                sy = smartSizeDelta.y;
                ay = smartAnchoredPos.y;
            }
            else
            {
                sy = -smartAnchoredPos.y - smartSizeDelta.y;
                ay = sy * rt.pivot.y + smartSizeDelta.y;
            }

            rt.sizeDelta = new Vector2(sx, sy);
            if (!smartAnchoredPos.z.Equals(0))
            {
                rt.anchoredPosition3D = new Vector3(ax, ay, smartAnchoredPos.z);
            }
            else
            {
                rt.anchoredPosition = new Vector2(ax, ay);
            }
            return rt;
        }
        /// <summary>
        /// 按Inspector设置习惯操作此RectTransform
        /// </summary>
        public static RectTransform SmartResize(this RectTransform rt, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, Vector3 smartAnchoredPos, Vector2 smartSizeDelta)
        {
            if (rt.parent == null) return rt;
            rt.pivot = pivot;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            SmartResize(rt, smartAnchoredPos, smartSizeDelta);
            return rt;
        }

        /// <summary>
        /// 绝对定位转换相对定位
        /// </summary>
        public static RectTransform convertAbsRTRelative(this RectTransform rt)
        {
            if (!rt.parent) return rt;

            Rect _rect = rt.rect;
            RectTransform _parentRT = rt.parent.GetComponent<RectTransform>();
            if (!_parentRT) return rt;

            //常规方式
            Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

            float xMin = (parentBase.x + rt.localPosition.x - _rect.width * rt.pivot.x) / _parentRT.rect.width;
            float yMin = (parentBase.y + rt.localPosition.y - _rect.height * rt.pivot.y) / _parentRT.rect.height;
            float xMax = (parentBase.x + rt.localPosition.x + _rect.width * (1 - rt.pivot.x)) / _parentRT.rect.width;
            float yMax = (parentBase.y + rt.localPosition.y + _rect.height * (1 - rt.pivot.y)) / _parentRT.rect.height;

            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            return rt;
        }

        /// <summary>
        /// 绝对定位转换相对定位(横向)
        /// </summary>
        public static RectTransform convertAbsRTRelativeH(this RectTransform rt)
        {
            if (!rt.parent) return rt;
            Rect _rect = rt.rect;
            RectTransform _parentRT = rt.parent.GetComponent<RectTransform>();
            if (!_parentRT) return rt;

            Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

            Vector2 baseSD = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);
            Vector2 baseAP = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y);

            float xMin = (parentBase.x + rt.localPosition.x - _rect.width * rt.pivot.x) / _parentRT.rect.width;
            float yMin = rt.anchorMin.y;
            float xMax = (parentBase.x + rt.localPosition.x + _rect.width * (1 - rt.pivot.x)) / _parentRT.rect.width;
            float yMax = rt.anchorMax.y;

            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.sizeDelta = new Vector2(0f, baseSD.y);
            rt.anchoredPosition = new Vector2(0f, baseAP.y);

            return rt;
        }

        /// <summary>
        /// 绝对定位转换相对定位(竖向)
        /// </summary>
        public static RectTransform convertAbsRTRelativeV(this RectTransform rt)
        {
            if (!rt.parent) return rt;
            Rect _rect = rt.rect;
            RectTransform _parentRT = rt.parent.GetComponent<RectTransform>();
            if (!_parentRT) return rt;

            Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

            Vector2 baseSD = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);
            Vector2 baseAP = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y);

            float xMin = rt.anchorMin.x;
            float yMin = (parentBase.y + rt.localPosition.y - _rect.height * rt.pivot.y) / _parentRT.rect.height;
            float xMax = rt.anchorMax.x;
            float yMax = (parentBase.y + rt.localPosition.y + _rect.height * (1 - rt.pivot.y)) / _parentRT.rect.height;

            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.sizeDelta = new Vector2(baseSD.x, 0f);
            rt.anchoredPosition = new Vector2(baseAP.x, 0f);

            return rt;
        }

        /// <summary>
        /// 相对定位转换绝对定位 (原点为左下角)
        /// 
        /// </summary>
        public static RectTransform convertRelativeRTAbs(this RectTransform rt)
        {
            if (rt.parent == null) return rt;

            Rect _rect = rt.rect;
            RectTransform _parentRT = rt.parent.GetComponent<RectTransform>();
            if (!_parentRT) return rt;

            //常规方式
            Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

            float xMinPos = parentBase.x + rt.localPosition.x;
            float yMinPos = parentBase.y + rt.localPosition.y;

            Vector3 anchoredPosition = new Vector3(xMinPos, yMinPos);
            Vector2 sizeDelta = rt.rect.size;

            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.sizeDelta = sizeDelta;
            rt.anchoredPosition = anchoredPosition;

            return rt;
        }

    }

}


