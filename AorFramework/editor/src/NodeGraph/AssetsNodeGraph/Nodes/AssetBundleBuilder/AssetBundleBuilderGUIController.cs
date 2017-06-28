using System;
using System.Collections.Generic;
using AorFramework.NodeGraph.Tool;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    [NodeToolItem("AssetBundleBuilder",
        "AorFramework.NodeGraph",
        "AssetBundleBuilderData|AssetBundleBuilderController|AssetBundleBuilderGUIController",
        "AssetBundleTools")]
    public class AssetBundleBuilderGUIController : NodeGUIController
    {

        public override string GetNodeLabel()
        {
            return "AssetBundleBuilder";
        }

        private Vector2 _MinSizeDefind = new Vector2(200, 120);
        public override Vector2 GetNodeMinSizeDefind()
        {
            return _MinSizeDefind;
        }

        public override void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {
            //string
            string info = "0";
            string[] assetList = (string[]) connection.GetConnectionValue(false);
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

//        private 
//        public override Vector2 GetNodeMinSizeDefind()
//        {
//            return base.GetNodeMinSizeDefind();
//        }

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            //AddonsPacking
            bool addp = (bool) m_nodeGUI.data.ref_GetField_Inst_Public("AddonsPacking");
            bool naddp = EditorGUILayout.Toggle("增量打包模式", addp);
            if (naddp != addp)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("AddonsPacking", naddp);
            }

            if (naddp)
            {
                //APBundleName
                string apbn = (string)m_nodeGUI.data.ref_GetField_Inst_Public("APBundleName");
                string napbn = EditorGUILayout.TextField("AssetBundleName", apbn);
                if (napbn != apbn)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("APBundleName", napbn);
                }

                //APVariantName
                string apvn = (string)m_nodeGUI.data.ref_GetField_Inst_Public("APBundleName");
                string napvn = EditorGUILayout.TextField("VariantName", apvn);
                if (napvn != apvn)
                {
                    m_nodeGUI.data.ref_SetField_Inst_Public("APVariantName", napvn);
                }
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("APBundleName", null);
                m_nodeGUI.data.ref_SetField_Inst_Public("APVariantName", null);
            }

            //BuildAssetBundleOptions
            BuildAssetBundleOptions bbo = (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BBOEnum"));
            BuildAssetBundleOptions nbbo = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("BuildAssetBundleOptions", bbo);
            if (nbbo != bbo)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("BBOEnum", nbbo.ToString());
            }

            //BuildTarget
            BuildTarget bt = (BuildTarget)Enum.Parse(typeof(BuildTarget), (string)m_nodeGUI.data.ref_GetField_Inst_Public("BTEnum"));
            BuildTarget nbt = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", bt);
            if (nbt != bt)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("BTEnum", nbt.ToString());
            }

            //save路径
            string sp = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SubPath");
            string nsp = EditorGUILayout.TextField("APBundleSave路径(默认为空)", sp);
            if (nsp != sp)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("SubPath", nsp);
            }

            if (GUILayout.Button("Build AssetBundle"))
            {
                m_nodeGUI.controller.update();
            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "input", m_nodeGUI, AssetNodeGraphLagDefind.GetLabelDefine(7), new Vector2(100,60),  ConnectionPointInoutType.MutiInput);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0};
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
