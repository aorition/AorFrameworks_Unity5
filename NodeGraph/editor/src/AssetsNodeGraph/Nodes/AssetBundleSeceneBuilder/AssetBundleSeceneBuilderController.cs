using System;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility.Extends;
using NodeGraph.SupportLib;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class AssetBundleSeceneBuilderController : NodeController
    {


       // private static Regex GUIDMatch = new Regex("{");

        public override void update(bool updateParentLoop = true)
        {

            BuildOptions bo = (BuildOptions)Enum.Parse(typeof(BuildOptions), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BOEnum"));
            BuildTarget bt = (BuildTarget)Enum.Parse(typeof(BuildTarget), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BTEnum"));

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            string subPath = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SubPath");
            string path = Application.streamingAssetsPath + (string.IsNullOrEmpty(subPath) ? "" : "/" + subPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //获取上级节点数据
            ConnectionPointGUI cpg = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
            List<ConnectionGUI> clist = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg);
            if (clist != null)
            {

                List<string> parentData = new List<string>();

                int i, len = clist.Count;
                for (i = 0; i < len; i++)
                {

                    string[] pd = (string[]) clist[i].GetConnectionValue(updateParentLoop);
                    if (pd != null)
                    {
                        //去重复
                        for (int a = 0; a < pd.Length; a++)
                        {
                            if (!parentData.Contains(pd[a]))
                            {
                                parentData.Add(pd[a]);
                            }
                        }
                    }
                }

                if (parentData.Count > 0)
                {

                    List<EditorAssetInfo> scenePaths = new List<EditorAssetInfo>();
                    for (int b = 0; b < parentData.Count; b++)
                    {
                        //验证文件格式
                        EditorAssetInfo spi = new EditorAssetInfo(parentData[b]);
                        if (spi.suffix == ".unity")
                        {
                            scenePaths.Add(spi);
                        }
                    }


                    if (scenePaths.Count > 0)
                    {

                        NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)

                        for (int s = 0; s < scenePaths.Count; s++)
                        {

                            string up = scenePaths[s].path;
                            string n = AssetImporter.GetAtPath(up).assetBundleName;

                            string[] levels = new [] { scenePaths[s].path };
                            BuildPipeline.BuildPlayer(levels, path + "/" + n + ".unity3d", bt, bo);
                            
                        }

                        // 刷新，可以直接在Unity工程中看见打包后的文件
                        AssetDatabase.Refresh();
                    }

                }

            }

            base.update(updateParentLoop);
        }
    }
}
