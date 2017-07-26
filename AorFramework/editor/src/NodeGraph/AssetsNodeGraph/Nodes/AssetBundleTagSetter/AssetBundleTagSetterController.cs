﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AorFramework.editor;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetBundleTagSetterController : NodeController
    {


       // private static Regex GUIDMatch = new Regex("{");

        public override void update(bool updateParentLoop = true)
        {

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

                    //读取ABNameKey
                    string abNameKey = (string)m_nodeGUI.data.ref_GetField_Inst_Public("ABNameKey");

                    //读取VariantKey
                    string variantKey = (string) m_nodeGUI.data.ref_GetField_Inst_Public("VariantKey");

                    List<string> assetList = new List<string>();

                    string abName, variant;

                    len = parentData.Count;
                    for (i = 0; i < len; i++)
                    {

                        EditorAssetInfo pathInfo = new EditorAssetInfo(parentData[i]);

                        if (pathInfo.suffix == ".cs" || pathInfo.suffix == ".js") continue;

                        //计算abName
                        abName = abNameKey;
                        //GUID
                        if (abNameKey.Contains("{GUID}"))
                        {
                            string guid = AssetDatabase.AssetPathToGUID(pathInfo.path);
                            abName = abName.Replace("{GUID}", guid);
                        }
                        if (abNameKey.Contains("{AP}"))
                        {
                            string ap = pathInfo.dirPath.Replace("Assets/","");
                            abName = abName.Replace("{AP}", ap);
                        }
                        if (abNameKey.Contains("{RP}"))
                        {
                            string rp = pathInfo.resDirPath.Replace("Resources/", "");
                            abName = abName.Replace("{RP}", rp);
                        }
                        if (abNameKey.Contains("{N}"))
                        {
                            string n = pathInfo.name;
                            abName = abName.Replace("{N}", n);
                        }
                        //计算variant
                        variant = variantKey;

                        AssetImporter ai = AssetImporter.GetAtPath(pathInfo.path);
                        ai.SetAssetBundleNameAndVariant(abName, variant);

                        EditorUtility.SetDirty(ai);

                        assetList.Add(pathInfo.path);
                    }

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();


                    if (assetList != null && assetList.Count > 0)
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", assetList.ToArray());

                    }

                }
                else
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
                }



            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
            }

            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(updateParentLoop);
        }
    }
}
