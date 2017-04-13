using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class HierarchyObjSelectorData : NodeData
    {
        public HierarchyObjSelectorData() {}

        public HierarchyObjSelectorData(long id) : base(id) {}

        public readonly int[] SelectedInstanceIDs;

    }
}
