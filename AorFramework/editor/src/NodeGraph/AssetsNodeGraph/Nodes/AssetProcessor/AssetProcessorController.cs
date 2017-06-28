using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AorFramework.AssetsNodeGraph;
using AorFramework.editor;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetProcessorController : NodeController
    {

        private Type _customScriptType;
        private object _customScript;
        private MethodInfo _customScriptMethodInfo;
        private MethodInfo _customScriptResetMethodInfo;

        private bool _getCustomScript(string GUID)
        {
            UnityEngine.Object cso = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID));
            if (cso != null)
            {
                MonoScript ms = cso as MonoScript;
                _customScriptType = ms.GetClass();
                //检查 自定义脚本 是否是 IAssetProcess
                if (_customScriptType.GetInterface("IAssetProcess") != null)
                {
                    _customScript = _customScriptType.Assembly.CreateInstance(_customScriptType.FullName);
                    _customScriptMethodInfo = _customScriptType.GetMethod("Process", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
                    _customScriptResetMethodInfo = _customScriptType.GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);

                    //获取CustomScriptDescribeAttribute
                    Attribute[] attributes = Attribute.GetCustomAttributes(_customScriptMethodInfo);
                    if (attributes != null && attributes.Length > 0)
                    {

                        int i, len = attributes.Length;
                        for (i = 0; i < len; i++)
                        {
                            if (attributes[i].GetType() == typeof(CustomScriptDescribeAttribute))
                            {
                                string des = (string)attributes[i].ref_GetField_Inst_Public("Describe");
                                if (!string.IsNullOrEmpty(des))
                                {
                                    m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptDescribe", des);
                                }
                            }
                        }

                    }

                    //获取ResultInfoDescribe
                    MethodInfo ridMInfo = _customScriptType.GetMethod("ResultInfoDescribe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
                    if (ridMInfo != null)
                    {
                        string rid = (string)ridMInfo.Invoke(_customScript, null);
                        if (!string.IsNullOrEmpty(rid))
                        {
                            m_nodeGUI.data.ref_SetField_Inst_Public("ResultInfoDescribe", rid);
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        private bool _hasCustomScript
        {
            get { return (_customScriptType != null && _customScript != null && _customScriptMethodInfo != null); }
        }

        public override void update(bool updateParentLoop = true)
        {
            List<string> resultPathList = new List<string>();
            List<string> resultInfoList = new List<string>();

            int i = 0;
            int len;

            //获取自定义脚本
            if (!_hasCustomScript)
            {
                string guid = (string)m_nodeGUI.data.ref_GetField_Inst_Public("CustomScriptGUID");
                if (!string.IsNullOrEmpty(guid))
                {
                    _getCustomScript(guid);
                }
            }

            //获取上级节点数据 (PathInput)
            ConnectionPointGUI cpg = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
            List<ConnectionGUI> clist = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg);
            if (clist != null)
            {

                List<string> parentData = new List<string>();

                len = clist.Count;
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
                    //自定义脚本
                    if (_hasCustomScript)
                    {

                        _customScriptResetMethodInfo.Invoke(_customScript, null);

                        len = parentData.Count;
                        for (i = 0; i < len; i++)
                        {
                            EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len, (float)i / len);
                            if ((bool)_customScriptMethodInfo.Invoke(_customScript, new object[] { parentData[i], resultInfoList }))
                            {
                                resultPathList.Add(parentData[i]);
                            }
                        }

                        EditorUtility.ClearProgressBar();

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
                
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
            }
            
            //输出 。。。
            if (resultInfoList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptResultInfo", resultInfoList.ToArray());
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptResultInfo", null);
            }

            if (resultPathList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", resultPathList.ToArray());
            }

            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(updateParentLoop);
        }
    }
}
