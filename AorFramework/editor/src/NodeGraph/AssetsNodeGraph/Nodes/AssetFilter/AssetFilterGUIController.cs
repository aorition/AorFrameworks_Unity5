using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("#<1>", 
        "AorFramework.NodeGraph.AssetNodeGraphToolItemDefinder",
        "CreateAssetFilter",
        "default",-99,true)]
    public class AssetFilterGUIController : NodeGUIController
    {
        public override string GetNodeLabel()
        {
            return AssetNodeGraphLagDefind.GetLabelDefine(1);
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

            GUILayout.Label(new GUIContent("Filter Key :"));
            string[] keylist = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("FilterKeys");
            bool _ischanged = false;
            int len = (keylist != null ? keylist.Length : 0);
            int nlen = EditorGUILayout.IntField(len);
            nlen = Mathf.Max(nlen, 0);
            if (nlen != len)
            {
                string[] nkl = new string[nlen];
                for (int i = 0; i < nlen; i++)
                {

                    if (keylist != null && i < keylist.Length)
                    {
                        nkl[i] = keylist[i];
                    }
                    else
                    {
                        nkl[i] = "";
                    }

                    keylist = nkl;
                    _ischanged = true;
                }
            }

            if (keylist != null)
            {
                len = keylist.Length;
                for (int i = 0; i < len; i++)
                {
                    string n = EditorGUILayout.TextField(keylist[i]);
                    if (n != keylist[i])
                    {
                        keylist[i] = n;
                        _ischanged = true;
                    }
                }

                if (_ischanged)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("FilterKeys", keylist);
                }
            }

            string[] assetPaths = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("AssetsPath");
            if (assetPaths != null && assetPaths.Length > 0)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("找到 " + assetPaths.Length + "个资源文件");
                GUILayout.EndVertical();
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
