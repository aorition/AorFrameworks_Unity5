using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.AssetsNodeGraph;
using AorFramework.editor;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class PrefabProcessorController : NodeController
    {

        private Type _customScriptType;
        private object _customScript;
        private MethodInfo _customScriptMethodInfo;

        private bool _getCustomScript(string GUID)
        {
            UnityEngine.Object cso = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID));
            if (cso != null)
            {
                MonoScript ms = cso as MonoScript;
                _customScriptType = ms.GetClass();
                //检查 自定义脚本 是否是 IPrefabProcess
                if (_customScriptType.GetInterface("IPrefabProcess") != null)
                {
                    _customScript = _customScriptType.Assembly.CreateInstance(_customScriptType.FullName);
                    _customScriptMethodInfo = _customScriptType.GetMethod("PrefabProcess", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);

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
                        parentData.AddRange(pd);
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
                    //m_nodeGUI.data.ref_SetField_Inst_Public("InputAssetsPath", inputPathList.ToArray());

                    //自定义脚本
                    if (_hasCustomScript)
                    {

                        len = inputPathList.Count;
                        for (i = 0; i < len; i++)
                        {
                            EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len, Mathf.Round((float)i / len * 10000)* 0.01f);
                            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(inputPathList[i]);
                            if (go)
                            {
     
                                if ((bool)_customScriptMethodInfo.Invoke(_customScript, new object[] { go, resultInfoList}))
                                {
                                    resultPathList.Add(inputPathList[i]);
                                }
                            }
                        }

                        EditorUtility.ClearProgressBar();

                    }
                    else
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", inputPathList.ToArray());
                    }

                }

            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("InputAssetsPath", null);
                //m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
            }

            //获取上级节点数据 (PrefabInput)
            ConnectionPointGUI cpg2 = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 101, ConnectionPointInoutType.MutiInput);
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
                        parentInsIdData.AddRange(pd);
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
                   // m_nodeGUI.data.ref_SetField_Inst_Public("InputAssetsPath", inputPathList.ToArray());

                    //自定义脚本
                    if (_hasCustomScript)
                    {

                        len = gameObjectList.Count;
                        for (i = 0; i < len; i++)
                        {
                            EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len, Mathf.Round((float)i / len * 10000) * 0.01f);
                            GameObject go = gameObjectList[i];
                            if ((bool)_customScriptMethodInfo.Invoke(_customScript, new object[] { go, resultInfoList }))
                            {
                                string goPath = AssetDatabase.GetAssetPath(go);
                                if (!string.IsNullOrEmpty(goPath))
                                {
                                    resultPathList.Add(goPath);
                                }
                            }
                        }

                        EditorUtility.ClearProgressBar();

                    }
                    else
                    {
                        len = gameObjectList.Count;
                        for (i = 0; i < len; i++)
                        {
                            string goPath = AssetDatabase.GetAssetPath(gameObjectList[i]);
                            resultPathList.Add(goPath);
                        }
                    }
                    
                }
                else
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("InputAssetsPath", null);
                }
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("InputAssetsPath", null);
                //m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
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
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
            }

            base.update(updateParentLoop);
        }
    }
}
