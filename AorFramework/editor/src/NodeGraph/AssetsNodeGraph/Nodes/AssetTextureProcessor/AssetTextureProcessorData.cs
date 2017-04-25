using System;
using System.Collections.Generic;
using AorBaseUtility;
using AorFramework.DataWrapers;

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

        public readonly int ActionId;

        //ActionId = 1:
        public readonly TextureImporterDatawraper PresetActionData_TextureImporterDatawraper;

        public readonly string CustomScriptGUID;

        public readonly string CustomScriptDescribe;

        public readonly string ResultInfoDescribe;

        public readonly string[] CustomScriptResultInfo;

        public readonly string[] AssetsPath;

    }
}
