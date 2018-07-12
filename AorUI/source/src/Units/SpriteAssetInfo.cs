using UnityEngine;
using System;

namespace UnityEngine.UI
{
    [Serializable]
    public struct SpriteAssetInfo
    {


        public SpriteAssetInfo(int ID, string name, Vector2 pivot, Rect rect, Sprite sprite)
        {
            this.ID = ID;
            this.name = name;
            this.pivot = pivot;
            this.rect = rect;
            this.sprite = sprite;
            this._isInit = true;
        }

        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit; }
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 名称
        /// </summary>
        public string name;

        /// <summary>
        /// 中心点
        /// </summary>
        public Vector2 pivot;

        /// <summary>
        ///坐标&宽高
        /// </summary>
        public Rect rect;

        /// <summary>
        /// 精灵
        /// </summary>
        public Sprite sprite;

    }
}

