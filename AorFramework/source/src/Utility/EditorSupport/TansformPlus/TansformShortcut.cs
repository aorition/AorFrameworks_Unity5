using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{

    public class TansformShortcut : MonoBehaviour, IEditorOnlyScript
    {

        [Serializable]
        public struct TansformStruct
        {

            public static void ApplyToTansform(TansformStruct TansformStruct, Transform transform)
            {
                TansformStruct.ApplyToTansform(transform);
            }

            public static TansformStruct CreateFromTansform(Transform transform)
            {
                return new TansformStruct(transform.localPosition, transform.localEulerAngles, transform.localScale);
            }

            public TansformStruct(Vector3 postion, Vector3 eulerAngles, Vector3 scale)
            {
                this.Postion = postion;
                this.EulerAngles = eulerAngles;
                this.Scale = scale;
                this._isInit = true;
            }

            private bool _isInit;
            public bool isInit
            {
                get { return _isInit;}
            }

            public Vector3 Postion;
            public Vector3 EulerAngles;
            public Vector3 Scale;

            public void ApplyToTansform(Transform transform)
            {
                transform.localScale = Scale;
                transform.localEulerAngles = EulerAngles;
                transform.localPosition = Postion;
            }

        }

        //-----------

        public int ShortcutIndex = -1;

        public int ShortcutCount
        {
            get { return _dataList.Count; }
        }

        [SerializeField]
        private List<TansformStruct> _dataList = new List<TansformStruct>();

        public int AddShortcut(TansformStruct shortcut)
        {
            _dataList.Add(shortcut);
            return _dataList.Count - 1;
        }
        public void RemoveShortcut(int index)
        {
            if (index >= 0 && index < _dataList.Count)
            {
                _dataList.RemoveAt(index);
            }
        }
        public void RemoveShortcut(TansformStruct shortcut)
        {
            _dataList.Remove(shortcut);
        }

        public TansformStruct GetShortcut(int index)
        {
            if (index >= 0 && index < _dataList.Count)
            {
                return _dataList[index];
            }
            return new TansformStruct();
        }

        public void ClearShortcut()
        {
            ShortcutIndex = 0;
            _dataList.Clear();
        }

    }

}
