﻿using System;
using System.Collections.Generic;
using AorFramework.NodeGraph.Tool;
using AorFramework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("AssetBundleLabel#<18>",
        "AorFramework.NodeGraph",
        "AssetBundleTagSetterData|AssetBundleTagSetterController|AssetBundleTagSetterGUIController",
        "AssetBundleTools")]
    public class AssetBundleTagSetterGUIController : NodeGUIController
    {

        private static string[] _TagSetRuleStrDefind = new[] {"GUID", "assets", "resources", "name"};

        public override string GetNodeLabel()
        {
            return "AssetBundleLabel" + AssetNodeGraphLagDefind.GetLabelDefine(18);
        }

        private Vector2 _MinSizeDefind = new Vector2(200, 120);
        public override Vector2 GetNodeMinSizeDefind()
        {
            return _MinSizeDefind;
        }

        public override void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {
            //string
            string info = "0";
            object ConnectionValue = connection.GetConnectionValue(false);
            if (ConnectionValue != null)
            {
                if (ConnectionValue is Array)
                {
                    info = (ConnectionValue as Array).Length.ToString();
                }
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

//        private 
//        public override Vector2 GetNodeMinSizeDefind()
//        {
//            return base.GetNodeMinSizeDefind();
//        }

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            //Todo 使用说明。。 

            bool adv = NodeGraphUtility.Draw_NG_Toggle(m_nodeGUI.data, "AdvancedOpt", new GUIContent("高级"));
            if (adv)
            {
                //高级设置 GUI
                NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "ABNameKey", new GUIContent("ABName关键字"));

                NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "VariantKey", new GUIContent("Variant关键字"));

            }
            else
            {
                int ri = NodeGraphUtility.Draw_NG_Popup(m_nodeGUI.data, "RuleIndex", _TagSetRuleStrDefind);
                switch (ri)
                {

                    case 1: //使用assets路径为ABName
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{AP}");
                        break;
                    case 2: //使用resources路径为ABName
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{RP}");
                        break;
                    case 3: //使用name作为ABName
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{N}");
                        break;
                    default: //默认TagSet处理规则
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{GUID}");
                        break;

                }

                NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "VariantKey", new GUIContent("Variant关键字"));
            }

            if (GUILayout.Button("Update"))
            {
                m_nodeGUI.controller.update();
            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "input", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(7), new Vector2(100,60),  ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(101, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(8), new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0, p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
