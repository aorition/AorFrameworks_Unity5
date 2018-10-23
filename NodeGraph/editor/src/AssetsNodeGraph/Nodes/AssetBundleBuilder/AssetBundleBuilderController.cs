using System;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility.Extends;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class AssetBundleBuilderController : NodeController
    {


       // private static Regex GUIDMatch = new Regex("{");

        public override void update(bool updateParentLoop = true)
        {
            


         bool None=(bool )m_nodeGUI .data .ref_GetField_Inst_Public ("None");
         bool UncompressedAssetBundle=(bool)m_nodeGUI.data.ref_GetField_Inst_Public("UncompressedAssetBundle");
         bool CollectDependencies = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("CollectDependencies");
         bool CompleteAssets = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("CompleteAssets");
         bool DisableWriteTypeTree = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("DisableWriteTypeTree");
         bool DeterministicAssetBundle = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("DeterministicAssetBundle");
         bool ForceRebuildAssetBundle = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("ForceRebuildAssetBundle");
         bool IgnoreTypeTreeChanges = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("IgnoreTypeTreeChanges");
         bool AppendHashToAssetBundleName = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("AppendHashToAssetBundleName");
         bool ChunkBasedCompression = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("ChunkBasedCompression");
         bool StrictMode = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("StrictMode");
         bool DryRunBuild = (bool)m_nodeGUI.data.ref_GetField_Inst_Public("DryRunBuild");
          BuildAssetBundleOptions bbo= BuildAssetBundleOptions.None;
          
                if(UncompressedAssetBundle)
                    bbo= BuildAssetBundleOptions.UncompressedAssetBundle;
                if (CollectDependencies)
                    bbo = bbo| BuildAssetBundleOptions.CollectDependencies; 
                if (CompleteAssets)
                    bbo = bbo | BuildAssetBundleOptions.CompleteAssets; 
                if (DisableWriteTypeTree)
                    bbo = bbo | BuildAssetBundleOptions.DisableWriteTypeTree; 
                if (DeterministicAssetBundle)
                    bbo = bbo | BuildAssetBundleOptions.DeterministicAssetBundle; 
                if (ForceRebuildAssetBundle)
                    bbo = bbo | BuildAssetBundleOptions.ForceRebuildAssetBundle; 
                if (IgnoreTypeTreeChanges)
                    bbo = bbo | BuildAssetBundleOptions.IgnoreTypeTreeChanges;
                if (AppendHashToAssetBundleName)
                    bbo = bbo | BuildAssetBundleOptions.AppendHashToAssetBundleName;
                if (ChunkBasedCompression)
                    bbo = bbo | BuildAssetBundleOptions.ChunkBasedCompression;
                if (StrictMode)
                    bbo = bbo | BuildAssetBundleOptions.StrictMode;
                if (DryRunBuild)
                    bbo = bbo | BuildAssetBundleOptions.DryRunBuild; 

            
       // BuildAssetBundleOptions bbo = (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BBOEnum"));
            BuildTarget bt = EditorUserBuildSettings.activeBuildTarget; //(BuildTarget)Enum.Parse(typeof(BuildTarget), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BTEnum"));

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            string subPath = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SubPath");
            string path = Application.streamingAssetsPath + (string.IsNullOrEmpty(subPath) ? "" : "/" + subPath);
            if(!Directory .Exists (path ))
            {
                Directory.CreateDirectory(path);
            }
           // Debug.Log(subPath);

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

                    //if (addons)
                    //{
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
                    //}
                    //else
                    //{
                    //    clist[i].GetConnectionValue(updateParentLoop);
                    //}
                }


                //if (addons)
                //{

                    if (parentData.Count > 0)
                    {
                        //APBundleName
                    //    string abName = (string) m_nodeGUI.data.ref_GetField_Inst_Public("APBundleName");
                    //    string vtName = (string) m_nodeGUI.data.ref_GetField_Inst_Public("APVariantName");

                    List<AssetBundleBuild> _abbList = new List<AssetBundleBuild>();

                    //    string[] enemyAssets = new string[parentData.Count];
                        for (int e = 0; e < parentData.Count; e++)
                        {

                            string p = parentData[e];
                            AssetImporter ai = AssetImporter.GetAtPath(p);
                        //ai.SetAssetBundleNameAndVariant(abName, vtName);
                        //EditorUtility.SetDirty(ai);
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = ai.assetBundleName ;// AssetDatabase .AssetPathToGUID (parentData [e]);
                        abb.assetNames = new string[] { parentData[e] };//AssetDatabase.GetDependencies(parentData[e]);
                        _abbList.Add(abb);
                        //buildMap.Add(abb);
                        //enemyAssets[e] = parentData[e];
                        }
                    //buildMap[0].assetNames = enemyAssets;

                        NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)
                    BuildPipeline.BuildAssetBundles(path,_abbList.ToArray (), bbo, bt);//, buildMap.ToArray (), bbo, bt);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    }
                }
                else
                {
                    NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)
                    BuildPipeline.BuildAssetBundles(path, bbo, bt);
                }
            //}
            //else
            //{
            //    NodeGraphBase.Instance.SaveLastShotcutGraph();//立即保存快照(无法使用延迟快照)
            //    BuildPipeline.BuildAssetBundles(path, bbo, bt);
            //}

            base.update(updateParentLoop);
        }
    }
}
