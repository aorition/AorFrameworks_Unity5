using System;
using System.Collections.Generic;
using Framework.NodeGraph.Utility;
using UnityEngine;
using AorBaseUtility.Extends;

namespace Framework.NodeGraph
{
    [NodeToolItem("Texture#<preprocessing>#<actions>",
        "Framework.NodeGraph",
        "ProcessPredefinedActionData|TextureProcessPredefinedActionController|TextureProcessPredefinedActionGUIController",
        "Texture")]
    public class TextureProcessPredefinedActionGUIController : NodeGUIController
    {

        private string[] _actionLabelDefine = {"无","Action1", "Action2" };

        private GUIStyle _describeStyle;
        private GUIStyle _resultInfoStyle;

        public override string GetNodeLabel()
        {
            return "Texture" + NodeGraphLagDefind.Get("preprocessing") + NodeGraphLagDefind.Get("actions");
        }

        private Vector2 _NodeMinSize = new Vector2(230, 120);
        public override Vector2 GetNodeMinSizeDefind()
        {
            return _NodeMinSize;
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p1 = new ConnectionPointGUI(100, 0, 1, typeof(int).Name, "ActionId", m_nodeGUI, NodeGraphLagDefind.Get("output"), new Vector2(100, 60), ConnectionPointInoutType.Output);

                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

        public override void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {
            //string
            int actionId = (int)m_nodeGUI.data.ref_GetField_Inst_Public("ActionId");
            string info = _actionLabelDefine[actionId];
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

            NodeGraphUtility.Draw_NG_Popup(m_nodeGUI.data, "ActionId", _actionLabelDefine);
//            switch (actionId)
//            {
//                //Action2 附加参数GUI显示
//                case 3:
//                    break;
//                default:
//                    break;
//            }


            GUILayout.EndVertical();

           // base.DrawNodeInspector(inspectorWidth);
        }
    }

}
