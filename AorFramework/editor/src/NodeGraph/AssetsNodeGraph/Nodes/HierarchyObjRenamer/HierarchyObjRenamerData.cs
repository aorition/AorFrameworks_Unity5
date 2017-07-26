using System;
using System.Collections.Generic;
using AorBaseUtility;

namespace AorFramework.NodeGraph
{
    public class HierarchyObjRenamerData : NodeData
    {
        public HierarchyObjRenamerData() {}

        public HierarchyObjRenamerData(long id) : base(id) {}
        
        public readonly bool UseEditorSelection = false;

        public readonly string RenameKey;

        public readonly int ShotNum;

        public readonly int[] InstancesPath;

        public readonly string[] ResultInfo;

    }
}
