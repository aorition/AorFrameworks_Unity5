using System.Collections.Generic;
using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Utility.Editor
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof (Transform), true)]
    public class TransformPlusEditor : UnityEditor.Editor
    {
        static public TransformPlusEditor instance;

        SerializedProperty mPos;
        SerializedProperty mRot;
        SerializedProperty mScale;

        void OnEnable()
        {
            instance = this;
            mPos = serializedObject.FindProperty("m_LocalPosition");
            mRot = serializedObject.FindProperty("m_LocalRotation");
            mScale = serializedObject.FindProperty("m_LocalScale");
        }

        void OnDestroy()
        {
            instance = null;
        }

        /// <summary>
        /// 开始绘制Transform
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 15;

            serializedObject.Update();

            //------------------------------------

            DrawPosition();
            DrawRotation();
            DrawScale();

            //-------------

            if (targets != null && targets.Length > 1)
            {
                __drawAlignToolUI();
            }
            else
            {
                //Todo 暂时没有实装多选状态下的记录器实现, 目前只能现在在单个目标上
                __drawTansformShortcutUI();
            }

            //------------------------------------

            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// 绘制坐标
        /// </summary>
        void DrawPosition()
        {
            GUILayout.BeginHorizontal();
            {
                bool reset = GUILayout.Button("P", GUILayout.Width(20f));

                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));

                if (reset) mPos.vector3Value = Vector3.zero;
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制形变
        /// </summary>
        void DrawScale()
        {
            GUILayout.BeginHorizontal();
            {
                bool reset = GUILayout.Button("S", GUILayout.Width(20f));

                EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));

                if (reset) mScale.vector3Value = Vector3.one;
            }
            GUILayout.EndHorizontal();
        }

        enum Axes : int
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4,
            All = 7,
        }

        Axes CheckDifference(Transform t, Vector3 original)
        {
            Vector3 next = t.localEulerAngles;

            Axes axes = Axes.None;

            if (Differs(next.x, original.x)) axes |= Axes.X;
            if (Differs(next.y, original.y)) axes |= Axes.Y;
            if (Differs(next.z, original.z)) axes |= Axes.Z;

            return axes;
        }

        Axes CheckDifference(SerializedProperty property)
        {
            Axes axes = Axes.None;

            if (property.hasMultipleDifferentValues)
            {
                Vector3 original = property.quaternionValue.eulerAngles;

                foreach (Object obj in serializedObject.targetObjects)
                {
                    axes |= CheckDifference(obj as Transform, original);
                    if (axes == Axes.All) break;
                }
            }
            return axes;
        }

        /// <summary>
        /// 绘制一个可编辑的浮动区域
        /// </summary>
        /// <param name="hidden">是否值用 -- 代替</param>
        static bool FloatField(string name, ref float value, bool hidden, GUILayoutOption opt)
        {
            float newValue = value;
            GUI.changed = false;

            if (!hidden)
            {
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
            }
            else
            {
                float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            }

            if (GUI.changed && Differs(newValue, value))
            {
                value = newValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 由于 Mathf.Approximately 太敏感.
        /// </summary>

        static bool Differs(float a, float b)
        {
            return Mathf.Abs(a - b) > 0.0001f;
        }

        /// <summary>
        /// 绘制旋转
        /// </summary>
        void DrawRotation()
        {
            GUILayout.BeginHorizontal();
            {
                bool reset = GUILayout.Button("R", GUILayout.Width(20f));

                Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;

                visible.x = WrapAngle(visible.x);
                visible.y = WrapAngle(visible.y);
                visible.z = WrapAngle(visible.z);

                Axes changed = CheckDifference(mRot);
                Axes altered = Axes.None;

                GUILayoutOption opt = GUILayout.MinWidth(30f);

                if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, opt)) altered |= Axes.X;
                if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, opt)) altered |= Axes.Y;
                if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, opt)) altered |= Axes.Z;

                if (reset)
                {
                    mRot.quaternionValue = Quaternion.identity;
                }
                else if (altered != Axes.None)
                {
                    //RegisterUndo("Change Rotation", serializedObject.targetObjects);
                    UnityEditor.Undo.RecordObjects(serializedObject.targetObjects, "Change Rotation");

                    foreach (Object obj in serializedObject.targetObjects)
                    {
                        Transform t = obj as Transform;
                        Vector3 v = t.localEulerAngles;

                        if ((altered & Axes.X) != 0) v.x = visible.x;
                        if ((altered & Axes.Y) != 0) v.y = visible.y;
                        if ((altered & Axes.Z) != 0) v.z = visible.z;

                        t.localEulerAngles = v;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 保证角在 180到-180度之间
        /// </summary>
        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        static public float WrapAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

//        /// <summary>
//        /// 创建制定对象的撤消点
//        /// </summary>
//        public static void RegisterUndo(string name, params UnityEngine.Object[] objects)
//        {
//            EditorPlusMethods.RegisterUndo(name, objects);
//        }
//
//        public static void RegisterUndo(string name, GameObject[] objects)
//        {
//            UnityEngine.Object[] o = new Object[objects.Length];
//            for (int i = 0; i < o.Length; i++)
//            {
//                o[i] = objects[i];
//            }
//            EditorPlusMethods.RegisterUndo(name, o);
//        }

        //--------------------- AlignTool

        private bool _alignToolEnable = false;

        private HOTransformTypeEnum TramsformType = HOTransformTypeEnum.Overlying;
        private Vector3 TransformValue = Vector3.zero;
        private Vector3 TransformOffset = Vector3.zero;
        private Vector3 TransformRate = Vector3.one;

        private float CircleDistance = 0f;
        private float CircleDistanceRate = 1f;
        private float CircleAngleStart = 0f;
        private float CircleAngle = 0f;
        private float CircleAngleRate = 1f;
        private HOCircleType HOCircleType = HOCircleType.X;

        private HORotationTypeEnum RotationType = HORotationTypeEnum.Overlying;
        private Vector3 RotationValue =Vector3.zero;
        private Vector3 RotationOffset = Vector3.zero;
        private Vector3 RotationRate = Vector3.one;

        private HOScaleTypeEnum ScaleType = HOScaleTypeEnum.Relative;
        private Vector3 ScaleValue = Vector3.one;
        private Vector3 ScaleOffset = Vector3.zero;
        private Vector3 ScaleRate = Vector3.one;

        private void __drawAlignToolUI()
        {

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            _alignToolEnable = EditorGUILayout.ToggleLeft(new GUIContent("Align Tools", "对齐工具"), _alignToolEnable);
            GUILayout.Space(5);

            if (_alignToolEnable)
            {

                //Transform

                TramsformType = (HOTransformTypeEnum)EditorGUILayout.EnumPopup("TransformType", TramsformType);
                TransformValue = EditorGUILayout.Vector3Field("Transform", TransformValue);

                if (TramsformType == HOTransformTypeEnum.Overlying
                    || TramsformType == HOTransformTypeEnum.AbsOverlying
                    || TramsformType == HOTransformTypeEnum.OverlyingRate
                    || TramsformType == HOTransformTypeEnum.AbsOverlyingRate
                    || TramsformType == HOTransformTypeEnum.Circle
                    )
                {
                    TransformOffset = EditorGUILayout.Vector3Field("TransformOffset", TransformOffset);
                }
                else
                {
                    TransformOffset = Vector3.zero;
                }

                if (TramsformType == HOTransformTypeEnum.OverlyingRate
                    || TramsformType == HOTransformTypeEnum.AbsOverlyingRate
                    || TramsformType == HOTransformTypeEnum.Circle
                    )
                {
                    TransformRate = EditorGUILayout.Vector3Field("TransformRate", TransformRate);
                }
                else
                {
                    TransformRate = Vector3.one;
                }

                if (TramsformType == HOTransformTypeEnum.Circle)
                {

                    CircleDistance = EditorGUILayout.FloatField("CircleDistance", CircleDistance);
                    CircleDistanceRate = EditorGUILayout.FloatField("CircleDistanceRate", CircleDistanceRate);
                    CircleAngleStart = EditorGUILayout.FloatField("CircleAngleStart", CircleAngleStart);
                    CircleAngle = EditorGUILayout.FloatField("CircleAngle", CircleAngle);
                    CircleAngleRate = EditorGUILayout.FloatField("CircleAngleRate", CircleAngleRate);
                    HOCircleType = (HOCircleType) EditorGUILayout.EnumPopup("HOCircleType", HOCircleType);
                }
                else
                {
                    CircleDistance = 0f;
                    CircleDistanceRate = 1f;
                    CircleAngleStart = 0f;
                    CircleAngle = 0f;
                    CircleAngleRate = 1f;
                }

                //Rotation

                RotationType = (HORotationTypeEnum)EditorGUILayout.EnumPopup("RotationType", RotationType);
                RotationValue = EditorGUILayout.Vector3Field("Rotation", RotationValue);

                if (RotationType == HORotationTypeEnum.Overlying
                    || RotationType == HORotationTypeEnum.AbsOverlying
                    || RotationType == HORotationTypeEnum.OverlyingRate
                    || RotationType == HORotationTypeEnum.AbsOverlyingRate
                    )
                {
                    RotationOffset = EditorGUILayout.Vector3Field("RotationOffset", RotationOffset);
                }
                else
                {
                    RotationOffset = Vector3.zero;
                }

                if (RotationType == HORotationTypeEnum.OverlyingRate
                    || RotationType == HORotationTypeEnum.AbsOverlyingRate
                    )
                {
                    RotationRate = EditorGUILayout.Vector3Field("RotationRate", RotationRate);
                }
                else
                {
                    RotationRate = Vector3.one;
                }

                //Scale
                ScaleType = (HOScaleTypeEnum)EditorGUILayout.EnumPopup("ScaleType", ScaleType);
                ScaleValue = EditorGUILayout.Vector3Field("Scale", ScaleValue);
                if (ScaleType == HOScaleTypeEnum.Overlying
                    || ScaleType == HOScaleTypeEnum.OverlyingRate
                    )
                {
                    ScaleOffset = EditorGUILayout.Vector3Field("ScaleOffset", ScaleOffset);
                }
                else
                {
                    ScaleOffset = Vector3.zero;
                }

                if (ScaleType == HOScaleTypeEnum.OverlyingRate)
                {
                    ScaleRate = EditorGUILayout.Vector3Field("ScaleRate", ScaleRate);
                }
                else
                {
                    ScaleRate = Vector3.one;
                }

                GUILayout.Space(10);


                if (GUILayout.Button(new GUIContent("ApplyAlign","应用对齐")))
                {

                    List<GameObject> gameObjectList = new List<GameObject>();
                    gameObjectList.AddRange(Selection.gameObjects);

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

                        //RegisterUndo("TansformAlign", gameObjectList.ToArray());
                        UnityEditor.Undo.RecordObjects(gameObjectList.ToArray(), "Tansform Align");

                        _resetNvalues();
                        int i, len = gameObjectList.Count;
                        for (i = 0; i < len; i++)
                        {
                            EditorUtility.DisplayProgressBar("Processing ...", "Processing ..." + i + " / " + len,
                                Mathf.Round((float)i / len * 10000) * 0.01f);
                            GameObject go = gameObjectList[i];

                            _setTransform(go);
                        }
                        EditorUtility.ClearProgressBar();

                    }

                }

            }

            GUILayout.EndVertical();
        }

        private Vector3 nTransformValue = Vector3.zero;
        private Vector3 nTransformOffset = Vector3.zero;
        private Vector3 nTransformRate = Vector3.one;

        private float nCircleDistance = 0f;
        private float nCircleDistanceRate = 1f;
        private float nCircleAngleStart = 0f;
        private float nCircleAngle = 0f;
        private float nCircleAngleRate = 1f;

        private Vector3 nRotationValue = Vector3.zero;
        private Vector3 nRotationOffset = Vector3.zero;
        private Vector3 nRotationRate = Vector3.one;

        private Vector3 nScaleValue = Vector3.one;
        private Vector3 nScaleOffset = Vector3.zero;
        private Vector3 nScaleRate = Vector3.one;

        private void _resetNvalues()
        {
           nTransformValue = TransformValue;
           nTransformOffset = TransformOffset;
           nTransformRate = TransformRate;

           nCircleDistance = CircleDistance;
           nCircleDistanceRate = CircleDistanceRate;
           nCircleAngleStart = CircleAngleStart;
           nCircleAngle = CircleAngle;
           nCircleAngleRate = CircleAngleRate;

           nRotationValue = RotationValue;
           nRotationOffset = RotationOffset;
           nRotationRate = RotationRate;

           nScaleValue = ScaleValue;
           nScaleOffset = ScaleOffset;
           nScaleRate = ScaleRate;

        }

        private void _setTransform(GameObject g)
        {

            switch (ScaleType)
            {
                case HOScaleTypeEnum.Overlying:
                    nScaleValue += nScaleOffset;
                    g.transform.localScale = nScaleValue;
                    break;
                case HOScaleTypeEnum.OverlyingRate:
                    nScaleOffset = new Vector3(nScaleOffset.x * nScaleRate.x, nScaleOffset.y * nScaleRate.y, nScaleOffset.z * nScaleRate.z);
                    nScaleValue += nScaleOffset;
                    g.transform.localScale = nScaleValue;
                    break;
                default:
                    g.transform.localScale = nScaleValue;
                    break;
            }

            switch (RotationType)
            {
                case HORotationTypeEnum.Absolutely:
                    g.transform.eulerAngles = nRotationValue;
                    break;
                case HORotationTypeEnum.AbsOverlying:
                    nRotationValue += nRotationOffset;
                    g.transform.eulerAngles = nRotationValue;
                    break;
                case HORotationTypeEnum.AbsOverlyingRate:
                    nRotationOffset = new Vector3(nRotationOffset.x * nRotationRate.x, nRotationOffset.y * nRotationRate.y, nRotationOffset.z * nRotationRate.z);
                    nRotationValue += nRotationOffset;
                    g.transform.eulerAngles = nRotationValue;
                    break;
                case HORotationTypeEnum.Overlying:
                    nRotationValue += nRotationOffset;
                    g.transform.localEulerAngles = nRotationValue;
                    break;
                case HORotationTypeEnum.OverlyingRate:
                    nRotationOffset = new Vector3(nRotationOffset.x * nRotationRate.x, nRotationOffset.y * nRotationRate.y, nRotationOffset.z * nRotationRate.z);
                    nRotationValue += nRotationOffset;
                    g.transform.localEulerAngles = nRotationValue;
                    break;
                default:
                    g.transform.localEulerAngles = nRotationValue;
                    break;
            }

            switch (TramsformType)
            {
                case HOTransformTypeEnum.Absolutely:
                    g.transform.position = nTransformValue;
                    break;
                case HOTransformTypeEnum.AbsOverlying:
                    nTransformValue += nTransformOffset;
                    g.transform.position = nTransformValue;
                    break;
                case HOTransformTypeEnum.AbsOverlyingRate:
                    nTransformOffset = new Vector3(nTransformOffset.x * nTransformRate.x, nTransformOffset.y * nTransformRate.y, nTransformOffset.z * nTransformRate.z);
                    nTransformValue += nTransformOffset;
                    g.transform.position = nTransformValue;
                    break;
                case HOTransformTypeEnum.Overlying:
                    nTransformValue += nTransformOffset;
                    g.transform.localPosition = nTransformValue;
                    break;
                case HOTransformTypeEnum.OverlyingRate:
                    nTransformOffset = new Vector3(nTransformOffset.x * nTransformRate.x, nTransformOffset.y * nTransformRate.y, nTransformOffset.z * nTransformRate.z);
                    nTransformValue += nTransformOffset;
                    g.transform.localPosition = nTransformValue;
                    break;
                case HOTransformTypeEnum.Circle:
                    nCircleAngle *= nCircleAngleRate;
                    nCircleAngleStart += nCircleAngle;

                    nCircleDistance *= nCircleDistanceRate;
                    Vector3 TCValue;
                    float x = Mathf.Cos(nCircleAngleStart * Mathf.Deg2Rad) * nCircleDistance;
                    float y = Mathf.Sin(nCircleAngleStart * Mathf.Deg2Rad) * nCircleDistance;
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

                    nTransformOffset = new Vector3(nTransformOffset.x * nTransformRate.x, nTransformOffset.y * nTransformRate.y, nTransformOffset.z * nTransformRate.z);
                    nTransformValue += nTransformOffset;
                    g.transform.localPosition = nTransformValue + TCValue;

                    break;
                default:
                    g.transform.localPosition = nTransformValue;
                    break;
            }
            EditorUtility.SetDirty(g);
        }

        //-------------------------------------------- TransformShortcut;
        
        private void __drawTansformShortcutUI()
        {

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");

            Transform transform = (target as Transform);
            TansformShortcut tansformShortcut = (target as Transform).GetComponent<TansformShortcut>();

            bool shortcutEnable = tansformShortcut;

            GUILayout.Space(5);
            shortcutEnable = EditorGUILayout.ToggleLeft(new GUIContent("Tansform Shortcut", "Tansform 数据快照工具"), shortcutEnable);
            GUILayout.Space(5);

            if (!shortcutEnable)
            {
                if (tansformShortcut)
                {
                    if (EditorUtility.DisplayDialog("提示", "关闭TransformShortcut会清空所有已有Shortcut记录,你确定要移除当前所有Shortcut记录?", "确定", "取消"))
                    {
                        EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                        {
                            if (Application.isPlaying)
                            {
                                GameObject.Destroy(tansformShortcut);
                            }
                            else
                            {
                                GameObject.DestroyImmediate(tansformShortcut);
                            }
                        });
                    }
                }
                GUILayout.EndVertical();
                return;
            }

            if (!tansformShortcut)
            {
                tansformShortcut = transform.gameObject.AddComponent<TansformShortcut>();
                tansformShortcut.hideFlags = HideFlags.HideInInspector;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("AddShortcut", "记录当前Tranform数据")))
            {
                tansformShortcut.ShortcutIndex = tansformShortcut.AddShortcut(TansformShortcut.TansformStruct.CreateFromTansform(transform));
            }

            GUILayout.FlexibleSpace();

            if (tansformShortcut.ShortcutIndex < 0)
            {
                GUILayout.Label("[ no Shortcut ]");
            }
            else
            {
                GUILayout.Label("[ " + (tansformShortcut.ShortcutIndex + 1) + " / " + tansformShortcut.ShortcutCount + " ]");
            }

            GUILayout.FlexibleSpace();

            if (tansformShortcut.ShortcutIndex < 0)
            {

                GUI.color = Color.gray;
                if (GUILayout.Button(new GUIContent("<", "上一个Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                {
                    //do nothing
                }
                if (GUILayout.Button(new GUIContent(">", "下一个Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                {
                    //do nothing
                }
                if (GUILayout.Button(new GUIContent("A", "应用Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                {
                    //do nothing
                }
                
                if (GUILayout.Button(new GUIContent("-", "移除当前Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                {
                    //do nothing
                }
                GUI.color = Color.white;

            }
            else
            {
                if (tansformShortcut.ShortcutIndex - 1 >= 0)
                {
                    if (GUILayout.Button(new GUIContent("<", "上一个Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                    {
                        tansformShortcut.ShortcutIndex--;
                    }
                }
                else
                {
                    GUI.color = Color.gray;
                    if (GUILayout.Button(new GUIContent("<", "上一个Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                    {
                        //do nothing
                    }
                    GUI.color = Color.white;
                }

                if (tansformShortcut.ShortcutIndex + 1 < tansformShortcut.ShortcutCount)
                {
                    if (GUILayout.Button(new GUIContent(">", "下一个Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                    {
                        tansformShortcut.ShortcutIndex++;
                    }

                }
                else
                {
                    GUI.color = Color.gray;
                    if (GUILayout.Button(new GUIContent(">", "下一个Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                    {
                        //do nothing
                    }
                    GUI.color = Color.white;
                }

                if (GUILayout.Button(new GUIContent("A", "应用Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                {
                    tansformShortcut.GetShortcut(tansformShortcut.ShortcutIndex).ApplyToTansform(transform);
                }


                if (GUILayout.Button(new GUIContent("-", "移除当前Shortcut记录"), GUILayout.Width(Mathf.Max(Screen.width * 0.1f, 28))))
                {
                    if (EditorUtility.DisplayDialog("提示", "当前行为不可逆转,你确定要移除当前Shortcut记录?", "确定", "取消"))
                    {
                        tansformShortcut.RemoveShortcut(tansformShortcut.ShortcutIndex);
                        tansformShortcut.ShortcutIndex--;
                    }
                }

                //            if (GUILayout.Button(new GUIContent("C", "清空Shortcut记录")))
                //            {
                //                if (EditorUtility.DisplayDialog("提示", "当前行为不可逆转,你确定要清空所有的Shortcut记录?", "确定", "取消"))
                //                {
                //                    tansformShortcut.ClearShortcut();
                //                }
                //            }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }


    }
}