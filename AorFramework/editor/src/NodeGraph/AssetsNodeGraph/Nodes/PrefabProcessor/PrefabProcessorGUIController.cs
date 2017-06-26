﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("GameObject#<3>",
        "AorFramework.NodeGraph",
        "PrefabProcessorData|PrefabProcessorController|PrefabProcessorGUIController",
        "Hierarchy Tools")]
    public class PrefabProcessorGUIController : NodeGUIController
    {

        private string[] _actionLabelDefine = { "无" };

        private GUIStyle _describeStyle;

        private GUIStyle _resultInfoStyle;

        public override string GetNodeLabel()
        {
             return "GameObject" + AssetNodeGraphLagDefind.GetLabelDefine(3);
        }

        private Vector2 _NodeMinSize = new Vector2(230, 120);
        public override Vector2 GetNodeMinSizeDefind()
        {
            return _NodeMinSize;
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(int[]).Name, "PrefabInput", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(9) + AssetNodeGraphLagDefind.GetLabelDefine(7), new Vector2(100, 60), ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(200, 0, 2, typeof(int[]).Name, "InstancesPath", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(8), new Vector2(120, 60), ConnectionPointInoutType.Output);
                ConnectionPointGUI p2 = new ConnectionPointGUI(201, 1, 2, typeof(int[]).Name, "CustomScriptResultInfo", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(16), new Vector2(120, 60), ConnectionPointInoutType.Output);

                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0, p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

        public override void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {
            //string
            string info = "0";
            int[] assetList = (int[])m_nodeGUI.data.ref_GetField_Inst_Public("InstancesPath");
            if (assetList != null)
            {
                info = assetList.Length.ToString();
            }

            //size
            Vector2 CTSzie = new Vector2(NodeGraphTool.GetConnectCenterTipLabelWidth(info) + 4, NodeGraphDefind.ConnectCenterTipLabelPreHeight);

            //rect
            connection.CenterRect = new Rect(centerPos.x - CTSzie.x * 0.5f, centerPos.y - CTSzie.y * 0.5f, CTSzie.x, CTSzie.y);

            //ConnectionTip
            GUI.Label(connection.CenterRect, info, GetConnectCenterTipStyle());

            //右键菜单检测
            if (Event.current.button == 1 && Event.current.isMouse && connection.CenterRect.Contains(Event.current.mousePosition))
            {
                DrawCenterTipContextMenu(connection);
                Event.current.Use();
            }
        }

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            int aId = (int)m_nodeGUI.data.ref_GetField_Inst_Public("ActionId");
            int actionId = EditorGUILayout.Popup("预制处理动作", aId, _actionLabelDefine);
            if (actionId != aId)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ActionId", actionId);
            }

            string oguid = (string)m_nodeGUI.data.ref_GetField_Inst_Public("CustomScriptGUID");
            
            if (!string.IsNullOrEmpty(oguid))
            {
                string customPath = AssetDatabase.GUIDToAssetPath(oguid);
                if (!string.IsNullOrEmpty(customPath))
                {
                    UnityEngine.Object custom = AssetDatabase.LoadMainAssetAtPath(customPath);
                    if (custom != null)
                    {
                        GUILayout.Label(AssetNodeGraphLagDefind.GetLabelDefine(4) + "(IGameObjectProcess)");
                        UnityEngine.Object n = EditorGUILayout.ObjectField(custom, typeof(MonoScript), false);
                        if (n == null)
                        {
                            m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptGUID", 0);
                            m_nodeGUI.SetDirty();
                        }
                        else if (n != custom)
                        {
                            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(n));
                            if (!string.IsNullOrEmpty(guid))
                            {
                                if((bool)m_nodeGUI.controller.ref_InvokeMethod_Inst_NonPublic("_getCustomScript", new object[] { guid })) { 
                                    m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptGUID", guid);
                                    m_nodeGUI.SetDirty();
                                }
                            }
                            
                        }

                        //显示描述
                        string des = (string) m_nodeGUI.data.ref_GetField_Inst_Public("CustomScriptDescribe");
                        if (!string.IsNullOrEmpty(des))
                        {
                            if (_describeStyle == null)
                            {
                                _describeStyle = GUI.skin.GetStyle("box");
                                _describeStyle.fontSize = 12;
                                _describeStyle.alignment = TextAnchor.MiddleLeft;
                                _describeStyle.wordWrap = true;
                                _describeStyle.normal.textColor = Color.white;
                            }

                            GUILayout.Label(AssetNodeGraphLagDefind.GetLabelDefine(5) + " : " + des, _describeStyle);
                        }

                        //显示自定义脚本字段
                        object cs = m_nodeGUI.controller.ref_GetField_Inst_NonPublic("_customScript");
                        if (cs != null)
                        {
                            //FieldInfo[] _customScriptFieldInfos
                            FieldInfo[] fieldInfos = (FieldInfo[])m_nodeGUI.controller.ref_GetField_Inst_NonPublic("_customScriptFieldInfos");
                            if (fieldInfos != null && fieldInfos.Length > 0)
                            {

                                for (int i = 0; i < fieldInfos.Length; i++)
                                {
                                    REFUtility.DrawFieldByFieldInfo(fieldInfos[i], cs);
                                }
                            }
                        }

                    }
                    else
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptGUID", null);
                        m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptDescribe", null);
                        m_nodeGUI.SetDirty();
                    }
                }
                else
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptGUID", null);
                    m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptDescribe", null);
                    m_nodeGUI.SetDirty();
                }
            }
            else
            {
                GUILayout.Label(AssetNodeGraphLagDefind.GetLabelDefine(4) + "(IGameObjectProcess)");
                UnityEngine.Object n = EditorGUILayout.ObjectField(null, typeof (MonoScript), false);
                if (n != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(n));
                    if (!string.IsNullOrEmpty(guid))
                    {
                        if ((bool)m_nodeGUI.controller.ref_InvokeMethod_Inst_NonPublic("_getCustomScript", new object[] { guid }))
                        {
                            m_nodeGUI.data.ref_SetField_Inst_Public("CustomScriptGUID", guid);
                            m_nodeGUI.SetDirty();
                        }
                    }
                }
            }

            //显示结果信息
            string[] scriptResultInfo = (string[]) m_nodeGUI.data.ref_GetField_Inst_Public("CustomScriptResultInfo");
            if (scriptResultInfo != null)
            {

                GUILayout.Space(10);
                string rid = (string)m_nodeGUI.data.ref_GetField_Inst_Public("ResultInfoDescribe");
                if (string.IsNullOrEmpty(rid))
                {
                    GUILayout.Label(AssetNodeGraphLagDefind.GetLabelDefine(6) + ":");
                }
                else
                {
                    GUILayout.Label(rid + ":");
                }
                GUILayout.Space(6);
                GUILayout.BeginVertical("box");

                int i, len = scriptResultInfo.Length;
                for (i = 0; i < len; i++)
                {

                    if (_resultInfoStyle == null)
                    {
                        _resultInfoStyle = GUI.skin.label;
                        _resultInfoStyle.alignment = TextAnchor.MiddleLeft;
                        _resultInfoStyle.wordWrap = true;
                        _resultInfoStyle.normal.textColor = Color.white;
                    }

                    GUILayout.Label(scriptResultInfo[i], _resultInfoStyle);
                }

                GUILayout.EndVertical();
            }

            if (GUILayout.Button("Update"))
            {
                m_nodeGUI.controller.update();
            }

            GUILayout.EndVertical();

           // base.DrawNodeInspector(inspectorWidth);
        }
    }

}
