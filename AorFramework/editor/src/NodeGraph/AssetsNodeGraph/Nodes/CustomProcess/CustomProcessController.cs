using AorFramework.editor;
using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class CustomProcessController : NodeController
    {

        public override void update(bool updateParentLoop = true)
        {
            base.update(false);
        }
    }
}
