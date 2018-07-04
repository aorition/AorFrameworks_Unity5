using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class UnityPackageExporterController : NodeController
    {

        public override void update(bool updateParentLoop = true)
        {

            string head = (string)m_nodeGUI.data.ref_GetField_Inst_Public("FileHead");
            string version = (string)m_nodeGUI.data.ref_GetField_Inst_Public("FileVersion");
            string suffix = (string)m_nodeGUI.data.ref_GetField_Inst_Public("FileSuffix");
            string path = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SaveDir");

            string fileName = (string.IsNullOrEmpty(head) ? "unNamePackage_" : head + "_")
                              + (string.IsNullOrEmpty(version) ? "" : "v." + version)
                              + (string.IsNullOrEmpty(suffix) ? ".unitypackage" : (suffix.IndexOf('.') == -1 ? "." + suffix : suffix));

            if (string.IsNullOrEmpty(path))
            {

                if (!AssetDatabase.IsValidFolder("Assets/UnityPackages/"))
                {
                    AssetDatabase.CreateFolder("Assets", "UnityPackages");
                }

                path = EditorUtility.SaveFolderPanel("保存到", "Assets/UnityPackages/", "");

                if (string.IsNullOrEmpty(path))
                {
                    EditorUtility.DisplayDialog("提示", "导出UnityPackage已取消!", "确定");
                    return;
                }
                path = path.Replace(Application.dataPath, "Assets");
                m_nodeGUI.data.ref_SetField_Inst_Public("SaveDir", path);

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
                    string[] pd = (string[])clist[i].GetConnectionValue(updateParentLoop);
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
                    string savePath = path + "/" + fileName;
                    EditorUtility.DisplayProgressBar("导出中...", "正在导出...", 0.98f);
                    AssetDatabase.ExportPackage(parentData.ToArray(), savePath, ExportPackageOptions.Default);
                    EditorUtility.DisplayDialog("提示", "导出成功 \n " + savePath, "确定");
                    AssetDatabase.Refresh();
                }

            }

            base.update(updateParentLoop);
        }
    }
}
