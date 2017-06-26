using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph {

    public enum ConnectionPointInoutType
    {
        Input,
        MutiInput,
        Output,
    }

    public class ConnectionPointGUI
    {
        
        private static Texture2D m_ConnectionPointInputBG;
        public static Texture2D ConnectionPointInputBG
        {
            get
            {
                // load shared connection textures
                if (m_ConnectionPointInputBG == null)
                {
                    m_ConnectionPointInputBG = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_INPUT_BG);
                }
                return m_ConnectionPointInputBG;
            }
        }

        private static Texture2D m_ConnectionPointOutputBG;
        public static Texture2D ConnectionPointOutputBG
        {
            get
            {
                // load shared connection textures
                if (m_ConnectionPointOutputBG == null)
                {
                    m_ConnectionPointOutputBG = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_OUTPUT_BG);
                }
                return m_ConnectionPointOutputBG;
            }
        }

        private static Texture2D m_ConnectionPointInput;
        public static Texture2D ConnectionPointInput
        {
            get
            {
                // load shared connection textures
                if (m_ConnectionPointInput == null)
                {
                    m_ConnectionPointInput = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_CONNECTIONPOINT_INPUT);
                }
                return m_ConnectionPointInput;
            }
        }

        private static Texture2D m_ConnectionPointOutput;
        public static Texture2D ConnectionPointOutput
        {
            get
            {
                // load shared connection textures
                if (m_ConnectionPointOutput == null)
                {
                    m_ConnectionPointOutput = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_CONNECTIONPOINT_OUTPUT);
                }
                return m_ConnectionPointOutput;
            }
        }

        private static Texture2D m_ConnectionPointInputLight;
        public static Texture2D ConnectionPointInputLight
        {
            get
            {
                // load shared connection textures
                if (m_ConnectionPointInputLight == null)
                {
                    m_ConnectionPointInputLight = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_CONNECTIONPOINT_ENABLE);
                }
                return m_ConnectionPointInputLight;
            }
        }

        private static Texture2D m_ConnectionPointOutputLight;
        public static Texture2D ConnectionPointOutputLight
        {
            get
            {
                // load shared connection textures
                if (m_ConnectionPointOutputLight == null)
                {
                    m_ConnectionPointOutputLight = NodeGraphTool.LoadTextureFromFile(NodeGraphDefind.RESOURCE_CONNECTIONPOINT_OUTPUT_CONNECTED);
                }
                return m_ConnectionPointOutputLight;
            }
        }

        public void Dispose()
        {
            node = null;
        }

        public long id;

        public int index; //(和totalLen一起，表示这个ConnectionPointGUI在NodeData数据点上排序位置)
        public int totalLen; //一共有几个

        public string label;
        public Vector2 labelSize;

        public string DataTypeName;
        public string propName;
