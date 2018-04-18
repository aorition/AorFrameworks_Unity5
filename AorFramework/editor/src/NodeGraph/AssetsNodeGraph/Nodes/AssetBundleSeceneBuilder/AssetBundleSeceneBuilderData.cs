using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class AssetBundleSeceneBuilderData : NodeData
    {

        public AssetBundleSeceneBuilderData()
        {
        }

        public AssetBundleSeceneBuilderData(long id) : base(id)
        {
        }

        //BuildOptions
        public readonly string BOEnum = "BuildAdditionalStreamedScenes";

        //BuildTarget
        public readonly string BTEnum = "NoTarget";

        public readonly string SubPath = "StreamedScenes";

    }
}
