using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class AssetNodeGraphToolItemDefinder
    {

        private static void CreateNode(Vector2 inputPos, INodeData nodedata, INodeController ctrl, INodeGUIController GUICtrl)
        {
            NodeGUI node = new NodeGUI(nodedata, ctrl, GUICtrl);
            node.position = inputPos;

            ctrl.setup(node);
            GUICtrl.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

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

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreateAssetFilter(Vector2 inputPos)
        {
            //---创建 AssetSearch
            AssetFilterData nodedata = new AssetFilterData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetFilterData).Name;

            AssetFilterController ctrl = new AssetFilterController();
            AssetFilterGUIController GUICtrl = new AssetFilterGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreateAssetPrefabProcessor(Vector2 inputPos)
        {
            //---创建 AssetPrefabProcessor
            AssetPrefabProcessorData nodedata = new AssetPrefabProcessorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetPrefabProcessorData).Name;

            AssetPrefabProcessorController ctrl = new AssetPrefabProcessorController();
            AssetPrefabProcessorGUIController GUICtrl = new AssetPrefabProcessorGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreatePrefabProcessor(Vector2 inputPos)
        {
            //---创建 PrefabProcessor
            PrefabProcessorData nodedata = new PrefabProcessorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(PrefabProcessorData).Name;

            PrefabProcessorController ctrl = new PrefabProcessorController();
            PrefabProcessorGUIController GUICtrl = new PrefabProcessorGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreateHierarchySelector(Vector2 inputPos)
        {
            //---创建 HierarchySelector
            HierarchyObjSelectorData nodedata = new HierarchyObjSelectorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(HierarchyObjSelectorData).Name;

            HierarchyObjSelectorController ctrl = new HierarchyObjSelectorController();
            HierarchyObjSelectorGUIController GUICtrl = new HierarchyObjSelectorGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreateAssetProcessor(Vector2 inputPos)
        {
            //---创建 AssetProcessor
            AssetProcessorData nodedata = new AssetProcessorData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetProcessorData).Name;

            AssetProcessorController ctrl = new AssetProcessorController();
            AssetProcessorGUIController GUICtrl = new AssetProcessorGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreateAssetDepend(Vector2 inputPos)
        {
            //---创建 AssetDepend
            AssetDependData nodedata = new AssetDependData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(AssetDependData).Name;

            AssetDependController ctrl = new AssetDependController();
            AssetDependGUIController GUICtrl = new AssetDependGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        public static void CreateInfoOutput(Vector2 inputPos)
        {
            //---创建 InfoOutput
            InfoOutputTxtData nodedata = new InfoOutputTxtData(NodeGraphBase.Instance.GetNewNodeID());
            nodedata.name = typeof(InfoOutputTxtData).Name;

            InfoOutputTxtController ctrl = new InfoOutputTxtController();
            InfoOutputTxtGUIController GUICtrl = new InfoOutputTxtGUIController();

            CreateNode(inputPos, nodedata, ctrl, GUICtrl);
        }

        #endregion
    }
}
