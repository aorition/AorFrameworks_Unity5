using System;
using System.Collections.Generic;
using AorBaseUtility;

namespace AorFramework.NodeGraph
{
    public class CustomProcessData : NodeData
    {
        public CustomProcessData() {}

        public CustomProcessData(long id) : base(id) {}

        //public readonly TreeNode<int> SelectedTreeNode; 

        public readonly int[] SelectedInstanceIDs;

    }
}
