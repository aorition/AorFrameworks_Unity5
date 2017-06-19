using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AorFramework.NodeGraph
{
    public class AssetDependController : NodeController
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

                //查询依赖关系

                if (parentData.Count > 0)
                {
                    string[] depends = AssetDatabase.GetDependencies(parentData.ToArray());

                    if (depends != null && depends.Length > 0)
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", depends);
                    }
                    else {
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
                    }
                }
                else {
                    m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
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
