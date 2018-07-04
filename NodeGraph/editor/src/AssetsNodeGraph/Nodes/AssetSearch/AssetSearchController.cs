using Framework.Editor;
using System;
using System.Collections.Generic;
using AorBaseUtility;
using NodeGraph.SupportLib;
using UnityEditor;

namespace Framework.NodeGraph
{
    public class AssetSearchController : NodeController
    {
        public override void update(bool updateParentLoop = true)
        {
            string[] sp = (string[]) m_nodeGUI.data.GetPublicField("SearchPaths");
            if (sp == null || sp.Length == 0) return;

            string sp2 = (string)m_nodeGUI.data.GetPublicField("SearchPattern");
            if (string.IsNullOrEmpty(sp2))
            {
                sp2 = "*.*";
            }
            bool ignoreMETAFile = (bool) m_nodeGUI.data.GetPublicField("IgnoreMETAFile");
            List<EditorAssetInfo> eai = new List<EditorAssetInfo>();
            foreach (string s in sp)
            {

                //检查s是否是文件夹路径 
                if (AssetDatabase.IsValidFolder(s))
                {
                    List<EditorAssetInfo> sub;
                    if (ignoreMETAFile)
                    {
                        sub = EditorAssetInfo.FindEditorAssetInfoInPath(s, sp2).filter((e) =>
                        {
                            return (e.suffix.ToLower() != ".meta");
                        });
                    }
                    else
                    {
                        sub = EditorAssetInfo.FindEditorAssetInfoInPath(s, sp2);
                    }
                    if (sub != null && sub.Count > 0)
                    {
                        foreach (EditorAssetInfo assetInfo in sub)
                        {
                            if (!eai.Contains(assetInfo))
                            {
                                eai.Add(assetInfo);
                            }
                        }
                    }
                }
                else
                {
                    //检查s是否是文件
                    if (s.ToLower().Contains("."))
                    {
                        EditorAssetInfo assetInfo = new EditorAssetInfo(s);
                        if (!eai.Contains(assetInfo))
                        {
                            eai.Add(assetInfo);
                        }
                    }
                }
            }

            if (eai.Count > 0)
            {
                List<string> assetPathList = new List<string>();
                int i, len = eai.Count;
                for (i = 0; i < len; i++)
                {
                    assetPathList.Add(eai[i].path);
                }

                string[] list = assetPathList.ToArray();
                m_nodeGUI.data.SetPublicField("AssetsPath", list);

            }

            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(updateParentLoop);
            m_nodeGUI.data.SetNonPublicField("m_isDirty", false);
        }
    }
}
