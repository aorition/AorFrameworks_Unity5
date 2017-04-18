using System;
using System.Collections.Generic;
using AorFramework.NodeGraph.Tool;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("#<0>", 
        "AorFramework.NodeGraph.AssetNodeGraphToolItemDefinder",
        "CreateAssetSearch",
        "default", -100, true)]
    public class AssetSearchGUIController : NodeGUIController
    {

        private GUIStyle _guiContentTextStyle;

        public override string GetNodeLabel()
        {
            return AssetNodeGraphLagDefind.GetLabelDefine(0);
        }

        private Vector2 _MinSizeDefind = new Vector2(160, 120);
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

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            bool IgnoreMETAFile = GUILayout.Toggle((bool)m_nodeGUI.data.ref_GetField_Inst_Public("IgnoreMETAFile"), "忽略META文件");
            if (IgnoreMETAFile != (bool)m_nodeGUI.data.ref_GetField_Inst_Public("IgnoreMETAFile"))
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("IgnoreMETAFile", IgnoreMETAFile);
                m_nodeGUI.SetDirty();
            }

            string SearchPattern = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SearchPattern");
            if (string.IsNullOrEmpty(SearchPattern))
            {
                SearchPattern = "*.*";
                m_nodeGUI.data.ref_SetField_Inst_Public("SearchPattern", SearchPattern);
            }

            GUILayout.Label(new GUIContent("搜索匹配："));
            string nsp = GUILayout.TextField(SearchPattern);
            if (nsp != SearchPattern)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("SearchPattern", nsp);
            }

            GUILayout.Space(5);

            string SearchPath = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            if (string.IsNullOrEmpty(SearchPath))
            {

                if (GUILayout.Button("SelcetSearchPath"))
                {
                    if (Selection.activeObject != null)
                    {
                        SearchPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    }
                    else
                    {
                        SearchPath = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }

                    if (!string.IsNullOrEmpty(SearchPath))
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", SearchPath);
                        m_nodeGUI.SetDirty();
                    }

                }

            }
            else
            {

                GUILayout.Label(new GUIContent("搜索路径："));
                GUILayout.BeginHorizontal();
                string n = GUILayout.TextField(SearchPath);
                if (n != SearchPath)
                {
                    m_nodeGUI.data.ref_SetField_Inst_NonPublic("m_isDirty", true);
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", n);
                }
                if (GUILayout.Button("重设"))
                {
                    if (Selection.activeObject != null)
                    {
                        SearchPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    }
                    else
                    {
                        SearchPath = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }

                    if (!string.IsNullOrEmpty(SearchPath))
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", SearchPath);
                        m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", null);
                        m_nodeGUI.SetDirty();
                    }
                }
                GUILayout.EndHorizontal();

                string[] assetPaths = (string[]) m_nodeGUI.data.ref_GetField_Inst_Public("AssetsPath");
                if (assetPaths != null && assetPaths.Length > 0)
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label("找到 " + assetPaths.Length + "个资源文件");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("资源浏览器"))
                    {
                        AssetPathBrowserWindow.init(assetPaths);
                    }
                    GUILayout.EndHorizontal();
                }

                if ((bool)m_nodeGUI.data.ref_GetField_Inst_NonPublic("m_isDirty"))
                {
                    if (GUILayout.Button("SearchAssets"))
                    {
                        m_nodeGUI.controller.update();
                    }
                }
                else
                {
                    if (GUILayout.Button("Refresh"))
                    {
                        m_nodeGUI.controller.update();
                    }
                }

            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo (GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI,AssetNodeGraphLagDefind.GetLabelDefine(8),new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }
    }
}
