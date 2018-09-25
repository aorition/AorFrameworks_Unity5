using System;
using UnityEngine;

namespace Framework.UI.Utility
{

    [ExecuteInEditMode]
    public class RectTransformSizeListener : MonoBehaviour
    {

        public Action<Vector2, RectTransform, RectTransform> onTargetSizeChange;

        public bool AutoRefresh = true;

        public bool BindingWidth = false;
        public bool BingingHeight = false;

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

                _targetWHCache = wh;

                if (BindingWidth && BingingHeight)
                {
                    _rt.sizeDelta = _targetWHCache;
                }
                else if (BindingWidth)
                {
                    _rt.sizeDelta = new Vector2(_targetWHCache.x, _rt.sizeDelta.y);
                }
                else if (BingingHeight) {
                    _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _targetWHCache.y);
                }

                    if (onTargetSizeChange != null) onTargetSizeChange(wh, _rt, Tartget);
            }
        }

    }

}


