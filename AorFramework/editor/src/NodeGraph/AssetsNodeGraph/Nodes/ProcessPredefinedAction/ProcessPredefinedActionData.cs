using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
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

    }
}
