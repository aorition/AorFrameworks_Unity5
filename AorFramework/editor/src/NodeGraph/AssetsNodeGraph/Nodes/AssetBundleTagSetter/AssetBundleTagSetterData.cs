using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class AssetBundleTagSetterData : NodeData
    {

        public AssetBundleTagSetterData()
        {
        }

        public AssetBundleTagSetterData(long id) : base(id)
        {
        }

        public readonly string ABNameKey;

        public readonly string VariantKey;

        public readonly string[] AssetsPath;

    }
}
