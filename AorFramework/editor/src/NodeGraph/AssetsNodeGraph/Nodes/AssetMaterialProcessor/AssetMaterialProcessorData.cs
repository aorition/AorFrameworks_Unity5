using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class AssetMaterialProcessorData : NodeData
    {

        public AssetMaterialProcessorData()
        {
        }

        public AssetMaterialProcessorData(long id):base(id)
        {
        }

        public readonly string CustomScriptGUID;

        public readonly string CustomScriptDescribe;

        public readonly string ResultInfoDescribe;

        public readonly string[] CustomScriptResultInfo;

        public readonly string[] AssetsPath;

        public readonly bool _ModifyZTest;
        public readonly string  _ZTest = "Less";

        public readonly bool _ModifyZWrite;
        public readonly bool _ZWrite;

        public readonly bool _ModifyCull;
        public readonly string  _Cull= "Off";

        public readonly bool _ModifySrcBlend;
        public readonly string  _SrcBlend= "One";

        public readonly bool _ModifyDstBlend;
        public readonly string  _DstBlend= "One";

        public readonly bool _ModifySrcAlphaBlend;
        public readonly string  _SrcAlphaBlend= "One";

        public readonly bool _ModifyDstAlphaBlend;
        public readonly string  _DstAlphaBlend= "One";

        //public readonly UnityEngine.Rendering.CompareFunction _ZTest;
        //public readonly bool _ZWrite;
        //public readonly UnityEngine.Rendering.CullMode _Cull;
       // public readonly UnityEngine.Rendering.BlendMode _SrcBlend;
        //public readonly UnityEngine.Rendering.BlendMode _DstBlend;
        //public readonly UnityEngine.Rendering.BlendMode _SrcAlphaBlend;
        //public readonly UnityEngine.Rendering.BlendMode _DstAlpahaBlend;

    }

    
}
