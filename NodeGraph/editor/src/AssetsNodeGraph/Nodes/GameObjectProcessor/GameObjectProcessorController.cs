using System;
using System.Collections.Generic;
using System.Reflection;
using AorBaseUtility.Extends;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class GameObjectProcessorController : NodeController
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
                //检查 自定义脚本 是否是 IGameObjectProcess
                if (_customScriptType.GetInterface("IGameObjectProcess") != null)
                {
                    _customScript = _customScriptType.Assembly.CreateInstance(_customScriptType.FullName);
                    _customScriptMethodInfo = _customScriptType.GetMethod("PrefabProcess", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
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

            List<string> resultInfoList = new List<string>();
            List<int> instanceList = new List<int>();

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
                actionID = (int)clist0[0].GetConnectionValue(false);
                PreActionTarget = clist0[0].OutputPointGui.node.controller;
                PreActionMI = PreActionTarget.GetType().GetMethod("PredefinedAction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
            }
            else
            {
                PreActionMI = null;
                PreActionTarget = null;
            }

            //获取上级节点数据 (PrefabInput)
            ConnectionPointGUI cpg2 = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
            List<ConnectionGUI> clist2 = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg2);
            if (clist2 != null)
            {

                List<int> parentInsIdData = new List<int>();

                len = clist2.Count;
                for (i = 0; i < len; i++)
                {
                    int[] pd = (int[])clist2[i].GetConnectionValue(updateParentLoop);
                    if (pd != null)
                    {
                        //去重复
                        for (int a = 0; a < pd.Length; a++)
                        {
                            if (!parentInsIdData.Contains(pd[a]))
                            {
                                parentInsIdData.Add(pd[a]);
                            }
                        }
                    }
                }

                //查找Prefab
                List<GameObject> gameObjectList = new List<GameObject>();
                len = parentInsIdData.Count;
                for (i = 0; i < len; i++)
                {
                    GameObject go = (GameObject)EditorUtility.InstanceIDToObject(parentInsIdData[i]);
                    if (go)
                    {
                        gameObjectList.Add(go);
                    }
                }

                if (gameObjectList.Count > 0)
                {
                    if (_hasCustomScript)
                    {
                        _customScriptResetMethodInfo.Invoke(_customScript, null);
                    }

                    len = gameObjectList.Count;
                    for (i = 0; i < len; i++)
                    {
                        EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len, Mathf.Round((float)i / len * 10000) * 0.01f);
                        GameObject go = gameObjectList[i];

                        bool n1 = true;
                        bool n2 = true;

                        // 预制动作
                        if (actionID > 0 && PreActionMI != null)
                        {

                            n1 = false;
                            if ((bool)PreActionMI.Invoke(PreActionTarget, new object[] { actionID, go, resultInfoList }))
                            {
                                instanceList.Add(go.GetInstanceID());
                            }
                        }

                        //自定义脚本
                        if (_hasCustomScript)
                        {
                            n2 = false;
                            if ((bool)_customScriptMethodInfo.Invoke(_customScript, new object[] { go, resultInfoList }))
                            {
                                instanceList.Add(go.GetInstanceID());
                            }

                        }

                        //如果该处理器既没有预设动作也没有自定义脚本，则视为通过
                        if (n1 && n2)
                        {
                            instanceList.Add(go.GetInstanceID());
                        }

                        EditorUtility.UnloadUnusedAssetsImmediate(true);

                    }

                    EditorUtility.ClearProgressBar();
                    
                }
                else
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("InstancesPath", null);
                }
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("InstancesPath", null);
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

            if (instanceList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("InstancesPath", instanceList.ToArray());
            }

            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(updateParentLoop);
        }
    }
}
