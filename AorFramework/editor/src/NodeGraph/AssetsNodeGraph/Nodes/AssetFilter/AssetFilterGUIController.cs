using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.NodeGraph.Tool;
using Framework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<assetsFilter>",
        "Framework.NodeGraph",
        "AssetFilterData|AssetFilterController|AssetFilterGUIController",
        "default",-99,true)]
    public class AssetFilterGUIController : NodeGUIController
    {

        public static string[] FKModeLabelDefine = {"无", "预制体", "场景", "图像", "材质", "FBX", "Shader", "字体"};

        public override string GetNodeLabel()
        {
            bool AdvancedOption = (bool)m_nodeGUI.data.GetPublicField("AdvancedOption");
            string hk = "(" + (AdvancedOption ? NodeGraphLagDefind.Get("advanced") : FKModeLabelDefine[(int)m_nodeGUI.data.GetPublicField("FilterMode")]) + ")";
            return NodeGraphLagDefind.Get("assetsFilter") + ((hk == "(无)") ? "" : " " + hk);
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

            GUILayout.BeginHorizontal();

            bool Advanced = NodeGraphUtility.Draw_NG_Toggle(m_nodeGUI.data, "AdvancedOption", new GUIContent("高级选项"), (b) =>
            {
                m_nodeGUI.SetDirty();
            });

            GUILayout.EndHorizontal();

            if (Advanced)
            {

                GUILayout.Label(new GUIContent("Filter Key :"));
                string[] keylist = (string[]) m_nodeGUI.data.ref_GetField_Inst_Public("FilterKeys");
                bool[] icList = (bool[]) m_nodeGUI.data.ref_GetField_Inst_Public("IgnoreCase");
                bool _ischanged = false;
                int len = (keylist != null ? keylist.Length : 0);
                int nlen = EditorGUILayout.IntField(len);
                nlen = Mathf.Max(nlen, 0);
                if (nlen != len)
                {
                    string[] nkl = new string[nlen];
                    bool[] nic = new bool[nlen];
                    for (int i = 0; i < nlen; i++)
                    {

                        if (keylist != null && i < keylist.Length)
                        {
                            nkl[i] = keylist[i];
                            nic[i] = icList[i];
                        }
                        else
                        {
                            nkl[i] = "";
                            nic[i] = false;
                        }

                        keylist = nkl;
                        icList = nic;
                        _ischanged = true;
                    }
                }

                if (keylist != null)
                {
                    len = keylist.Length;
                    for (int i = 0; i < len; i++)
                    {
                        GUILayout.BeginHorizontal();
                        string n = EditorGUILayout.TextField(keylist[i]);
                        if (n != keylist[i])
                        {
                            keylist[i] = n;
                            _ischanged = true;
                        }
                        GUILayout.FlexibleSpace();
                        //bool ni = EditorGUILayout.ToggleLeft(new GUIContent(AssetNodeGraphLagDefind.GetLabelDefine(14)),icList[i]);
                        bool ni = EditorGUILayout.Toggle(new GUIContent(NodeGraphLagDefind.Get("ignorecase")),
                            icList[i], NodeGraphDefind.GetToggleARStyle());
                        if (ni != icList[i])
                        {
                            icList[i] = ni;
                            _ischanged = true;
                        }
                        GUILayout.EndHorizontal();
                    }

                    if (_ischanged)
                    {
                        m_nodeGUI.data.ref_SetField_Inst_Public("FilterKeys", keylist);
                        m_nodeGUI.data.ref_SetField_Inst_Public("IgnoreCase", icList);
                    }
                }

            }
            else
            {
                NodeGraphUtility.Draw_NG_Popup(m_nodeGUI.data, "FilterMode", new GUIContent("过滤条件"), FKModeLabelDefine, (p) =>
                {
                    _changeFKMode(p);
                });
            }

            string[] assetPaths = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("AssetsPath");
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

            if (GUILayout.Button("Update"))
            {
                m_nodeGUI.controller.update();
            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        //改变
        //{"无", "预制体", "图像", "材质", "FBX", "Shader"};
        private void _changeFKMode(int mode)
        {
            //m_nodeGUI.data.ref_SetField_Inst_Public("FilterMode", mode);

            List<string> keys = new List<string>();
            List<bool> IgnoreCs = new List<bool>();

            switch (mode)
            {
                //预制体
                case 1:
                    keys.Add(".prefab");    IgnoreCs.Add(true);
                    break;
                //场景
                case 2:
                    keys.Add(".unity"); IgnoreCs.Add(true);
                    break;
                //图像
                case 3:
                    keys.Add(".jpg");   IgnoreCs.Add(true);
                    keys.Add(".gif");   IgnoreCs.Add(true);
                    keys.Add(".bmp");   IgnoreCs.Add(true);
                    keys.Add(".tiff");   IgnoreCs.Add(true);
                    keys.Add(".iff");   IgnoreCs.Add(true);
                    keys.Add(".pict");   IgnoreCs.Add(true);
                    keys.Add(".dds");   IgnoreCs.Add(true);
                    keys.Add(".jpeg");  IgnoreCs.Add(true);
                    keys.Add(".png");   IgnoreCs.Add(true);
                    keys.Add(".tga");   IgnoreCs.Add(true);
                    keys.Add(".exr");   IgnoreCs.Add(true);
                    keys.Add(".psd");   IgnoreCs.Add(true);
                    break;
                //材质
                case 4:
                    keys.Add(".mat"); IgnoreCs.Add(true);
                    break;
                //FBX
                case 5:
                    keys.Add(".fbx"); IgnoreCs.Add(true);
                    break;
                //Shader
                case 6:
                    keys.Add(".shader"); IgnoreCs.Add(true);
                    break;
                //Font
                case 7:
                    keys.Add(".ttf"); IgnoreCs.Add(true);
                    keys.Add(".otf"); IgnoreCs.Add(true);
                    break;
                default:
                    //
                    break;
            }

            if (keys.Count > 0 && IgnoreCs.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("FilterKeys", keys.ToArray());
                m_nodeGUI.data.ref_SetField_Inst_Public("IgnoreCase", IgnoreCs.ToArray());
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("FilterKeys", null);
                m_nodeGUI.data.ref_SetField_Inst_Public("IgnoreCase", null);
            }

            m_nodeGUI.SetDirty();
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
