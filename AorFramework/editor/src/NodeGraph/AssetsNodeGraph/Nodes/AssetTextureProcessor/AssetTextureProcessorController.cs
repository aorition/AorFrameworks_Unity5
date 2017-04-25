using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.AssetsNodeGraph;
using AorFramework.editor;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetTextureProcessorController : NodeController
    {
        
        private static List<string> _filtSuffixDefine = new List<string>() { ".jpg", ".gif", ".bmp", ".tiff", ".iff", ".pict", ".dds", ".jpeg", ".png", ".tga", ".exr", ".psd" };

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
                if (_customScriptType.GetInterface("ITexturePrecess") != null)
                {
                    _customScript = _customScriptType.Assembly.CreateInstance(_customScriptType.FullName);
                    _customScriptMethodInfo = _customScriptType.GetMethod("TextureProcess", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);

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

            //获取ActionID
            int actionID = (int)m_nodeGUI.data.ref_GetField_Inst_Public("ActionId");

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

                //查找Texture2D
                List<string> inputPathList = new List<string>();
                len = parentData.Count;
                for (i = 0; i < len; i++)
                {
                    EditorAssetInfo info = new EditorAssetInfo(parentData[i]);
                    if (_filtSuffixDefine.Contains(info.suffix.ToLower()))
                    {
                        inputPathList.Add(info.path);
                    }
                }

                if (inputPathList.Count > 0)
                {
                    //m_nodeGUI.data.ref_SetField_Inst_Public("InputAssetsPath", inputPathList.ToArray());

                    //预制动作
                    if (actionID > 0)
                    {

                        switch (actionID)
                        {
                            //
                            case 1:
                                
                                break;
                            //Todo 预制动作处理

                            default: //0
                                break;
                        }

                    }

                    //自定义脚本
                    if (_hasCustomScript)
                    {

                        len = inputPathList.Count;
                        for (i = 0; i < len; i++)
                        {
                            EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len,
                                Mathf.Round((float) i/len*10000)*0.01f);
                            Texture2D t2d = AssetDatabase.LoadAssetAtPath<Texture2D>(inputPathList[i]);
                            if (t2d)
                            {

                                if (
                                    (bool)
                                        _customScriptMethodInfo.Invoke(_customScript, new object[] { t2d, resultInfoList}))
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

            base.update(updateParentLoop);
        }
    }
}
