using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{

    public enum NodeGUIModifyState
    {
        Default,
        InResizeMode
    }

    /// <summary>
    /// Node节点的GUI对象
    /// </summary>
    public class NodeGUI
    {

        private static Texture2D m_RefreshIcon;
        public static Texture2D RefreshIcon
        {
            get
            {
                if (m_RefreshIcon == null)
                {
                    m_RefreshIcon = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_REFRESH_ICON);
                }
                return m_RefreshIcon;
            }
        }

        public NodeGUI(INodeData nodeMData, INodeController controller, INodeGUIController GUIController)
        {
            m_data = nodeMData;
            m_controller = controller;
            m_GUIController = GUIController;
        }

        /// <summary>
        /// 是否Active;
        /// </summary>
        private bool _isActive = false;

        /// <summary>
        /// 最小显示大小
        /// </summary>
        public Vector2 MinSize
        {
            get { return m_GUIController.GetNodeMinSizeDefind(); }
        }

        public int id
        {
            get
            {
                if (m_data != null) return (int) m_data.id;
                return Int32.MinValue;
            }
        }

        public Vector2 size = new Vector2(NodeGraphDefind.NodeGUIMinSizeX, NodeGraphDefind.NodeGUIMinSizeY);
        public Vector2 position = Vector2.zero;
        public Rect rect
        {
            get
            {
                Vector2 minSize = MinSize;
                return new Rect(position.x, position.y, Mathf.Max(size.x, minSize.x), Mathf.Max(size.y, minSize.y));
            }
            set
            {
                position = new Vector2(value.x, value.y);
                Vector2 minSize = MinSize;
                size = new Vector2(Mathf.Max(value.width, minSize.x), Mathf.Max(value.height, minSize.y));
            }
        }

        private INodeController m_controller;
        public INodeController controller
        {
            get { return m_controller;}
        }

        private INodeGUIController m_GUIController;
        public INodeGUIController GUIController
        {
            get { return m_GUIController;}
        }

        private INodeData m_data;
        public INodeData data
        {
            get { return m_data;}
        }

        public void DrawNode(bool isMainNode = false)
        {
            if (NodeGraphBase.Instance != null)
            {
                GUIStyle style = isMainNode ? m_GUIController.GetNodeGUIBaseMainStyle(_isActive) : m_GUIController.GetNodeGUIBaseStyle(_isActive);
                rect = UnityEngine.GUILayout.Window((int)m_data.id, rect, DrawThisNode, m_GUIController.GetNodeLabel(), style);
            }
        }

        public void SetDirty(bool selfDirty = true, bool childrenDirty = true)
        {
            if (selfDirty)
            {
                m_data.ref_SetField_Inst_NonPublic("m_isDirty", true);
            }
            else
            {
                m_data.ref_SetField_Inst_NonPublic("m_isDirty", false);
            }

            if (childrenDirty)
            {
                //修改子节点标脏
                List<ConnectionPointGUI> cpList = m_GUIController.GetConnectionPointInfo(GetConnectionPointMode.OutputPoints);
                if (cpList != null)
                {
                    int i, len = cpList.Count;
                    for (i = 0; i < len; i++)
                    {
                        List<ConnectionGUI> clist = NodeGraphBase.Instance.GetContainsConnectionGUI(cpList[i]);
                        if (clist != null)
                        {
                            int i2, len2 = clist.Count;
                            for (i2 = 0; i2 < len2; i2++)
                            {
                                clist[i2].InputPointGui.node.SetDirty();
                            }
                        }
                    }
                }
            }
        }

        private NodeGUIModifyState _state = NodeGUIModifyState.Default;
        public NodeGUIModifyState state
        {
            get { return _state;}
        }

        private void DrawThisNode(int id)
        {
            _defaultEventHandle();
            HandleNodeEvent();

            GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            //do nothing ...
            GUILayout.EndVertical();

            DrawNodeConnectionPoint();

            //Todo 多选后同时移动的实现不好做啊 。。。
            if (_state != NodeGUIModifyState.InResizeMode && Event.current.button < 1 && NodeGraphBase.Instance.state != NodeGraphModifyState.ConnectionDraw)
                UnityEngine.GUI.DragWindow();
        }

        /**
			retrieve mouse events for this node in this AssetGraoh window.
		*/
        protected void DrawNodeConnectionPoint()
        {
            //绘制 ConnectionPointGUI
            List<ConnectionPointGUI> cpList = m_GUIController.GetConnectionPointInfo(GetConnectionPointMode.AllPoints);
            if (cpList != null && cpList.Count > 0)
            {
                int i, len = cpList.Count;
                for (i = 0; i < len; i++)
                {
                    //这里出于新能考虑没有过滤交叉引用 -- 
                    if (cpList[i].isInput && NodeGraphBase.Instance != null &&
                        NodeGraphBase.Instance.state == NodeGraphModifyState.ConnectionDraw &&
                        NodeGraphBase.Instance.GetDrawLineModeStartPoint() != null &&
                        NodeGraphBase.Instance.GetDrawLineModeStartPoint().DataTypeName == cpList[i].DataTypeName &&
                        NodeGraphBase.Instance.GetDrawLineModeStartPoint().node.data != cpList[i].node.data
                        )
                    {
                        cpList[i].DrawConnectionPoint(true);
                    }
                    else
                    {
                        cpList[i].DrawConnectionPoint();
                    }
                }
            }
        }

        //-------------------------------- Event handle

        private Vector2 _mouseDownPos;
        private Vector2 _mouseDragPos;

        private void HandleNodeEvent()
        {
            switch (Event.current.type)
            {
                /*
					handling release of mouse drag from this node to another node.
					this node doesn't know about where the other node is. the master only knows.
					only emit event.
				*/
                case EventType.Ignore:
                {
                    OnEventIgnore();
                    break;
                }

                /*
					handling drag.
				*/
                case EventType.MouseDrag:
                {
                    if (Event.current.button == 0)
                    {
                        _mouseDragPos += Event.current.delta;
                        OnMouseDrag();
                    }
                    break;
                }

                /*
					check if the mouse-down point is over one of the connectionPoint in this node.
					then emit event.
				*/
                case EventType.MouseDown:
                {
                    if (Event.current.button == 0)
                    {
                        _mouseDownPos = Event.current.mousePosition;
                        _mouseDragPos = _mouseDownPos;
                        OnMouseDown();
                    }
                    break;
                }
                default:
                    break;
            }

            /*
				retrieve mouse events for this node in|out of this AssetGraoh window.
			*/
            switch (Event.current.rawType)
            {
                case EventType.MouseUp:
                {
                    OnMouseUp();
                    if (new Rect(_mouseDownPos.x - NodeGraphDefind.MouseClickThresholdX*0.5f,_mouseDownPos.y - NodeGraphDefind.MouseClickThresholdY*0.5f,NodeGraphDefind.MouseClickThresholdX, NodeGraphDefind.MouseClickThresholdY).Contains(_mouseDragPos))
                    {
                        //OnClick
                        _defaultClickToActiveLogic();
                        OnClick(_mouseDragPos);
                        
                        if(NodeGraphBase.Instance != null)
                                NodeGraphBase.Instance.Repaint();
                    }
                    break;
                }
                default:
                    break;
            }

            /*
				right click to open Context menu
			*/
            // if (scaleFactor == SCALE_MAX)
            // {
            if (Event.current.type == EventType.ContextClick || (Event.current.type == EventType.MouseUp && Event.current.button == 1))
            {
                OnContextMenu();
            }
            // }
        }

        protected void _defaultClickToActiveLogic()
        {
            //单点激活切换逻辑
            if (NodeGraphBase.Instance != null)
            {
                if (Event.current.shift && Event.current.control)
                {
                    //交替模式
                    if (_isActive)
                    {
                        NodeGraphBase.Instance.RemoveActiveNodeGUI(this);
                    }
                    else
                    {
                        NodeGraphBase.Instance.AddActiveNodeGUI(this);
                    }
                    NodeGraphBase.Instance.ResetActiveNodeGUIWithoutList();
                }
                else if (Event.current.shift)
                {
                    //+ 模式
                    NodeGraphBase.Instance.AddActiveNodeGUI(this);
                    NodeGraphBase.Instance.ResetActiveNodeGUIWithoutList();
                }
                else if (Event.current.control)
                {
                    //- 模式
                    NodeGraphBase.Instance.RemoveActiveNodeGUI(this);
                    NodeGraphBase.Instance.ResetActiveNodeGUIWithoutList();
                }
                else
                {
                    //new 模式
                    if (!_isActive)
                    {
                        if (NodeGraphBase.Instance != null)
                        {
                            NodeGraphBase.Instance.ClearActiveNodeGUI();
                            NodeGraphBase.Instance.AddActiveNodeGUI(this);
                        }
                    }
                }
            }
        }

        protected void _defaultEventHandle()
        {

            Rect resizeIconRect = new Rect(rect.width - NodeGraphDefind.NodeGUIContentMarginX,
                                          rect.height - NodeGraphDefind.NodeGUIContentMarginY, 
                                          NodeGraphDefind.NodeGUIMinSizeX, 
                                          NodeGraphDefind.NodeGUIMinSizeY);

            GUI.Box(resizeIconRect, "", "box");

            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && resizeIconRect.Contains(Event.current.mousePosition))
                _state = NodeGUIModifyState.InResizeMode;

            if (_state == NodeGUIModifyState.InResizeMode && Event.current.type == EventType.MouseUp)
                _state = NodeGUIModifyState.Default;

            if (_state == NodeGUIModifyState.InResizeMode)
            {
                size = new Vector2(Mathf.Max(Event.current.mousePosition.x + 10, MinSize.x), Mathf.Max(Event.current.mousePosition.y + 10, MinSize.y));
            }

            if (m_data.isDirty)
            {
                Rect refreshIconRect = new Rect(0,
                                          rect.height - NodeGraphDefind.ModeGUIRefreshIconY,
                                          NodeGraphDefind.ModeGUIRefreshIconX,
                                          NodeGraphDefind.ModeGUIRefreshIconY);
                GUI.Box(refreshIconRect, new GUIContent(RefreshIcon, NodeGraphLagDefind.GetLabelDefine(12)));
            }

        }



