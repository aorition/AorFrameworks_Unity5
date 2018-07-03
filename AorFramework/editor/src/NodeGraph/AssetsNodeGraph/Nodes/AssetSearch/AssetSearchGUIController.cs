using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.NodeGraph.Tool;
using Framework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<importRes>",
        "Framework.NodeGraph",
        "AssetSearchData|AssetSearchController|AssetSearchGUIController",
        "default", -100, true)]
    public class AssetSearchGUIController : NodeGUIController
    {

        private GUIStyle _guiContentTextStyle;

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("importRes");
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

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            GUILayout.BeginHorizontal();

            NodeGraphUtility.Draw_NG_ToggleLeft(m_nodeGUI.data, "IgnoreMETAFile",new GUIContent("忽略META文件"), (b) =>
            {
                m_nodeGUI.SetDirty();
            });

            bool Advanced = NodeGraphUtility.Draw_NG_ToggleLeft(m_nodeGUI.data, "AdvancedOption", new GUIContent("高级选项"), (b) =>
            {
                m_nodeGUI.SetDirty();
            });

            GUILayout.EndHorizontal();

            if (Advanced)
            {
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
            }
            
            GUILayout.Space(5);

            string[] SearchPaths = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPaths");
            if (SearchPaths == null || SearchPaths.Length == 0)
            {
                //空的
                if (GUILayout.Button("SelcetSearchPath"))
                {
                    SearchPaths = _getSearchPathsFormSelection();
                    if (SearchPaths != null && SearchPaths.Length > 0)
                    {
                        m_nodeGUI.data.SetPublicField("SearchPaths", SearchPaths);
                        m_nodeGUI.data.SetNonPublicField("m_isDirty", true);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "你没有选择任何对象,请在Project下选择输入对象", "确定");
                    }
                }

            }
            else
            {
                //显示已经记录的搜索路径
                GUILayout.Label(new GUIContent("搜索路径："));
                GUILayout.Space(5);
                GUILayout.BeginVertical("box");
                foreach (string path in SearchPaths)
                {
                    GUILayout.Label(path);
                }
                if (GUILayout.Button("重设搜索路径"))
                {
                    SearchPaths = _getSearchPathsFormSelection();
                    if (SearchPaths != null && SearchPaths.Length > 0)
                    {
                        m_nodeGUI.data.SetPublicField("SearchPaths", SearchPaths);
                        m_nodeGUI.data.SetNonPublicField("m_isDirty", true);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "你没有选择任何对象,请在Project下选择输入对象", "确定");
                    }
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);

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

        private string[] _getSearchPathsFormSelection()
        {
            if (Selection.objects != null && Selection.objects.Length > 0)
            {
                string path;
                List<string> list = new List<string>();
                foreach (UnityEngine.Object obj in Selection.objects)
                {
                    path = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!list.Contains(path))
                        {
                            list.Add(path);
                        }
                    }
                }
                return list.ToArray();
            }
            return null;
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo (GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI,NodeGraphLagDefind.Get("output"),new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }
    }
}
