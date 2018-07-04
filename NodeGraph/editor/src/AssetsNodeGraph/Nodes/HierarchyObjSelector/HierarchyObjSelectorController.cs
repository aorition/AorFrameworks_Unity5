using Framework.Editor;
using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class HierarchyObjSelectorController : NodeController
    {

        public override void update(bool updateParentLoop = true)
        {
            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(false);
        }
    }
}
