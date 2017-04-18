using System;
using System.Collections.Generic;
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
                        parentData.AddRange(pd);
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
                        bool ig = fks[c].StartsWith(".");
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

                        if (ics[c] || ig)
                        {
                            finded = parentData.FindAll((s) =>
                            {
                                if (string.IsNullOrEmpty(s)) return true;
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
                                if (string.IsNullOrEmpty(s)) return true;
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

            base.update(updateParentLoop);
        }
    }
}
