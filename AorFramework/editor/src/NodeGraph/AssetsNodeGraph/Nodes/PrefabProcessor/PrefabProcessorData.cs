using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class PrefabProcessorData : NodeData
    {

        public PrefabProcessorData()
        {
        }

        public PrefabProcessorData(long id):base(id)
        {
        }

        public readonly string CustomScriptGUID;

        public readonly string CustomScriptDescribe;

        public readonly string ResultInfoDescribe;

        public readonly string[] CustomScriptResultInfo;

//        public readonly string[] InputAssetsPath;

        public readonly string[] AssetsPath;

    }
}
