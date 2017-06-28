using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetBundleBuilderController : NodeController
    {


       // private static Regex GUIDMatch = new Regex("{");

        public override void update(bool updateParentLoop = true)
        {
            
            BuildAssetBundleOptions bbo = (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BBOEnum"));
            BuildTarget bt = (BuildTarget)Enum.Parse(typeof(BuildTarget), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BTEnum"));

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            string subPath = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SubPath");
            string path = Application.streamingAssetsPath + (string.IsNullOrEmpty(subPath) ? "" : "/" + subPath);

            //获取上级节点数据
            ConnectionPointGUI cpg = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
            List<ConnectionGUI> clist = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg);
            if (clist != null)
            {

                bool addons = (bool) m_nodeGUI.data.ref_GetField_Inst_Public("AddonsPacking");

                List<string> parentData = new List<string>();

                int i, len = clist.Count;
                for (i = 0; i < len; i++)
                {

                    if (addons)
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
                    else
                    {
                        clist[i].GetConnectionValue(updateParentLoop);
                    }
                }


                if (addons)
                {

                    if (parentData.Count > 0)
                    {
                        //APBundleName
                        string abName = (string) m_nodeGUI.data.ref_GetField_Inst_Public("APBundleName");
                        string vtName = (string) m_nodeGUI.data.ref_GetField_Inst_Public("APVariantName");

                        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

                        string[] enemyAssets = new string[parentData.Count];
                        for (int e = 0; e < parentData.Count; e++)
                        {

                            string p = parentData[e];
                            AssetImporter ai = AssetImporter.GetAtPath(p);
                            ai.SetAssetBundleNameAndVariant(abName, vtName);
                            EditorUtility.SetDirty(ai);

                            enemyAssets[e] = parentData[e];
                        }
                        buildMap[0].assetNames = enemyAssets;

                        NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)
                        BuildPipeline.BuildAssetBundles(path, buildMap, bbo, bt);

                    }
                }
                else
                {
                    NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)
                    BuildPipeline.BuildAssetBundles(path, bbo, bt);
                }
            }
            else
            {
                NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)
                BuildPipeline.BuildAssetBundles(path, bbo, bt);
            }

            base.update(updateParentLoop);
        }
    }
}
