using System;
using System.Collections.Generic;

namespace Framework.NodeGraph
{
    public class UnityPackageExporterData : NodeData
    {

        public UnityPackageExporterData()
        {
        }

        public UnityPackageExporterData(long id) : base(id)
        {
        }

        //BuildAssetBundleOptions
        public readonly string SaveDir = "";
        public readonly string FileHead = "UnityPackage";
        public readonly string FileVersion = "1.0.0.0";
        public readonly string FileSuffix = ".unitypackage";

    }
}