#region ---------------- 事件

        protected void OnContextMenu()
        {
            m_GUIController.DrawNodeContextMenu();
        }

        protected void OnEventIgnore()
        {
            //NodeGUIUtility.FireNodeEvent(new OnNodeEvent(OnNodeEvent.EventType.EVENT_NODE_CONNECTION_OVERED, this, Event.current.mousePosition, null));
        }

        protected void OnMouseDown()
        {
            //                        var connectionPoints = WholeConnectionPoints();
            //                        var result = IsOverConnectionPoint(connectionPoints, Event.current.mousePosition);
            //
            //                        if (!string.IsNullOrEmpty(result))
            //                        {
            //                            if (scaleFactor == SCALE_MAX)
            //                            {
            //                                NodeGUIUtility.FireNodeEvent(new OnNodeEvent(OnNodeEvent.EventType.EVENT_NODE_CONNECT_STARTED, this, Event.current.mousePosition, result));
            //                            }
            //                            break;
            //                        }
        }

        protected void OnMouseUp()
        {
            //                        var connectionPoints = WholeConnectionPoints();
            //                        // if mouse position is on the connection point, emit mouse raised event.
            //                        foreach (var connectionPoint in connectionPoints)
            //                        {
            //                            var globalConnectonPointRect = new Rect(connectionPoint.buttonRect.x, connectionPoint.buttonRect.y, connectionPoint.buttonRect.width, connectionPoint.buttonRect.height);
            //                            if (globalConnectonPointRect.Contains(Event.current.mousePosition))
            //                            {
            //                                NodeGUIUtility.FireNodeEvent(new OnNodeEvent(OnNodeEvent.EventType.EVENT_NODE_CONNECTION_RAISED, this, Event.current.mousePosition, connectionPoint.pointId));
            //                                return;
            //                            }
            //                        }
            //
            //                        NodeGUIUtility.FireNodeEvent(new OnNodeEvent(OnNodeEvent.EventType.EVENT_NODE_TOUCHED, this, Event.current.mousePosition, null));
        }

        protected void OnMouseDrag()
        {
            //NodeGUIUtility.FireNodeEvent(new OnNodeEvent(OnNodeEvent.EventType.EVENT_NODE_MOVING, this, Event.current.mousePosition, null));
        }

        protected void OnClick(Vector2 MousePosition)
        {
            //Debug.Log("NodeGUI.OnClick");
        }

#endregion


#region ---------------- 方法
        
#endregion

    }
}