using System;
using UnityEngine;

namespace AorFramework.NodeGraph
{

    public class NodeGraphToolItemData
    {

        public NodeGraphToolItemData(Rect rect, string label, string data, bool isDefaultIntoShortcutMenu = false)
        {
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
