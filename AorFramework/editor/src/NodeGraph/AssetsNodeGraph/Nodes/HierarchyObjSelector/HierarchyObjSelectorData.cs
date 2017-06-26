using System;
using System.Collections.Generic;
using AorBaseUtility;

namespace AorFramework.NodeGraph
{
    public class HierarchyObjSelectorData : NodeData
    {
        public HierarchyObjSelectorData() {}

        public HierarchyObjSelectorData(long id) : base(id) {}

        //public readonly TreeNode<int> SelectedTreeNode; 

        public readonly int[] SelectedInstanceIDs;

    }
}
