using System;
using System.Collections.Generic;
using AorBaseUtility.Extends;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<info>#<output>",
        "Framework.NodeGraph",
        "InfoOutputTxtData|InfoOutputTxtController|InfoOutputTxtGUIController"
        )]
    public class InfoOutputTxtGUIController : NodeGUIController
    {

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("info") + NodeGraphLagDefind.Get("output");
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

            GUILayout.Label(new GUIContent("请输入要导出的地址："));

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            string outPath = (string)m_nodeGUI.data.ref_GetField_Inst_Public("outPutPath");

            if (string.IsNullOrEmpty(outPath))
            {

                if (GUILayout.Button("选择导出地址"))
                {
                    if (Selection.activeObject != null)
                    {
                        outPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    }
                    else
                    {
                        outPath = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }

                    if (!string.IsNullOrEmpty(outPath))
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("outPutPath", outPath);
                        m_nodeGUI.SetDirty();
                    }
                }
            }
            else
            {

                GUILayout.Label(new GUIContent("导出路径："));
                string n = GUILayout.TextField(outPath);
                if (n != outPath)
                {
                    m_nodeGUI.data.ref_SetField_Inst_NonPublic("m_isDirty", true);
                    m_nodeGUI.data.ref_SetField_Inst_Public("outPutPath", n);
                }
                if (GUILayout.Button("重设"))
                {
                    if (Selection.activeObject != null)
                    {
                        outPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    }
                    else
                    {
                        outPath = EditorUtility.OpenFolderPanel("指定导出文件夹", null, null);
                    }

                    if (!string.IsNullOrEmpty(outPath))
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("outPutPath", outPath);
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
                        m_nodeGUI.SetDirty();
                    }
                }

                string[] assetPaths = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("AssetsPath");
                if (assetPaths != null && assetPaths.Length > 0)
                {
                    GUILayout.Label("导出 " + assetPaths.Length + "个资源地址");
                    GUILayout.FlexibleSpace();
                }
            }

            if ((bool)m_nodeGUI.data.ref_GetField_Inst_NonPublic("m_isDirty"))
            {
                if (GUILayout.Button("导出"))
                {
                    m_nodeGUI.controller.update();
                }
            }
            GUILayout.EndVertical();
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "input", m_nodeGUI, NodeGraphLagDefind.Get("info"), new Vector2(100,60),  ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(101, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI, NodeGraphLagDefind.Get("output"), new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0, p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
