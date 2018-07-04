using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.NodeGraph.Utility;
using NodeGraph.SupportLib;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<hierarchy>#<object>#<rename>",
        "Framework.NodeGraph",
        "HierarchyObjRenamerData|HierarchyObjRenamerController|HierarchyObjRenamerGUIController",
        "Hierarchy Tools",-100)]
    public class HierarchyObjRenamerGUIController : NodeGUIController
    {

        private GUIStyle _guiContentTextStyle;

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("hierarchy") + NodeGraphLagDefind.Get("object") + NodeGraphLagDefind.Get("rename");
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

        private GUIStyle m_selectedLabelStyle;
        public GUIStyle selectedLabelStyle
        {
            get
            {
                if (m_selectedLabelStyle == null)
                {
                    m_selectedLabelStyle = GUI.skin.GetStyle("Label").Clone();
                    m_selectedLabelStyle.wordWrap = true;
                }
                return m_selectedLabelStyle;
            }
        }

        private GUIStyle m_selectedLabelStyleOn;
        public GUIStyle selectedLabelStyleOn
        {
            get
            {
                if (m_selectedLabelStyleOn == null)
                {
                    m_selectedLabelStyleOn = GUI.skin.GetStyle("Label").Clone();
                    m_selectedLabelStyleOn.wordWrap = true;
                    m_selectedLabelStyleOn.normal.textColor = Color.yellow;
                }
                return m_selectedLabelStyleOn;
            }
        }

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            NodeGraphUtility.Draw_NG_Toggle(nodeGUI.data, "UseEditorSelection", new GUIContent("UseEditorSelection"), (b) =>{
                m_nodeGUI.SetDirty();
            });

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.Label("renameKey 解析规则 :");
            GUILayout.Label("[n:数值] 数值表示该序列的固定位数");
            GUILayout.Label("[r?查找字符串:替换字符串] 原始GameObject名,附加字符替换功能");

            GUILayout.Space(5);

            GUILayout.EndVertical();

            NodeGraphUtility.Draw_NG_TextField(nodeGUI.data, "RenameKey", new GUIContent("RenameKey"), (s) =>
            {
                m_nodeGUI.SetDirty();
            });

            NodeGraphUtility.Draw_NG_IntField(nodeGUI.data, "ShotNum", new GUIContent("ShotNum"), (i) =>
            {
                m_nodeGUI.SetDirty();
            });

            if (GUILayout.Button("Update"))
            {
                m_nodeGUI.controller.update();
            }

            string[] resultInfoList = (string[])nodeGUI.data.ref_GetField_Inst_Public("ResultInfo");
            if (resultInfoList != null && resultInfoList.Length > 0)
            {

                GUILayout.Label("result : (" + resultInfoList.Length + ")");

                for (int i = 0; i < resultInfoList.Length; i++)
                {
                    GUILayout.Label(resultInfoList[i]);
                }

            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo (GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(int[]).Name, "InputInstanceIDs", m_nodeGUI,NodeGraphLagDefind.Get("input"),new Vector2(120, 60), ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(200, 0, 2, typeof(int[]).Name, "InstancesPath", m_nodeGUI,NodeGraphLagDefind.Get("output"),new Vector2(120, 60), ConnectionPointInoutType.Output);
                ConnectionPointGUI p2 = new ConnectionPointGUI(201, 1, 2, typeof(string[]).Name, "ResultInfo", m_nodeGUI,NodeGraphLagDefind.Get("resultInfo"),new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0,p1,p2 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}