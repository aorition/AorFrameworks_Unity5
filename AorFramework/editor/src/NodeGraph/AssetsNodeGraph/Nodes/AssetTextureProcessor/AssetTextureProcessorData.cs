using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class AssetTextureProcessorData : NodeData
    {

        public AssetTextureProcessorData()
        {
        }

        public AssetTextureProcessorData(long id):base(id)
        {
        }

        public readonly string CustomScriptGUID;

        public readonly string CustomScriptDescribe;

        public readonly string ResultInfoDescribe;

        public readonly string[] CustomScriptResultInfo;

        public readonly string[] AssetsPath;

    }
}
