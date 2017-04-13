using System;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class NodeGraphToolItemDefinder
    {

        #region ToolItem指令集合, 所有方法必须带有Vector2作为输入坐标的形参

        /// <summary>
        /// 创建NodeBase
        /// </summary>
        /// <param name="inputPos"></param>
        public static void AddNodeBase(Vector2 inputPos)
        {
            NodeData nodedata = new NodeData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(NodeData).Name;

            NodeController ctrl = new NodeController();
            NodeGUIController GUICtrl = new NodeGUIController();

            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos - new Vector2(node.size.x, node.size.y / 2);

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

        #endregion


    }
}
