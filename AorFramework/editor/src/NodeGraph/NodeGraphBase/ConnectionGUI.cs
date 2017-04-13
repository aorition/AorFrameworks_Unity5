using System;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class ConnectionGUI
    {

        private class ConnectionGUISingleton
        {
           // public Action<OnConnectionEvent> emitAction;

            public Texture2D connectionArrowTex;

            private static ConnectionGUISingleton s_singleton;

            public static ConnectionGUISingleton s
            {
                get
                {
                    if (s_singleton == null)
                    {
                        s_singleton = new ConnectionGUISingleton();
                    }

                    return s_singleton;
                }
            }
        }

        public static Texture2D connectionArrowTex
        {
            get
            {
                // load shared connection textures
                if (ConnectionGUISingleton.s.connectionArrowTex == null)
                {
                    ConnectionGUISingleton.s.connectionArrowTex = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_ARROW);
                }
                return ConnectionGUISingleton.s.connectionArrowTex;
            }
        }

        //------------->

        private ConnectionPointGUI m_OutputPointGui;
        public ConnectionPointGUI OutputPointGui
        {
            get { return m_OutputPointGui; }
        }
        private ConnectionPointGUI m_InputPointGui;
        public ConnectionPointGUI InputPointGui
        {
            get { return m_InputPointGui; }
        }

        private Vector3 m_center;

        /// <summary>
        /// CenterTipRect
        /// </summary>
        public Rect CenterRect;

        public ConnectionGUI(ConnectionPointGUI output, ConnectionPointGUI input)
        {
//            conInsp = ScriptableObject.CreateInstance<ConnectionGUIInspectorHelper>();
//            conInsp.hideFlags = HideFlags.DontSave;

            this.m_OutputPointGui = output;
            this.m_InputPointGui = input;

        }

        public void Dispose()
        {
            m_OutputPointGui = null;
            m_InputPointGui = null;
        }

        /// <summary>
        /// 标识连线两点是否用过
        /// </summary>
        public void SetPointUsed(bool isUsed)
        {
            m_InputPointGui.isUsed = isUsed;
            m_OutputPointGui.isUsed = isUsed;
        }

        public object GetConnectionValue(bool updateParentLoop = true)
        {

            string propName = m_OutputPointGui.propName;

            if (updateParentLoop)
            {
                //建立上联列表
                List<NodeGUI> shortList = new List<NodeGUI>();
                _dataUpdateLoop(m_InputPointGui, ref shortList);

                if (shortList != null)
                {
                    //反转
                    shortList.Reverse();

                    //尝试更新上联列表中的Node
                    int i, len = shortList.Count;
                    for (i = 0; i < len; i++)
                    {

                        if (shortList[i].data.isDirty)
                        {
                            //注意 列表内不可以遍历更新上级节点，否则引起过量递归计算。
                            shortList[i].controller.update(false);
                        }

                    }

                }

            }

            return m_OutputPointGui.node.data.ref_GetField_Inst_Public(propName);
        }

        private void _dataUpdateLoop(ConnectionPointGUI inputPointGui,ref List<NodeGUI> shortList)
        {
            if(shortList == null) shortList = new List<NodeGUI>();
            List<ConnectionGUI> clist = NodeGraphBase.Instance.GetContainsConnectionGUI(inputPointGui);
            if (clist != null)
            {
                int i, len = clist.Count;
                for (i = 0; i < len; i++)
                {
                    ConnectionGUI c = clist[i];
                    if (!shortList.Contains(c.OutputPointGui.node))
                    {
                        shortList.Add(c.OutputPointGui.node);

                        List<ConnectionPointGUI> opList = c.OutputPointGui.node.GUIController.GetConnectionPointInfo(GetConnectionPointMode.InputPoints);
                        if (opList != null)
                        {
                            int i2, len2 = opList.Count;
                            for (i2 = 0; i2 < len2; i2++)
                            {
                                _dataUpdateLoop(opList[i2], ref shortList);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绘制连线
        /// </summary>
        public void DrawConnection()
        {
            if (NodeGraphBase.Instance == null) return;
            //绘制连线
            Rect sRect = m_OutputPointGui.GlobalPointRect;
//            Vector3 startV3 = new Vector3(
//                sRect.x + sRect.width * 0.5f - NodeGraphBase.Instance.NodeGraphCanvasScrollPos.x,
//                sRect.y + sRect.height * 0.5f - NodeGraphBase.Instance.NodeGraphCanvasScrollPos.y + NodeGraphDefind.MenuLayoutHeight,
//                0f);
            Vector3 startV3 = new Vector3(
                sRect.x + sRect.width * 0.5f,
                sRect.y + sRect.height * 0.5f - NodeGraphDefind.MenuLayoutHeight,
                0f);

            Rect eRect = m_InputPointGui.GlobalPointRect;
            Vector3 endV3 = new Vector3(
                eRect.x + eRect.width * 0.5f,
                eRect.y + sRect.height * 0.5f - NodeGraphDefind.MenuLayoutHeight,
                0f);

            //中点
            m_center = startV3 + ((endV3 - startV3) / 2);

            float pointDistance = (endV3.x - startV3.x) / 3f;
            if (pointDistance < NodeGraphDefind.CONNECTION_CURVE_LENGTH) pointDistance = NodeGraphDefind.CONNECTION_CURVE_LENGTH;

            Vector3 startTan = new Vector3(startV3.x + pointDistance, startV3.y, 0f);
            Vector3 endTan = new Vector3(endV3.x - pointDistance, endV3.y, 0f);

            //绘制 连线
            Handles.DrawBezier(startV3, endV3, startTan, endTan, Color.gray, null, 4f);

            //-- 绘制箭头
            GUI.DrawTexture(
                new Rect(
                    endV3.x - NodeGraphDefind.CONNECTION_ARROW_WIDTH + 4f,
                    endV3.y - (NodeGraphDefind.CONNECTION_ARROW_HEIGHT / 2f) - 1f,
                    NodeGraphDefind.CONNECTION_ARROW_WIDTH,
                    NodeGraphDefind.CONNECTION_ARROW_HEIGHT
                ),
                ConnectionGUI.connectionArrowTex
            );

            //DrawCenterTip
            if (NodeGraphBase.Instance != null)
            {
                m_OutputPointGui.node.GUIController.DrawConnectionTip(m_center, this);
            }

        }

    }

}
