using System;
using System.Collections.Generic;
using AorBaseUtility.Extends;
using Framework.Extends;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<hierarchy>#<selector>",
        "Framework.NodeGraph",
        "HierarchyObjSelectorData|HierarchyObjSelectorController|HierarchyObjSelectorGUIController",
        "Hierarchy Tools",-100)]
    public class HierarchyObjSelectorGUIController : NodeGUIController
    {

        private GUIStyle _guiContentTextStyle;

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("hierarchy") + NodeGraphLagDefind.Get("selector");
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

        private void ClearInnerSelected()
        {

            m_nodeGUI.data.ref_SetField_Inst_Public("SelectedInstanceIDs", null);
            m_nodeGUI.SetDirty();

            _innerActiveSet.Clear();
        }

        private void SelectToInnerSelectedItem()
        {
            List<GameObject> gos = new List<GameObject>();
            foreach (int i in _innerActiveSet)
            {
                GameObject go = (GameObject) EditorUtility.InstanceIDToObject(i);
                if (go)
                {
                    gos.Add(go);
                }
            }

            if (gos.Count > 0)
            {
                Selection.objects = gos.ToArray();
            }

        }

        private void DeleteInnerSelectedItem()
        {

            List<int> nIIDs = new List<int>();

            int[] cIIDs = (int[])m_nodeGUI.data.ref_GetField_Inst_Public("SelectedInstanceIDs");
            if (cIIDs != null && cIIDs.Length > 0)
            {
                nIIDs.AddRange(cIIDs);
            }

            foreach (int i in _innerActiveSet)
            {
                if (nIIDs.Contains(i))
                {
                    nIIDs.Remove(i);
                }
            }
            _innerActiveSet.Clear();

            m_nodeGUI.data.ref_SetField_Inst_Public("SelectedInstanceIDs", nIIDs.ToArray());
            m_nodeGUI.SetDirty();

        }

        private void SelectAllInHierarchy()
        {

            List<int> nIIDs = new List<int>();

            int i, len = SceneManager.sceneCount;
            for (i = 0; i < len; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    GameObject[] roots = scene.GetRootGameObjects();
                    if (roots != null && roots.Length > 0)
                    {
                        for (int a = 0; a < roots.Length; a++)
                        {
                            GameObject root = roots[a];

                            _addGameObjectsToList(root, true, ref nIIDs);
                        }
                    }
                }
            }

            m_nodeGUI.data.ref_SetField_Inst_Public("SelectedInstanceIDs", nIIDs.ToArray());
            m_nodeGUI.SetDirty();
        }

        private void AddSelectionGameObjects(bool incudeChildren = false)
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
                    _addGameObjectsToList(Selection.gameObjects[i], incudeChildren, ref nIIDs);
                }

                m_nodeGUI.data.ref_SetField_Inst_Public("SelectedInstanceIDs", nIIDs.ToArray());
                m_nodeGUI.SetDirty();
            }
        }

        private void _addGameObjectsToList(GameObject go, bool incudeChildren, ref List<int> nIIDs)
        {
            int iid = go.GetInstanceID();
                    if (!nIIDs.Contains(iid))
                    {
                        nIIDs.Add(iid);
                    }
                    if (incudeChildren)
                    {
                _addAddChildrenLoop(go.transform, ref nIIDs);
            }
        }

        private void _addAddChildrenLoop(Transform tran, ref List<int> list)
        {
            if (tran.childCount > 0)
            {
                int i, len = tran.childCount;
                for (i = 0; i < len; i++)
                {
                    Transform sub = tran.GetChild(i);
                    int iid = sub.gameObject.GetInstanceID();
                    if (!list.Contains(iid))
                    {
                        list.Add(iid);
                    }
                    _addAddChildrenLoop(sub, ref list);
                }
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

        private bool _incudeChilrenOnAddSelect = false;
        private HashSet<int> _innerActiveSet = new HashSet<int>(); 

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            if (GUILayout.Button("SelectAllInHierarchy"))
            {
                SelectAllInHierarchy();
            }

            GUILayout.BeginHorizontal();

            _incudeChilrenOnAddSelect = EditorGUILayout.ToggleLeft("IncChildren", _incudeChilrenOnAddSelect);

            if (GUILayout.Button("AddSelection"))
            {
                AddSelectionGameObjects(_incudeChilrenOnAddSelect);
            }

            if (GUILayout.Button("ClearInnerSelected"))
            {
                if (EditorUtility.DisplayDialog("提示", "取消所选对象?", "OK", "Cancel"))
                {
                    ClearInnerSelected();
                }
            }

            GUILayout.EndHorizontal();

            if (_innerActiveSet.Count > 0)
            {
                _draw_btnAfterSelected(true);
            }
            else
            {
                _draw_btnAfterSelected(false);
            }

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

                GUILayout.Label("Selected " + selectedInstanceIDs.Count + " items : ");

                GUILayout.BeginVertical("box");

                int i, len = selectedInstanceIDs.Count;
                for (i = 0; i < len; i++)
                {
                    int currentIID = selectedInstanceIDs[i];
                    //GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(selectedInstanceIDs[i]));
                    GameObject go = (GameObject)EditorUtility.InstanceIDToObject(currentIID);
                    if (go)
                    {
                        if (_innerActiveSet.Contains(currentIID))
                        {
                            //选定的
                            if (GUILayout.Button(go.name + "[" + go.getHierarchyPath() + "]", selectedLabelStyleOn))
                            {
                                //                                Selection.activeGameObject = go;
                                _innerActiveSet.Remove(currentIID);
                            }
                        }
                        else
                        {
                            //未选定
                            if (GUILayout.Button(go.name + "[" + go.getHierarchyPath() + "]", selectedLabelStyle))
                            {
                                //Selection.activeGameObject = go;
                                _innerActiveSet.Add(currentIID);
                            }
                        }
                    }
                }

                GUILayout.EndVertical();

            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo (GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(int[]).Name, "SelectedInstanceIDs", m_nodeGUI,NodeGraphLagDefind.Get("output"),new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }

        private void _draw_btnAfterSelected(bool active)
        {
            if (active)
            {

                if (GUILayout.Button("SelectToInnerSelectedItem"))
                {
                    SelectToInnerSelectedItem();
                }

                if (GUILayout.Button("DeleteInnerSelectedItem"))
                {
                    if (EditorUtility.DisplayDialog("提示", "确认移除所选对象?", "OK", "Cancel"))
                    {
                        DeleteInnerSelectedItem();
                    }
                }
            }
            else
            {

                GUI.color = Color.gray;

                if (GUILayout.Button("SelectToInnerSelectedItem"))
                {
                    //do nothing ...
                }

                if (GUILayout.Button("DeleteInnerSelectedItem"))
                {
                    //do nothing ...
                }
                GUI.color = Color.white;
            }
        }

    }
}
