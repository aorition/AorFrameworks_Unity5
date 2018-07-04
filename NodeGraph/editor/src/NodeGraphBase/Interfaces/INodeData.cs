using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.NodeGraph
{
    public interface INodeData
    {
        
        //id 同windowID，在一NodeGraph中不可重复
        long id { get; }

        //名称 类名
        string name { get; set; }

        //标脏
        bool isDirty { get; set; }

    }
}
