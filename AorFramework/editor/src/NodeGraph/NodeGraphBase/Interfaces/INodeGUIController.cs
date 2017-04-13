using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public interface INodeGUIController
    {
        NodeGUI nodeGUI { get; }

        void setup(NodeGUI node);

        bool isInit { get; }

        string GetNodeLabel();

        string GetConnectionPointLabel(ConnectionPointGUI point);

        //(废弃)       void DrawNodeGUIContent();

        void DrawNodeInspector(float inspectorWidth);

        void DrawNodeContextMenu();

        List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode);

        void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection);

        void DrawCenterTipContextMenu(ConnectionGUI connection);

        Vector2 GetNodeMinSizeDefind();

        GUIStyle GetNodeGUIBaseStyle(bool isActive = false);

        GUIStyle GetNodeGUIBaseMainStyle(bool isActive = false);

    }
}
