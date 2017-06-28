﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.AssetsNodeGraph;
using AorFramework.editor;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetPrefabProcessorController : NodeController
    {

        private Type _customScriptType;
        private object _customScript;
        private MethodInfo _customScriptMethodInfo;
        private MethodInfo _customScriptResetMethodInfo;
        private FieldInfo[] _customScriptFieldInfos; 

        private bool _getCustomScript(string GUID)
        {
            UnityEngine.Object cso = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID));
            if (cso != null)
            {
                MonoScript ms = cso as MonoScript;
                _customScriptType = ms.GetClass();

                //检查 自定义脚本 是否是 IGameObjectProcess
                if (_customScriptType.GetInterface("IGameObjectProcess") != null)
                {
                    _customScript = _customScriptType.Assembly.CreateInstance(_customScriptType.FullName);
                    _customScriptMethodInfo = _customScriptType.GetMethod("PrefabProcess", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
                    _customScriptResetMethodInfo = _customScriptType.GetMethod("Reset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);

                    //获取设置字段
                    _customScriptFieldInfos = _customScriptType.GetFields(BindingFlags.Instance| BindingFlags.Public| BindingFlags.GetField);

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

            //获取ActionID
            int actionID = 0;
            MethodInfo PreActionMI;
            object PreActionTarget;
            ConnectionPointGUI cpg0 = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 101, ConnectionPointInoutType.Input);
            List<ConnectionGUI> clist0 = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg0);
            if (clist0 != null)
            {
                actionID = (int) clist0[0].GetConnectionValue(false);
                PreActionTarget = clist0[0].OutputPointGui.node.controller;
                PreActionMI = PreActionTarget.GetType().GetMethod("PredefinedAction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
            }
            else
            {
                PreActionMI = null;
                PreActionTarget = null;
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
                
                //查找Prefab
                List<string> inputPathList = new List<string>();
                len = parentData.Count;
                for (i = 0; i < len; i++)
                {
                    EditorAssetInfo info = new EditorAssetInfo(parentData[i]);
                    if (info.suffix.ToLower() == ".prefab")
                    {
                        inputPathList.Add(info.path);
                    }
                }

                if (inputPathList.Count > 0)
                {
                    
                    if (_hasCustomScript)
                    {
                        _customScriptResetMethodInfo.Invoke(_customScript, null);
                    }

                    len = inputPathList.Count;
                    for (i = 0; i < len; i++)
                    {
                        EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len, (float)i / len);
                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(inputPathList[i]);
                        if (go)
                        {

                            bool n1 = true;
                            bool n2 = true;

                            //预制动作
                            if (actionID > 0 && PreActionMI != null)
                            {
                                n1 = false;
                                if ((bool)PreActionMI.Invoke(PreActionTarget, new object[] { actionID, go, resultInfoList }))
                                {
                                    resultPathList.Add(inputPathList[i]);
                                }
                            }

                            //自定义脚本
                            if (_hasCustomScript)
                            {

                                n2 = false;
                                if ((bool) _customScriptMethodInfo.Invoke(_customScript, new object[] {go, resultInfoList}))
                                {
                                    resultPathList.Add(inputPathList[i]);
                                }
                            }

                            //如果该处理器既没有预设动作也没有自定义脚本，则视为通过
                            if (n1 && n2)
                            {
                                resultPathList.Add(inputPathList[i]);
                            }

                            EditorUtility.UnloadUnusedAssetsImmediate(true);
                        }
                    }

                    EditorUtility.ClearProgressBar();

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
