using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class AssetBundleBuilderData : NodeData
    {

        public AssetBundleBuilderData()
        {
        }

        public AssetBundleBuilderData(long id) : base(id)
        {
        }

        //BuildAssetBundleOptions
        public readonly string BBOEnum = "None";

        //BuildTarget
        public readonly string BTEnum = "NoTarget";
        
        //启用增量打包（资源强制打在一个包内）
        public readonly bool AddonsPacking;

        public readonly string APBundleName;
        public readonly string APVariantName;

        public readonly string SubPath;

    }
}
