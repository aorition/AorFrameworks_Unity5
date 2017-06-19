using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class InfoOutputTxtData : NodeData
    {

        public InfoOutputTxtData()
        {
        }

        public InfoOutputTxtData(long id) : base(id)
        {
        }

        public readonly bool[] IgnoreCase;

        public readonly bool AdvancedOption = false;

        public readonly int FilterMode = 0;

        public readonly string[] FilterKeys;

        public readonly string[] AssetsPath;

        public readonly string outPutPath;

    }
}
