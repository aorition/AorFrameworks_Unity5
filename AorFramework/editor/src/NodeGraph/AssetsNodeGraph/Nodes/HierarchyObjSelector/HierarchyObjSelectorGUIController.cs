using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("Hierarchy #<12>", 
        "AorFramework.NodeGraph.AssetNodeGraphToolItemDefinder", 
        "CreateHierarchySelector",
        "Hierarchy Tools")]
    public class HierarchyObjSelectorGUIController : NodeGUIController
    {

        private GUIStyle _guiContentTextStyle;

        public override string GetNodeLabel()
        {
            return "Hierarchy" + AssetNodeGraphLagDefind.GetLabelDefine(11);
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
            string[] assetList = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SelectedInstanceIDs");
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

        private void AddSelectionGameObjects()
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {

                List<int> nIIDs = new List<int>();

                int[] cIIDs = (int[])m_nodeGUI.data.ref_GetField_Inst_Public("SelectedInstanceIDs");
                if (cIIDs != null && cIIDs.Length > 0)
                {
                    nIIDs.AddRange(cIIDs);
                }

                int i, len = Selection.gameObjects.Length;
                for (i = 0; i < len; i++)
                {
                  //  Selection.gameObjects[i]
                    int iid = Selection.gameObjects[i].GetInstanceID();
                    nIIDs.Add(iid);
                }

                m_nodeGUI.data.ref_SetField_Inst_Public("SelectedInstanceIDs", nIIDs.ToArray());
                m_nodeGUI.SetDirty();
            }
        }

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            List<int> selectedInstanceIDs;
            int[] d = (int[])m_nodeGUI.data.ref_GetField_Inst_Public("SelectedInstanceIDs");
            if (d != null)
            {
                selectedInstanceIDs = new List<int>(d);
            }
            else
            {
                selectedInstanceIDs = new List<int>();
            }

            if (selectedInstanceIDs.Count > 0)
            {

                GUILayout.Label("Selected : ");

                GUILayout.BeginVertical("box");

                int i, len = selectedInstanceIDs.Count;
                for (i = 0; i < len; i++)
                {
                    //GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(selectedInstanceIDs[i]));
                    GameObject go = (GameObject)EditorUtility.InstanceIDToObject(selectedInstanceIDs[i]);
                    if (go)
                    {
                        if (GUILayout.Button(go.name + "[" + go.getHierarchyPath() + "]","label"))
                        {
                            Selection.activeGameObject = go;
                        }
                    }
                }

                GUILayout.EndVertical();

            }
            
            if (GUILayout.Button("AddSelection"))
            {
                AddSelectionGameObjects();
            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo (GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(int[]).Name, "SelectedInstanceIDs", m_nodeGUI,AssetNodeGraphLagDefind.GetLabelDefine(8),new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }
    }
}
