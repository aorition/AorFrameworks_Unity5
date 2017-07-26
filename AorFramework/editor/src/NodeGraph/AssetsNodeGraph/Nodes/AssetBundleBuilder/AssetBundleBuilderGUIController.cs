using System;
using System.Collections.Generic;
using AorFramework.NodeGraph.Tool;
using AorFramework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("AssetBundleBuilder",
        "AorFramework.NodeGraph",
        "AssetBundleBuilderData|AssetBundleBuilderController|AssetBundleBuilderGUIController",
        "AssetBundleTools")]
    public class AssetBundleBuilderGUIController : NodeGUIController
    {

        public override string GetNodeLabel()
        {
            return "AssetBundleBuilder";
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

            //AddonsPacking
            bool addp = NodeGraphUtility.Draw_NG_Toggle(m_nodeGUI.data, "AddonsPacking", new GUIContent("增量打包模式"));
            if (addp)
            {
                //APBundleName
                NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "APBundleName", new GUIContent("AssetBundleName"));

                //APVariantName
                NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "APVariantName", new GUIContent("VariantName"));
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("APBundleName", null);
                m_nodeGUI.data.ref_SetField_Inst_Public("APVariantName", null);
            }

            //BuildAssetBundleOptions
            NodeGraphUtility.Draw_NG_EnumPopup<BuildAssetBundleOptions>(m_nodeGUI.data, "BBOEnum",new GUIContent("BuildAssetBundleOptions"));

            //BuildTarget
            NodeGraphUtility.Draw_NG_EnumPopup<BuildAssetBundleOptions>(m_nodeGUI.data, "BTEnum", new GUIContent("BuildTarget"));

            //save路径
            NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "SubPath", new GUIContent("APBundleSave路径(默认为空)"));

            if (GUILayout.Button("Build AssetBundle"))
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
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
