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
                if (fks != null)
                {
                    List<string> result = new List<string>();

                    int c, clen = fks.Length;
                    for (c = 0; c < clen; c++)
                    {

                        if(string.IsNullOrEmpty(fks[c])) continue;
                        
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
                        parentData = parentData.FindAll((s) =>
                        {
                            if (string.IsNullOrEmpty(s)) return true;
                            string fn = s.Substring(s.LastIndexOf('/')+1);
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

                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", parentData.ToArray());

                
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
            }

            base.update(updateParentLoop);
        }
    }
}
