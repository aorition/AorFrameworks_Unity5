using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{

    public enum GetConnectionPointMode
    {
        AllPoints,
        InputPoints,
        OutputPoints
    }

    /// <summary>
    /// NodeData GUI内容绘制器
    /// </summary>
    //[NodeToolItem("Test", "AddNodeBase")]
    public class NodeGUIController : INodeGUIController
    {

        public NodeGUIController(){}

        protected bool _isInit = false;
        public bool isInit {
            get { return _isInit; }
        }

        public void setup(NodeGUI nodeGui)
        {
            m_nodeGUI = nodeGui;
            _isInit = true;
        }

        protected NodeGUI m_nodeGUI;
        public NodeGUI nodeGUI
        {
            get { return m_nodeGUI; }
        }

        /// <summary>
        /// 获取该Node的ConnectionPoint的Label显示内容
        /// </summary>
        /// <returns></returns>
        public virtual string GetConnectionPointLabel(ConnectionPointGUI point)
        {
            return point.label + "(" + point.DataTypeName + ")";
        }

        /// <summary>
        /// 绘制NodeData的GUI显示内容
        /// (*** 请自行扩展子类)
        /// (废弃) NodeGUI没有必要显示此自定义内容
        /// </summary>
//        public virtual void DrawNodeGUIContent()
//        {
//            if (m_nodeGUI == null) return;
//            //
//
//            //默认外框
//            AorGUILayout.Vertical("box",() =>
//            {
//                GUILayout.FlexibleSpace();
//                AorGUILayout.Horizontal(() =>
//                {
//                    GUILayout.FlexibleSpace();
//                    GUILayout.Label(m_nodeGUI.data.id.ToString());
//                    GUILayout.FlexibleSpace();
//                });
//                GUILayout.FlexibleSpace();
//            });
//        }
//        
        public virtual GUIStyle GetNodeGUIBaseStyle(bool isActive = false)
        {
            return NodeGraphDefind.GetNodeGUIBaseStyle(isActive);
        }

        public virtual GUIStyle GetNodeGUIBaseMainStyle(bool isActive = false)
        {
            return NodeGraphDefind.GetNodeGUIBaseMainStyle(isActive);
        }

        protected virtual GUIStyle GetConnectCenterTipStyle()
        {
            return NodeGraphDefind.GetConnectCenterTipBaseStyle();
        }

        protected List<ConnectionPointGUI> _ConnectionPointGUIList;
        /// <summary>
        /// 获取此Node定义的最小显示尺寸
        /// (*** 请自行扩展此方法)
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetNodeMinSizeDefind()
        {
            return new Vector2(NodeGraphDefind.NodeGUIMinSizeX, NodeGraphDefind.NodeGUIMinSizeY);
        }

        /// <summary>
        /// 获取此Node上ConnectionPoint List
        /// (*** 此方法声明此Node的输入点和输出点，请自行扩展此方法)
        /// </summary>
        public virtual List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 2, typeof(int).Name, "id", m_nodeGUI, "Input", new Vector2(100, 60), ConnectionPointInoutType.Input);
                ConnectionPointGUI p0a = new ConnectionPointGUI(101, 1, 2, typeof(int).Name, "id", m_nodeGUI, "MutiInput", new Vector2(100, 60), ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(102, 0, 1, typeof(int).Name, "id", m_nodeGUI, "Output", new Vector2(100, 60), ConnectionPointInoutType.Output); 
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0, p0a, p1 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }

        protected List<ConnectionPointGUI> _GetConnectionPointsByMode(GetConnectionPointMode GetMode)
        {
            switch (GetMode)
            {
                case GetConnectionPointMode.InputPoints:
                    return _ConnectionPointGUIList.FindAll((t) =>
                    {
                        return t.isInput;
                    });
                case GetConnectionPointMode.OutputPoints:
                    return _ConnectionPointGUIList.FindAll((t) =>
                    {
                        return t.isOutput;
                    });
                default:
                    return _ConnectionPointGUIList;
            }
        }

        /// <summary>
        /// 绘制NodeData的Inspector显示内容
        /// (*** 请自行扩展此方法)
        /// </summary>
        public virtual void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));
            GUILayout.Label("Inspector : " + m_nodeGUI.data.name);

            GUILayout.Label("DebugInfo:" + m_nodeGUI.size.x + " x " + m_nodeGUI.size.y);

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制NodeData得ContextMenu
        /// (*** 请自行扩展此方法)
        /// </summary>
        public virtual void DrawNodeContextMenu()
        {

            if (m_nodeGUI == null) return;

            var menu = new GenericMenu();
            menu.AddItem(
                new GUIContent(NodeGraphLagDefind.GetLabelDefine(14)),
                false,
                () => {
                    if (NodeGraphBase.Instance != null)
                    {
                        NodeGraphBase.Instance.RemoveNodeGUI(m_nodeGUI);
                    }
                }
            );
            if (NodeGraphBase.Instance != null)
            {
                if (NodeGraphBase.Instance.MainNode != null && m_nodeGUI == NodeGraphBase.Instance.MainNode)
                {
                    menu.AddItem(
                        //取消MianNode
                        new GUIContent(NodeGraphLagDefind.GetLabelDefine(16)),
                        false,
                        () =>
                        {
                            if (NodeGraphBase.Instance != null)
                            {
                                NodeGraphBase.Instance.SetMainNode(null);
                            }
                        }
                        );
                }
                else
                {
                    menu.AddItem(
                        //设置MainNode
                        new GUIContent(NodeGraphLagDefind.GetLabelDefine(15)),
                        false,
                        () =>
                        {
                            if (NodeGraphBase.Instance != null)
                            {
                                NodeGraphBase.Instance.SetMainNode(m_nodeGUI);
                            }
                        }
                        );
                }
            }
            
            menu.ShowAsContext();
            Event.current.Use();
        }

        /// <summary>
        /// 获取NodeData标签
        /// (*** 请自行扩展此方法)
        /// </summary>
        /// <returns></returns>
        public virtual string GetNodeLabel()
        {
            return m_nodeGUI.data.name;
        }

        /// <summary>
        /// 绘制ConnectionTip 
        /// (*** 请自行扩展此方法， 提示：显示内容为string，长度最好不要大于20)
        /// </summary>
        public virtual void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {

            string info = m_nodeGUI.data.name;
            
            //size
            Vector2 CTSzie = new Vector2(NodeGraphTool.GetConnectCenterTipLabelWidth(info), NodeGraphDefind.ConnectCenterTipLabelPreHeight);
            
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

        

        /// <summary>
        /// 绘制CenterTipContextMenu
        /// (*** 请自行扩展此方法)
        /// </summary>
        public virtual void DrawCenterTipContextMenu(ConnectionGUI connection)
        {
            if (m_nodeGUI == null) return;

            var menu = new GenericMenu();
            //删除连线
            menu.AddItem(
                new GUIContent("DeleteConnection"),
                false,
                () => {
                    if (NodeGraphBase.Instance != null)
                    {
                        NodeGraphBase.Instance.RemoveConnection(connection);
                    }
                }
            );
            menu.ShowAsContext();
        }

    }
}
