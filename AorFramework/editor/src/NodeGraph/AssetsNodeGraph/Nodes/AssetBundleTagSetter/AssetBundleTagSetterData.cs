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

        //启用高级设置
        public readonly bool AdvancedOpt;

        //名称规则id（启用高级设置时无效）
        public readonly int RuleIndex;

        //ABName关键字
        public readonly string ABNameKey;

        //Variant关键字
        public readonly string VariantKey;

        //输出
        public readonly string[] AssetsPath;

    }
}
