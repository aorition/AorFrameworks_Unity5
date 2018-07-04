using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class CheckReferenceData : NodeData
    {

        public CheckReferenceData()
        {
        }

        public CheckReferenceData(long id) : base(id)
        {
        }

        public readonly bool[] IgnoreCase;

        public readonly bool AdvancedOption = false;

        public readonly int FilterMode = 0;

        public readonly string[] FilterKeys;

        public readonly string[] AssetsPath;
        public readonly string[] SearchPath;
        public readonly Dictionary <string ,List <string >> Redependence;
        public readonly Dictionary<string, List<string>> Redependence_Down;
        public readonly Dictionary<string, List<string>> Redependence_Cross;
        public readonly Dictionary<string, List<string>> Redependence_None;
        public readonly int ActionID;
    }
}
