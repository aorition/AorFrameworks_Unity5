using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class ProcessPredefinedActionData : NodeData
    {

        public ProcessPredefinedActionData()
        {
        }

        public ProcessPredefinedActionData(long id):base(id)
        {
        }

        public readonly int ActionId;

        public readonly bool Action3UseCustomScript;
        public readonly int Action3ComponentID;
        public readonly string Action3CustomScriptGUID;
    }
}
