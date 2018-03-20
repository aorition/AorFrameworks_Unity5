using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetFilterController : NodeController
    {
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

                //计算过滤结果
                string[] fks = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("FilterKeys");
                bool[] ics = (bool[])m_nodeGUI.data.ref_GetField_Inst_Public("IgnoreCase");
                if (fks != null)
                {
                    List<string> result = new List<string>();

                    int c, clen = fks.Length;
                    for (c = 0; c < clen; c++)
                    {

                        if (string.IsNullOrEmpty(fks[c])) continue;

                        fks[c] = fks[c].Trim();

                        List<string> finded = new List<string>();
                        bool sx = fks[c].StartsWith(".");
                        bool ig = ics[c];
                        bool nt = fks[c].StartsWith("!");
                        string key;
                        if (nt)
                        {
                            key = fks[c].Substring(1);
                        }
                        else
                        {
                            key = fks[c];
                        }

                        if (sx)
                        {
                            finded = parentData.FindAll((s) =>
                            {
                               // if (string.IsNullOrEmpty(s)) return true;
                                string fn = s.Substring(s.LastIndexOf('/') + 1);
                                if (nt)
                                {
                                    return !fn.ToLower().EndsWith(key.ToLower());
                                }
                                else
                                {
                                    return fn.ToLower().EndsWith(key.ToLower());
                                }
                            });
                        }
                        else
                        {
                            if (ig)
                            {
                                finded = parentData.FindAll((s) =>
                                {
                                 //   if (string.IsNullOrEmpty(s)) return true;
                                    string fn = s.Substring(s.LastIndexOf('/') + 1);
                                    if (nt)
                                    {
                                        return !fn.ToLower().Contains(key.ToLower());
                                    }
                                    else
                                    {
                                        return fn.ToLower().Contains(key.ToLower());
                                    }

                                });
                            }
                            else
                            {
                                finded = parentData.FindAll((s) =>
                                {
                                   // if (string.IsNullOrEmpty(s)) return true;
                                    string fn = s.Substring(s.LastIndexOf('/') + 1);
                                    if (nt)
                                    {
                                        return !fn.Contains(key);
                                    }
                                    else
                                    {
                                        return fn.Contains(key);
                                    }

                                });
                            }
                        }

                        if (finded.Count > 0)
                        {
                            result.AddRange(finded);
                        }

                    }
                    if (result.Count > 0)
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", result.ToArray());
                    }
                    else
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
                    }
                }
                else
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", parentData.ToArray());
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
