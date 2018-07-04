using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class AssetProcessorData : NodeData
    {

        public AssetProcessorData()
        {
        }

        public AssetProcessorData(long id):base(id)
        {
        }
        
        public readonly string CustomScriptGUID;

        public readonly string CustomScriptDescribe;

        public readonly string ResultInfoDescribe;

        public readonly string[] CustomScriptResultInfo;

        public readonly string[] AssetsPath;

    }
}