//        public INodeData data;
        public NodeGUI node;
        
        private ConnectionPointInoutType m_InOutType;
        public ConnectionPointInoutType InOutType
        {
            get { return m_InOutType; }
        }

        public bool isInput
        {
            get { return (m_InOutType == ConnectionPointInoutType.Input || m_InOutType == ConnectionPointInoutType.MutiInput); }
        }

        public bool isOutput
        {
            get { return m_InOutType == ConnectionPointInoutType.Output; }
        }

        private bool m_isUsed;

        public bool isUsed
        {
            get { return m_isUsed; }
            set
            {
                //如果标识为多重输入点时，isUsed always false
                if (m_InOutType == ConnectionPointInoutType.MutiInput)
                {
                    m_isUsed = false;
                }
                else
                {
                    m_isUsed = value;
                }
            }
        }

        private Rect m_pointRect;
        public Rect pointRect
        {
            get { return m_pointRect; }
        }

        /// <summary>
        /// （提示，绘制连线时应使用此Rect）
        /// </summary>
        public Rect GlobalPointRect
        {
            get
            {
                UpdatePos();
                return new Rect(
                                    m_pointRect.x + node.position.x,
                                    m_pointRect.y + node.position.y + NodeGraphDefind.MenuLayoutHeight,
                                    m_pointRect.width,
                                    m_pointRect.height
                                );
            }
        }



        public ConnectionPointGUI(long id, int index, int totalLen, string DataTypeName, string propName, NodeGUI node, string label, Vector2 labelSize, ConnectionPointInoutType InOutType)
        {
            this.id = id;
            this.index = index;
            this.totalLen = totalLen;

            this.DataTypeName = DataTypeName;
            this.propName = propName;
            this.node = node;

            this.label = label;
            this.labelSize = labelSize;

            this.m_InOutType = InOutType;

            _PointLabelStyle = NodeGraphDefind.GetConnectPointLabelStyle(isOutput);
        }

        /// <summary>
        /// 更新所在位置
        /// </summary>
        public void UpdatePos()
        {
            //如果只有一个点，那就居中放
            if (totalLen == 1)
            {
                if (isOutput)
                {
                    m_pointRect = new Rect(node.size.x - NodeGraphDefind.OUTPUT_POINT_WIDTH,
                        (node.size.y - NodeGraphDefind.OUTPUT_POINT_HEIGHT)*0.5f,
                        NodeGraphDefind.OUTPUT_POINT_WIDTH,
                        NodeGraphDefind.OUTPUT_POINT_HEIGHT);
                }
                else
                {
                    m_pointRect = new Rect(0,
                        (node.size.y - NodeGraphDefind.OUTPUT_POINT_HEIGHT) *0.5f,
                        NodeGraphDefind.INPUT_POINT_WIDTH,
                        NodeGraphDefind.INPUT_POINT_HEIGHT);
                }
            }
            else
            {
                if (isOutput)
                {
                    m_pointRect = new Rect(node.size.x - NodeGraphDefind.OUTPUT_POINT_WIDTH,
                        NodeGraphDefind.OUTPUT_POINT_HEIGHT * index + NodeGraphDefind.NODE_TITLE_HEIGHT,
                        NodeGraphDefind.OUTPUT_POINT_WIDTH,
                        NodeGraphDefind.OUTPUT_POINT_HEIGHT);
                }
                else
                {
                    m_pointRect = new Rect(0,
                        NodeGraphDefind.OUTPUT_POINT_HEIGHT * index + NodeGraphDefind.NODE_TITLE_HEIGHT,
                        NodeGraphDefind.INPUT_POINT_WIDTH,
                        NodeGraphDefind.INPUT_POINT_HEIGHT);
                }
            }
        }

        private GUIStyle _PointLabelStyle;
        private Texture2D PointTex;
        private Texture2D markTex;

        /// <summary>
        /// 绘制ConnectionPoint
        /// </summary>
        public virtual void DrawConnectionPoint(bool enabled = false)
        {
            UpdatePos();

            
            if (m_InOutType == ConnectionPointInoutType.Input || m_InOutType == ConnectionPointInoutType.MutiInput)
            {
                PointTex = ConnectionPointInputBG;
            }
            else
            {
                PointTex = ConnectionPointOutputBG;
            }

            GUI.DrawTexture(
                        m_pointRect,
                        PointTex
                    );

            if (m_InOutType == ConnectionPointInoutType.Input || m_InOutType == ConnectionPointInoutType.MutiInput)
            {
                if (enabled || isUsed)
                {
                    markTex = ConnectionPointInputLight;
                }
                else
                {
                    markTex = ConnectionPointOutput;
                }
            }
            else
            {
                if (isUsed)
                {
                    markTex = ConnectionPointOutputLight;
                }
                else
                {
                    markTex = ConnectionPointOutput;
                }
            }

            float markSize = NodeGraphDefind.CONNECTION_INPUT_POINT_MARK_SIZE;
            if (m_InOutType == ConnectionPointInoutType.Output)
            {
                markSize = NodeGraphDefind.CONNECTION_OUTPUT_POINT_MARK_SIZE;
            }

            //画点
            Rect markRect = new Rect(
                                        (m_pointRect.width - markSize) * 0.5f + m_pointRect.x,
                                        (m_pointRect.height - markSize) * 0.5f + m_pointRect.y,
                                         markSize,
                                         markSize
                                    );

            GUI.DrawTexture(
                        markRect,
                        markTex
                    );

            //画Label
            Rect labelRect;
            if (isOutput)
            {
                //右边
                labelRect = new Rect(
                                        markRect.xMin - labelSize.x,
                                        markRect.center.y - labelSize.y * 0.5f,
                                        labelSize.x,
                                        labelSize.y
                                    );
            }
            else
            {
                //左边
                labelRect = new Rect(
                                        markRect.xMax,
                                        markRect.center.y - labelSize.y * 0.5f,
                                        labelSize.x,
                                        labelSize.y
                                    );
            }
            
            GUI.Label(labelRect, node.GUIController.GetConnectionPointLabel(this), _PointLabelStyle);

            if (Event.current.button == 0 && Event.current.type == EventType.mouseDown &&
                m_pointRect.Contains(Event.current.mousePosition))
            {
                //这里插头按下后触发
                if (NodeGraphBase.Instance != null)
                    NodeGraphBase.Instance.EnterDrawLineMode(this);
            }

        }

    }
}
