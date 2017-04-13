using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class AssetSearchData : NodeData
    {
        public AssetSearchData() {}

        public AssetSearchData(long id) : base(id) {}

        public readonly bool IgnoreMETAFile = true;

        public readonly string SearchPattern;

        public readonly string SearchPath;

        public readonly string[] AssetsPath;

    }
}
