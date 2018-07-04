using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using AorBaseUtility;

namespace Framework.NodeGraph
{
    public class InfoOutputTxtController : NodeController
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

                //信息输出TXT

                if (parentData.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    for (int j = 0; j < parentData.Count; j++)
                    {
                        sb.Append(parentData[j]+"\r\n");
                    }
                    if (sb != null && sb.Length > 0)
                    {
                        string path = (string)m_nodeGUI.data.ref_GetField_Inst_Public("outPutPath");
                        if(path != null)
                        {
                            AorIO.SaveStringToFile(path+"/测试结果导出文件.txt",sb.ToString());
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

            base.update(updateParentLoop);
        }
    }
}
