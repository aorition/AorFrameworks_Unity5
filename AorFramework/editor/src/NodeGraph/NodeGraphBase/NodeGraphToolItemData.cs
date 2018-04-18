using System;
using UnityEngine;

namespace Framework.NodeGraph
{

    public class NodeGraphToolItemData
    {

        public NodeGraphToolItemData(Rect rect, string label, string data, bool isDefaultIntoShortcutMenu = false)
        {
            this._DefineWidth = rect.width;
            this._DefineHeight = rect.height;
            this.rect = rect;
            this.label = label;
            this.data = data;
            this.isDefaultIntoShortcutMenu = isDefaultIntoShortcutMenu;
        }

        public float height
        {
            get { return rect.height; }
        }

        public float width
        {
            get { return rect.width; }
        }

        private float _DefineWidth;
        public float DefineWidth {
            get { return _DefineWidth; }
        }
        private float _DefineHeight;
        public float DefineHeight
        {
            get { return _DefineHeight; }
        }

        public Rect rect;
        public string label;
        public string data;

        public bool isDefaultIntoShortcutMenu;

        public override string ToString()
        {
            return "NodeGraphToolItemData[" +
                   "label:" + label +
                   ",data:" + data + "]";
        }
    }
}
