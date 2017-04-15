using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetNodeGraphToolItemDefinder
    {

        #region ToolItem指令集合, 所有方法必须带有Vector2作为输入坐标的形参

        /// <summary>
        /// 创建NodeBase
        /// </summary>
        /// <param name="inputPos"></param>
        public static void CreateAssetSearch(Vector2 inputPos)
        {
            //---创建 AssetSearch
            AssetSearchData nodedata = new AssetSearchData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetSearchData).Name;

            AssetSearchController ctrl = new AssetSearchController();
            AssetSearchGUIController GUICtrl = new AssetSearchGUIController();

            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos - new Vector2(node.size.x, node.size.y / 2);

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

        public static void CreateAssetFilter(Vector2 inputPos)
        {
            //---创建 AssetSearch
            AssetFilterData nodedata = new AssetFilterData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetFilterData).Name;

            AssetFilterController ctrl = new AssetFilterController();
            AssetFilterGUIController GUICtrl = new AssetFilterGUIController();

            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos;

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

        public static void CreateAssetPrefabProcessor(Vector2 inputPos)
        {
            //---创建 AssetPrefabProcessor
            AssetPrefabProcessorData nodedata = new AssetPrefabProcessorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetPrefabProcessorData).Name;

            AssetPrefabProcessorController ctrl = new AssetPrefabProcessorController();
            AssetPrefabProcessorGUIController GUICtrl = new AssetPrefabProcessorGUIController();

            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos;

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

        public static void CreatePrefabProcessor(Vector2 inputPos)
        {
            //---创建 PrefabProcessor
            PrefabProcessorData nodedata = new PrefabProcessorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(PrefabProcessorData).Name;

            PrefabProcessorController ctrl = new PrefabProcessorController();
            PrefabProcessorGUIController GUICtrl = new PrefabProcessorGUIController();

            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos;

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

        public static void CreateHierarchySelector(Vector2 inputPos)
        {
            //---创建 HierarchySelector
            HierarchyObjSelectorData nodedata = new HierarchyObjSelectorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(HierarchyObjSelectorData).Name;

            HierarchyObjSelectorController ctrl = new HierarchyObjSelectorController();
            HierarchyObjSelectorGUIController GUICtrl = new HierarchyObjSelectorGUIController();

            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos;

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

        #endregion
    }
}
