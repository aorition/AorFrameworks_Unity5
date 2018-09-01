using System;
using System.Collections.Generic;
using AorBaseUtility.Extends;
using Framework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("UnityPackage#<exporter>",
        "Framework.NodeGraph",
        "UnityPackageExporterData|UnityPackageExporterController|UnityPackageExporterGUIController",
        "Advanced")]
    public class UnityPackageExporterGUIController : NodeGUIController
    {

        public override string GetNodeLabel()
        {
            return "UnityPackage" + NodeGraphLagDefind.Get("exporter");
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

            NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "FileHead", new GUIContent("Name Header"));
            NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "FileVersion", new GUIContent("Version"));
            NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "FileSuffix", new GUIContent("Suffix"));

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            NodeGraphUtility.Draw_NG_TextField(m_nodeGUI.data, "SaveDir", new GUIContent("Save Dir"));
            if (GUILayout.Button("Set"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/UnityPackages/"))
                {
                    AssetDatabase.CreateFolder("Assets", "UnityPackages");
                }

                string path = EditorUtility.SaveFolderPanel("保存到", "Assets/UnityPackages/", "");
                if (!string.IsNullOrEmpty(path))
                {
                    path = path.Replace(Application.dataPath, "Assets");
                    m_nodeGUI.data.ref_SetField_Inst_Public("SaveDir", path);
                }
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Export UnityPackage"))
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
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
