using System;
using System.Collections.Generic;
using AorFramework.NodeGraph.Tool;
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

        private static string[] _TagSetRuleStrDefind = new[] {"GUID", "assets", "resources"};

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
            string[] assetList = (string[]) connection.GetConnectionValue(false);
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

            bool adv = (bool) m_nodeGUI.data.ref_GetField_Inst_Public("AdvancedOpt");
            bool nadv = EditorGUILayout.Toggle("高级", adv);
            if (nadv != adv)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AdvancedOpt", nadv);
            }

            if (nadv)
            {
                //高级设置 GUI
                string abKey = (string)m_nodeGUI.data.ref_GetField_Inst_Public("ABNameKey");
                string nabKey = EditorGUILayout.TextField("ABName关键字", abKey);
                if (nabKey != abKey)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", nabKey);
                }

                string vKey = (string)m_nodeGUI.data.ref_GetField_Inst_Public("VariantKey");
                string nvKey = EditorGUILayout.TextField("Variant关键字", vKey);
                if (nvKey != vKey)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("VariantKey", nvKey);
                }

            }
            else
            {

                int ri = (int)m_nodeGUI.data.ref_GetField_Inst_Public("RuleIndex");
                int nri = EditorGUILayout.Popup("ABName命名规则", ri, _TagSetRuleStrDefind);
                if (nri != ri)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("RuleIndex", nri);
                }

                switch (nri)
                {

                    case 1: //使用assets路径为ABName
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{AP}");
                        break;
                    case 2: //使用resources路径为ABName
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{RP}");
                        break;
                    default: //默认TagSet处理规则
                        m_nodeGUI.data.ref_SetField_Inst_Public("ABNameKey", "{GUID}");
                        break;

                }

                string vKey = (string)m_nodeGUI.data.ref_GetField_Inst_Public("VariantKey");
                string nvKey = EditorGUILayout.TextField("Variant关键字", vKey);
                if (nvKey != vKey)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("VariantKey", nvKey);
                }

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
