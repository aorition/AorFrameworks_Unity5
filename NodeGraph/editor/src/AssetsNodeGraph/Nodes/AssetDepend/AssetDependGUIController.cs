using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.NodeGraph.Tool;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<assetsDepend>",
        "Framework.NodeGraph",
        "AssetDependData|AssetDependController|AssetDependGUIController")]
    public class AssetDependGUIController : NodeGUIController
    {

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("assetsDepend");
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

            string[] assetPaths = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("AssetsPath");
            if (assetPaths != null && assetPaths.Length > 0)
            {

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("找到 " + assetPaths.Length + "个资源依赖文件");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("资源浏览器"))
                {
                    AssetPathBrowserWindow.init(assetPaths);
                }
                GUILayout.EndHorizontal();

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
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "input", m_nodeGUI, NodeGraphLagDefind.Get("input"), new Vector2(100,60),  ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(101, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI, NodeGraphLagDefind.Get("output"), new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0, p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
