using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public interface INodeController
    {
        NodeGUI nodeGUI { get; }

        void setup(NodeGUI node);

        bool isInit { get; }

        string nodeDataToString();

        void update(bool updateParentLoop = true);

    }
}
