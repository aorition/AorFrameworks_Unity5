﻿using System;
using System.Collections.Generic;
using AorBaseUtility;
using AorFramework.DataWrapers;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("Assets#<15>",
        "AorFramework.NodeGraph",
        "AssetTextureProcessorData|AssetTextureProcessorController|AssetTextureProcessorGUIController")]
    public class AssetTextureProcessorGUIController : NodeGUIController
    {

        private string[] _actionLabelDefine = { "无","批量修改图片" };

        private GUIStyle _describeStyle;

        private GUIStyle _resultInfoStyle;

        public override string GetNodeLabel()
        {
             return "Assets" + AssetNodeGraphLagDefind.GetLabelDefine(15);
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
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "PathInput", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(10) + AssetNodeGraphLagDefind.GetLabelDefine(7), new Vector2(100, 60), ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(200, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(8), new Vector2(100, 60), ConnectionPointInoutType.Output);

                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0, p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

        public override void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {
            //string
            string info = "0";
            string[] assetList = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("AssetsPath");
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
            int actionId = EditorGUILayout.Popup("预设处理动作", aId, _actionLabelDefine);
            if (actionId != aId)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ActionId", actionId);
            }

            //TODO预设动作操作
            if (actionId > 0)
            {
                draw_presetActionGUI(actionId);
            }
            else
            {
                //移除所有数据
                m_nodeGUI.data.ref_SetField_Inst_Public("PresetActionData_TextureImporterDatawraper", null);//移除PresetActionData
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
                        GUILayout.Label(AssetNodeGraphLagDefind.GetLabelDefine(4) + "(ITexturePrecess)");
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
                GUILayout.Label(AssetNodeGraphLagDefind.GetLabelDefine(4) + "(ITexturePrecess)");
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

        /// <summary>
        /// 绘制并处理相应Id的预设动作GUI（包括处理数据）
        /// </summary>
        /// <param name="actionId"></param>
        private void draw_presetActionGUI(int actionId)
        {
            switch (actionId)
            {
                //同一修改TextureImporter
                case 1:
                    TextureImporterDatawraper TIDwraper = (TextureImporterDatawraper)m_nodeGUI.data.ref_GetField_Inst_Public("PresetActionData_TextureImporterDatawraper");
                    if (TIDwraper == null)
                    {

                        TIDwraper = new TextureImporterDatawraper();
                        m_nodeGUI.data.ref_SetField_Inst_Public("PresetActionData_TextureImporterDatawraper", TIDwraper);
                    }
                    //TextureImporterDatawraper 编辑界面显示
                    draw_presetActionGUI_1_GUI(TIDwraper);

                    //TODO 移除其他PresetActionData （如果有 。。。）
                    //
                    break;
            }
        }

        private void draw_presetActionGUI_1_GUI(TextureImporterDatawraper TIDwraper)
        {
            GUILayout.BeginVertical("box");
            bool isChanged = false;
            //textureType
            TextureImporterDatawraper.inner_TextureImporterType oType = (TextureImporterDatawraper.inner_TextureImporterType)Enum.Parse(typeof(TextureImporterDatawraper.inner_TextureImporterType), TIDwraper.textureType);
            TextureImporterDatawraper.inner_TextureImporterType nType = (TextureImporterDatawraper.inner_TextureImporterType) EditorGUILayout.EnumPopup("Texture Type", oType);
            if (nType != oType)
            {
                TIDwraper.updateConfigValue("textureType", nType.ToString());
                isChanged = true;
            }
            //textureShape
            TextureImporterShape oShape = (TextureImporterShape)Enum.Parse(typeof(TextureImporterShape), TIDwraper.textureShape);
            TextureImporterShape nShape = (TextureImporterShape)EditorGUILayout.EnumPopup("Texture Shape", oShape);
            if (nShape != oShape)
            {
                TIDwraper.updateConfigValue("textureShape", nShape.ToString());
                isChanged = true;
            }

            GUILayout.Space(10);



            GUILayout.EndVertical();
        }

        


    }

}
