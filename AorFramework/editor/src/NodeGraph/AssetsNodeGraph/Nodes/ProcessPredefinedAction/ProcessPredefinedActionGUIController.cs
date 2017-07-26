using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.NodeGraph.Tool;
using AorFramework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("ProcessPredefinedAction",
        "AorFramework.NodeGraph",
        "ProcessPredefinedActionData|ProcessPredefinedActionController|ProcessPredefinedActionGUIController")]
    public class ProcessPredefinedActionGUIController : NodeGUIController
    {

        private static string[] _actionLabelDefine = {"无","FindMissing", "FindActiveFalse", "FindComponent" };
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
            return "ProcessPredefinedAction";
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
                ConnectionPointGUI p1 = new ConnectionPointGUI(100, 0, 1, typeof(int).Name, "ActionId", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(8), new Vector2(100, 60), ConnectionPointInoutType.Output);

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
               // m_nodeGUI.SetDirty();
            }

            switch (actionId)
            {
                case 3:


                    bool useAction3Custom = NodeGraphUtility.Draw_NG_Toggle(m_nodeGUI.data, "Action3UseCustomScript",new GUIContent("自定义MonoBehaviour脚本"));
                    GUILayout.Label("Component : ");

                    if (useAction3Custom)
                    {
                        string oguid = (string)m_nodeGUI.data.ref_GetField_Inst_Public("Action3CustomScriptGUID");
                        if (!string.IsNullOrEmpty(oguid))
                        {
                            string customPath = AssetDatabase.GUIDToAssetPath(oguid);
                            if (!string.IsNullOrEmpty(customPath))
                            {

                                UnityEngine.Object custom = AssetDatabase.LoadMainAssetAtPath(customPath);
                                if (custom != null)
                                {
                                    UnityEngine.Object n = EditorGUILayout.ObjectField(custom, typeof(MonoScript), false);
                                    if (n == null)
                                    {
                                        m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", null);
                                        m_nodeGUI.SetDirty();
                                    }
                                    else if (n != custom)
                                    {
                                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(n));
                                        if (!string.IsNullOrEmpty(guid))
                                        {
                                            m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", guid);
                                            m_nodeGUI.SetDirty();
                                        }
                                        else
                                        {
                                            m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", null);
                                            m_nodeGUI.SetDirty();
                                        }
                                    }
                                }
                                else
                                {
                                    m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", null);
                                    m_nodeGUI.SetDirty();
                                }
                            }
                            else
                            {
                                m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", null);
                                m_nodeGUI.SetDirty();
                            }
                        }
                        else
                        {
                            UnityEngine.Object n = EditorGUILayout.ObjectField(null, typeof(MonoScript), false);
                            if (n != null)
                            {
                                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(n));
                                if (!string.IsNullOrEmpty(guid))
                                {
                                    m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", guid);
                                    m_nodeGUI.SetDirty();
                                }
                                else
                                {
                                    m_nodeGUI.data.ref_SetField_Inst_Public("Action3CustomScriptGUID", null);
                                    m_nodeGUI.SetDirty();
                                }
                            }
                        }
                    }
                    else
                    {
                        //Action3ScriptID
                        NodeGraphUtility.Draw_NG_Popup(m_nodeGUI.data, "Action3ComponentID", Action3ComponentIDs);
                    }
                    
                    break;
                default:
                    break;
            }


            GUILayout.EndVertical();

           // base.DrawNodeInspector(inspectorWidth);
        }
    }

}
