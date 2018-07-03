using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AorBaseUtility;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<material>#<preprocessing>#<actions>",
        "Framework.NodeGraph",
        "ProcessMaterialActionData|ProcessMaterialActionController|ProcessMaterialActionGUIController", "Action")]
    public class ProcessMaterialActionGUIController : NodeGUIController
    {

        private static string[] _actionLabelDefine = {"无","修改共有属性","查找材质Shader丢失" };
        public static string[] Action3ComponentIDs = {"无"
                                                   ,"Renderer","MeshRenderer","SkinnedMeshRenderer","MeshFilter"
                                                   , "Collider", "Collider2D"
                                                   ,"ParticleSystem"
                                                   ,"Light","Camera"
                                                   ,"AudioSource"
                                                };

        private GUIStyle _describeStyle;
        private GUIStyle _resultInfoStyle;

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("material") + NodeGraphLagDefind.Get("preprocessing") + NodeGraphLagDefind.Get("actions");
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

            int aId = (int)m_nodeGUI.data.ref_GetField_Inst_Public("ActionId");
            int actionId = EditorGUILayout.Popup("预制处理动作", aId, _actionLabelDefine);
            if (actionId != aId)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ActionId", actionId);
                m_nodeGUI.SetDirty();
            }

                GUILayout.EndVertical();

           // base.DrawNodeInspector(inspectorWidth);
        }
    }

}
