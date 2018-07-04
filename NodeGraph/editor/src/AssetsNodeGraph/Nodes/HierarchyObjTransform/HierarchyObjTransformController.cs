using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class HierarchyObjTransformController : NodeController
    {

        private HOTransformTypeEnum TramsformType;
        private Vector3 TransformValue;
        private Vector3 TransformOffset;
        private Vector3 TransformRate;

        private float CircleDistance = 0f;
        private float CircleDistanceRate = 1f;
        private float CircleAngleStart = 0f;
        private float CircleAngle = 0f;
        private float CircleAngleRate = 1f;
        private HOCircleType HOCircleType;

        private HORotationTypeEnum RotationType;
        private Vector3 RotationValue;
        private Vector3 RotationOffset;
        private Vector3 RotationRate;

        private HOScaleTypeEnum ScaleType;
        private Vector3 ScaleValue;
        private Vector3 ScaleOffset;
        private Vector3 ScaleRate;

        public override void update(bool updateParentLoop = true)
        {

            bool useEditorSelection = (bool)nodeGUI.data.ref_GetField_Inst_Public("UseEditorSelection");

            List<string> resultInfoList = new List<string>();
            List<int> instanceList = new List<int>();

            List<GameObject> gameObjectList = new List<GameObject>();

            TramsformType = (HOTransformTypeEnum)Enum.Parse(typeof(HOTransformTypeEnum), (string)nodeGUI.data.ref_GetField_Inst_Public("TransformType"));
            TransformValue = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("Transform");
            TransformOffset = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("TransformOffset");
            TransformRate = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("TransformRate");

            CircleDistance = (float)nodeGUI.data.ref_GetField_Inst_Public("CircleDistance");
            CircleDistanceRate = (float)nodeGUI.data.ref_GetField_Inst_Public("CircleDistanceRate");
            CircleAngleStart = (float)nodeGUI.data.ref_GetField_Inst_Public("CircleAngleStart");
            CircleAngle = (float)nodeGUI.data.ref_GetField_Inst_Public("CircleAngle");
            CircleAngleRate = (float)nodeGUI.data.ref_GetField_Inst_Public("CircleAngleRate");
            HOCircleType = (HOCircleType) Enum.Parse(typeof(HOCircleType),(string) nodeGUI.data.ref_GetField_Inst_Public("HOCircleType"));

            RotationType = (HORotationTypeEnum)Enum.Parse(typeof(HORotationTypeEnum), (string)nodeGUI.data.ref_GetField_Inst_Public("RotationType"));
            RotationValue = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("Rotation");
            RotationOffset = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("RotationOffset");
            RotationRate = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("RotationRate");

            ScaleType = (HOScaleTypeEnum)Enum.Parse(typeof(HOScaleTypeEnum), (string)nodeGUI.data.ref_GetField_Inst_Public("ScaleType"));
            ScaleValue = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("Scale");
            ScaleOffset = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("ScaleOffset");
            ScaleRate = (Vector3)nodeGUI.data.ref_GetField_Inst_Public("ScaleRate");


            int i = 0;
            int len;

            if (useEditorSelection)
            {

                gameObjectList.AddRange(Selection.gameObjects);

            }
            else
            {

                //获取上级节点数据 (PrefabInput)
                ConnectionPointGUI cpg2 = NodeGraphBase.Instance.GetConnectionPointGui(m_nodeGUI.id, 100, ConnectionPointInoutType.MutiInput);
                List<ConnectionGUI> clist2 = NodeGraphBase.Instance.GetContainsConnectionGUI(cpg2);
                if (clist2 != null)
                {

                    List<int> parentInsIdData = new List<int>();

                    len = clist2.Count;
                    for (i = 0; i < len; i++)
                    {
                        int[] pd = (int[]) clist2[i].GetConnectionValue(updateParentLoop);
                        if (pd != null)
                        {
                            //去重复
                            for (int a = 0; a < pd.Length; a++)
                            {
                                if (!parentInsIdData.Contains(pd[a]))
                                {
                                    parentInsIdData.Add(pd[a]);
                                }
                            }
                        }
                    }

                    //查找Prefab

                    len = parentInsIdData.Count;
                    for (i = 0; i < len; i++)
                    {
                        GameObject go = (GameObject) EditorUtility.InstanceIDToObject(parentInsIdData[i]);
                        if (go)
                        {
                            gameObjectList.Add(go);
                        }
                    }
                    
                }

            }

            if (gameObjectList.Count > 0)
            {

                gameObjectList.Sort((a, b) =>
                {
                    if (a.transform.GetSiblingIndex() >= b.transform.GetSiblingIndex())
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                });

                len = gameObjectList.Count;
                for (i = 0; i < len; i++)
                {
                    EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len,
                        Mathf.Round((float)i / len * 10000) * 0.01f);
                    GameObject go = gameObjectList[i];

                    _setTransform(go, ref instanceList, ref resultInfoList);
                }

                EditorUtility.ClearProgressBar();

            }

            //输出 。。。
            if (resultInfoList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ResultInfo", resultInfoList.ToArray());
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ResultInfo", null);
            }

            if (instanceList.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("InstancesPath", instanceList.ToArray());
            }

            //
            NodeGraphBase.TimeInterval_Request_SAVESHOTCUTGRAPH = true; //申请延迟保存快照
            base.update(false);
        }

        /// <summary>
        ///
        /// renameKey 解析规则:
        /// 
        /// [n:数值] 数值表示该序列的固定位数
        ///
        /// </summary>
        private void _setTransform (GameObject g, ref List<int> instanceList, ref List<string> resultInfoList)
        {

            switch (ScaleType)
            {
                case HOScaleTypeEnum.Overlying:
                    ScaleValue += ScaleOffset;
                    g.transform.localScale = ScaleValue;
                    break;
                case HOScaleTypeEnum.OverlyingRate:
                    ScaleOffset = new Vector3(ScaleOffset.x * ScaleRate.x, ScaleOffset.y * ScaleRate.y, ScaleOffset.z * ScaleRate.z);
                    ScaleValue += ScaleOffset;
                    g.transform.localScale = ScaleValue;
                    break;
                default:
                    g.transform.localScale = ScaleValue;
                    break;
            }

            switch (RotationType)
            {
                case HORotationTypeEnum.Absolutely:
                    g.transform.eulerAngles = RotationValue;
                    break;
                case HORotationTypeEnum.AbsOverlying:
                    RotationValue += RotationOffset;
                    g.transform.eulerAngles = RotationValue;
                    break;
                case HORotationTypeEnum.AbsOverlyingRate:
                    RotationOffset = new Vector3(RotationOffset.x * RotationRate.x, RotationOffset.y * RotationRate.y, RotationOffset.z * RotationRate.z);
                    RotationValue += RotationOffset;
                    g.transform.eulerAngles = RotationValue;
                    break;
                case HORotationTypeEnum.Overlying:
                    RotationValue += RotationOffset;
                    g.transform.localEulerAngles = RotationValue;
                    break;
                case HORotationTypeEnum.OverlyingRate:
                    RotationOffset = new Vector3(RotationOffset.x * RotationRate.x, RotationOffset.y * RotationRate.y, RotationOffset.z * RotationRate.z);
                    RotationValue += RotationOffset;
                    g.transform.localEulerAngles = RotationValue;
                    break;
                default:
                    g.transform.localEulerAngles = RotationValue;
                    break;
            }

            switch (TramsformType)
            {
                case HOTransformTypeEnum.Absolutely:
                    g.transform.position = TransformValue;
                    break;
                case HOTransformTypeEnum.AbsOverlying:
                    TransformValue += TransformOffset;
                    g.transform.position = TransformValue;
                    break;
                case HOTransformTypeEnum.AbsOverlyingRate:
                    TransformOffset = new Vector3(TransformOffset.x * TransformRate.x, TransformOffset.y * TransformRate.y, TransformOffset.z * TransformRate.z);
                    TransformValue += TransformOffset;
                    g.transform.position = TransformValue;
                    break;
                case HOTransformTypeEnum.Overlying:
                    TransformValue += TransformOffset;
                    g.transform.localPosition = TransformValue;
                    break;
                case HOTransformTypeEnum.OverlyingRate:
                    TransformOffset = new Vector3(TransformOffset.x * TransformRate.x, TransformOffset.y * TransformRate.y, TransformOffset.z * TransformRate.z);
                    TransformValue += TransformOffset;
                    g.transform.localPosition = TransformValue;
                    break;
                case HOTransformTypeEnum.Circle:
                    CircleAngle *= CircleAngleRate;
                    CircleAngleStart += CircleAngle;

                    CircleDistance *= CircleDistanceRate;
                    Vector3 TCValue;
                    float x = Mathf.Cos(CircleAngleStart*Mathf.Deg2Rad)*CircleDistance;
                    float y = Mathf.Sin(CircleAngleStart*Mathf.Deg2Rad)*CircleDistance;
                    switch (HOCircleType)
                    {
                        case HOCircleType.X:
                            TCValue = new Vector3(0, x, y);
                        break;
                        case HOCircleType.Y:
                            TCValue = new Vector3(x, 0, y);
                            break;
                        case HOCircleType.Z:
                            TCValue = new Vector3(x, y, 0);
                            break;
                        default:
                            TCValue = Vector3.zero;
                            break;
                    }

                    TransformOffset = new Vector3(TransformOffset.x * TransformRate.x, TransformOffset.y * TransformRate.y, TransformOffset.z * TransformRate.z);
                    TransformValue += TransformOffset;
                    g.transform.localPosition = TransformValue + TCValue;

                    break;
                default:
                    g.transform.localPosition = TransformValue;
                    break;
            }

            int ins = g.GetInstanceID();
            if (!instanceList.Contains(ins))
            {

                Vector3 s, r, t;

                s = g.transform.localScale;

                switch (RotationType)
                {
                    case HORotationTypeEnum.Absolutely:
                    case HORotationTypeEnum.AbsOverlying:
                    case HORotationTypeEnum.AbsOverlyingRate:
                        r = g.transform.eulerAngles;
                        break;
                    //case HORotationTypeEnum.Overlying:
                    //case HORotationTypeEnum.OverlyingRate:
                    default:
                        r = g.transform.localEulerAngles;
                        break;
                }

                switch (TramsformType)
                {
                    case HOTransformTypeEnum.Absolutely:
                    case HOTransformTypeEnum.AbsOverlying:
                    case HOTransformTypeEnum.AbsOverlyingRate:
                        t = g.transform.position;
                        break;
                    //case HOTransformTypeEnum.Overlying:
                    //case HOTransformTypeEnum.OverlyingRate:
                    default:
                        t = g.transform.localPosition;
                        break;
                }

                resultInfoList.Add(g.name + " Transform[ " + TramsformType.ToString() + ":" + t.ToString()
                                        + "], Rotation[" + RotationType.ToString() + ":" + r.ToString()
                                        + "], Scale[Relative:" + s.ToString() + "]" 
                                        );
                instanceList.Add(ins);
            }

            EditorUtility.SetDirty(g);
        }

    }
}
