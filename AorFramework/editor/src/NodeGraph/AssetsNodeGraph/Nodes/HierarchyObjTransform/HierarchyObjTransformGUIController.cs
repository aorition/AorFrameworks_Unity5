using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;
using Framework.NodeGraph.Utility;

namespace Framework.NodeGraph
{
    [NodeToolItem("HierarchyObjTransform",
        "Framework.NodeGraph",
        "HierarchyObjTransformData|HierarchyObjTransformController|HierarchyObjTransformGUIController",
        "Hierarchy Tools",-100)]
    public class HierarchyObjTransformGUIController : NodeGUIController
    {

        private GUIStyle _guiContentTextStyle;

        public override string GetNodeLabel()
        {
            return "HierarchyObjTransform";
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

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

            bool useEditorSelection = (bool)nodeGUI.data.ref_GetField_Inst_Public("UseEditorSelection");
            bool nUseEditorSelection = EditorGUILayout.Toggle("Use Editor Selection", useEditorSelection);
            if (nUseEditorSelection != useEditorSelection)
            {
                nodeGUI.data.ref_SetField_Inst_Public("UseEditorSelection", nUseEditorSelection);
            }

            GUILayout.Space(5);

            //Transform
            HOTransformTypeEnum transformType = (HOTransformTypeEnum)NodeGraphUtility.Draw_NG_EnumPopup<HOTransformTypeEnum>(nodeGUI.data, "TransformType",new GUIContent("TransformType"));
            NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "Transform", new GUIContent("Transform"));

            if (transformType == HOTransformTypeEnum.Overlying 
                || transformType == HOTransformTypeEnum.AbsOverlying
                || transformType == HOTransformTypeEnum.OverlyingRate
                || transformType == HOTransformTypeEnum.AbsOverlyingRate
                || transformType == HOTransformTypeEnum.Circle
                )
            {
                NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "TransformOffset", new GUIContent("TransformOffset"));
            }

            if (transformType == HOTransformTypeEnum.OverlyingRate
                || transformType == HOTransformTypeEnum.AbsOverlyingRate
                || transformType == HOTransformTypeEnum.Circle
                )
            {
                NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "TransformRate", new GUIContent("TransformRate"));
            }

            if (transformType == HOTransformTypeEnum.Circle
                )
            {

                NodeGraphUtility.Draw_NG_FloatField(nodeGUI.data, "CircleDistance", new GUIContent("CircleDistance"));
                NodeGraphUtility.Draw_NG_FloatField(nodeGUI.data, "CircleDistanceRate", new GUIContent("CircleDistanceRate"));
                NodeGraphUtility.Draw_NG_FloatField(nodeGUI.data, "CircleAngleStart", new GUIContent("CircleAngleStart"));
                NodeGraphUtility.Draw_NG_FloatField(nodeGUI.data, "CircleAngle", new GUIContent("CircleAngle"));
                NodeGraphUtility.Draw_NG_FloatField(nodeGUI.data, "CircleAngleRate", new GUIContent("CircleAngleRate"));
                NodeGraphUtility.Draw_NG_EnumPopup<HOCircleType>(nodeGUI.data, "HOCircleType", new GUIContent("HOCircleType"));
            }

            //Rotation
            HORotationTypeEnum rotationType = (HORotationTypeEnum)NodeGraphUtility.Draw_NG_EnumPopup<HORotationTypeEnum>(nodeGUI.data, "RotationType", new GUIContent("RotationType"));
            NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "Rotation", new GUIContent("Rotation"));

            if (rotationType == HORotationTypeEnum.Overlying
                || rotationType == HORotationTypeEnum.AbsOverlying
                || rotationType == HORotationTypeEnum.OverlyingRate
                || rotationType == HORotationTypeEnum.AbsOverlyingRate
                )
            {
                NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "RotationOffset", new GUIContent("RotationOffset"));
            }

            if (rotationType == HORotationTypeEnum.OverlyingRate
                || rotationType == HORotationTypeEnum.AbsOverlyingRate
                )
            {
                NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "RotationRate", new GUIContent("RotationRate"));
            }

            //Scale
            HOScaleTypeEnum scaleType = (HOScaleTypeEnum)NodeGraphUtility.Draw_NG_EnumPopup<HOScaleTypeEnum>(nodeGUI.data, "ScaleType",new GUIContent("ScaleType"));
            NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "Scale", new GUIContent("Scale"));
            if (scaleType == HOScaleTypeEnum.Overlying
                || scaleType == HOScaleTypeEnum.OverlyingRate
                )
            {
                NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "ScaleOffset", new GUIContent("ScaleOffset"));
            }

            if (scaleType == HOScaleTypeEnum.OverlyingRate
                )
            {
                NodeGraphUtility.Draw_NG_Vector3Field(nodeGUI.data, "ScaleRate", new GUIContent("ScaleRate"));
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Update"))
            {
                m_nodeGUI.controller.update();
            }

            string[] resultInfoList = (string[])nodeGUI.data.ref_GetField_Inst_Public("ResultInfo");
            if (resultInfoList != null && resultInfoList.Length > 0)
            {

                GUILayout.Label("result : (" + resultInfoList.Length + ")");

                for (int i = 0; i < resultInfoList.Length; i++)
                {
                    GUILayout.Label(resultInfoList[i]);
                }

            }

            GUILayout.EndVertical();

            //base.DrawNodeInspector(inspectorWidth);
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo (GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(int[]).Name, "InputInstanceIDs", m_nodeGUI,AssetNodeGraphLagDefind.GetLabelDefine(7),new Vector2(120, 60), ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(200, 0, 2, typeof(int[]).Name, "InstancesPath", m_nodeGUI,AssetNodeGraphLagDefind.GetLabelDefine(8),new Vector2(120, 60), ConnectionPointInoutType.Output);
                ConnectionPointGUI p2 = new ConnectionPointGUI(201, 1, 2, typeof(string[]).Name, "ResultInfo", m_nodeGUI,AssetNodeGraphLagDefind.GetLabelDefine(6),new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() { p0,p1,p2 };
            }

            return _GetConnectionPointsByMode(GetMode);
        }

    }
}
