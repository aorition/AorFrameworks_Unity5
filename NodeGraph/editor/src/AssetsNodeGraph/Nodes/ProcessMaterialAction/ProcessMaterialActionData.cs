using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class ProcessMaterialActionData : NodeData
    {

        public ProcessMaterialActionData()
        {
        }

        public ProcessMaterialActionData(long id):base(id)
        {
        }

        public readonly int ActionId;

        public readonly bool Action3UseCustomScript;
        public readonly int Action3ComponentID;
        public readonly string Action3CustomScriptGUID;

      
    }
}
