using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Editor;
using Framework.Extends;
using UnityEditor;

namespace Modules.SpringBone.Editor
{
    public class SpringBoneEditWindow : UnityEditor.EditorWindow
    {

        private static GUIStyle m_linkerGUIStyle;
        private static GUIStyle LinkerGUIStyle
        {
            get
            {
                if (m_linkerGUIStyle == null)
                {
                    m_linkerGUIStyle = new GUIStyle();
                    m_linkerGUIStyle.fontSize = 16;
                    m_linkerGUIStyle.wordWrap = true;
                    m_linkerGUIStyle.normal.textColor = Color.yellow;
                }
                return m_linkerGUIStyle;
            }
        }

        private static GUIStyle m_titleGUIStyle;
        private static GUIStyle TitleGUIStyle
        {
            get
            {
                if (m_titleGUIStyle == null)
                {
                    m_titleGUIStyle = new GUIStyle();
                    m_titleGUIStyle.fontSize = 20;
                    m_titleGUIStyle.normal.textColor = Color.white;
                }
                return m_titleGUIStyle;
            }
        }

        private static GUIStyle m_subTitleGUIStyle;
        private static GUIStyle SubTitleGUIStyle
        {
            get
            {
                if (m_subTitleGUIStyle == null)
                {
                    m_subTitleGUIStyle = new GUIStyle();
                    m_subTitleGUIStyle.fontSize = 14;
                    m_subTitleGUIStyle.wordWrap = true;
                    m_subTitleGUIStyle.normal.textColor = Color.white;
                }
                return m_subTitleGUIStyle;
            }
        }

        private static GUIStyle m_SbSelectorItemGUIStyle;
        private static GUIStyle SbSelectorItemGUIStyle
        {
            get
            {
                if (m_SbSelectorItemGUIStyle == null)
                {
                    m_SbSelectorItemGUIStyle = EditorStyles.helpBox.Clone();
                    m_SbSelectorItemGUIStyle.fontSize = 12;
                    m_SbSelectorItemGUIStyle.wordWrap = true;
                    m_SbSelectorItemGUIStyle.alignment = TextAnchor.MiddleCenter;
                    m_SbSelectorItemGUIStyle.normal.textColor = Color.yellow;
                }
                return m_SbSelectorItemGUIStyle;
            }
        }

        private static GUIStyle m_SbSelectorItemGUIStyle2;
        private static GUIStyle SbSelectorItemGUIStyle2
        {
            get
            {
                if (m_SbSelectorItemGUIStyle2 == null)
                {
                    m_SbSelectorItemGUIStyle2 = EditorStyles.helpBox.Clone();
                    m_SbSelectorItemGUIStyle2.fontSize = 12;
                    m_SbSelectorItemGUIStyle2.wordWrap = true;
                    m_SbSelectorItemGUIStyle2.alignment = TextAnchor.MiddleCenter;
                    m_SbSelectorItemGUIStyle2.normal.textColor = Color.white;
                }
                return m_SbSelectorItemGUIStyle2;
            }
        }

        private static GUIStyle m_SbSelectorItemMinusGUIStyle;
        private static GUIStyle SbSelectorItemMinusGUIStyle
        {
            get
            {
                if (m_SbSelectorItemMinusGUIStyle == null)
                {
                    m_SbSelectorItemMinusGUIStyle = EditorStyles.helpBox.Clone();
                    m_SbSelectorItemMinusGUIStyle.fontSize = 12;
                    m_SbSelectorItemMinusGUIStyle.alignment = TextAnchor.MiddleCenter;
                    m_SbSelectorItemMinusGUIStyle.normal.textColor = Color.white;
                }
                return m_SbSelectorItemMinusGUIStyle;
            }
        }

        //----------

        private static SpringBoneEditWindow _instance;
        public static SpringBoneEditWindow init(Transform SpringBoneRoot)
        {
            _instance = UnityEditor.EditorWindow.GetWindow<SpringBoneEditWindow>();
            _instance.minSize = new Vector2(300, 600);
            _instance.Setup(SpringBoneRoot);
            return _instance;
        }



        //-----------------------------------------------------

        //此3字段加了static也不能解决非运行时转运行时被清空的问题.
        private SpringBone _spingBoneRoot;
        private Transform _root;
        private SpringBoneEditon _edition;

        public void Setup(Transform SpringBoneRoot)
        {

            _root = SpringBoneRoot;
            _spingBoneRoot = _root.FindComponentInChildren<SpringBone>(true);
            if (_spingBoneRoot)
            {
                _edition = _spingBoneRoot.GetComponent<SpringBoneEditon>();
                if (!_edition)
                {
                    _edition = _spingBoneRoot.gameObject.AddComponent<SpringBoneEditon>();
                    _edition.hideFlags = HideFlags.HideInInspector; //隐藏
                }
                if (_edition) {
                    _edition.FillSpringBones();
                }
            }
        }

        private void OnGUI() {

            #region 编译中
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling Please Wait..."));
                Repaint();
                return;
            }
            RemoveNotification();
            #endregion

            EditorPlusMethods.Draw_DebugWindowSizeUI();

            if (!_spingBoneRoot || !_edition) {
                _draw_noTargetUI();
                return;
            }

            GUILayout.Space(12);

            _draw_titleUI();

            GUILayout.Space(12);

            _draw_selectorUI();

            GUILayout.Space(12);

            if (_edition.SpringBones.Count > 0)
            {

                _draw_mutiEditUI();

                GUILayout.Space(12);
            }
            Repaint();
        }

        //----------------------
        private static GUIContent m_noTargetTipLabel;
        private static GUIContent NoTargetTipLabel {
            get {
                if (m_noTargetTipLabel == null) {
                    m_noTargetTipLabel = new GUIContent("无初始化数据,请关闭此窗口重新打开.");
                }
                return m_noTargetTipLabel;
            }
        }

        private void _draw_noTargetUI() {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(NoTargetTipLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }



        private void _draw_titleUI() {
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("SpringBone 多节点编辑窗口", TitleGUIStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private Vector2 _selectorUIScrollPos;
        private void _draw_selectorUI() {

            GUILayout.Label("节点选择器:", SubTitleGUIStyle);

            _selectorUIScrollPos = GUILayout.BeginScrollView(_selectorUIScrollPos, "box", GUILayout.Height(200));
            GUILayout.Space(5);
            bool hasSpringBones = false;
            for (int i = 0; i < _edition.SpringBones.Count; i++)
            {
                hasSpringBones = true;
                if (i > 0) {
                    GUILayout.Space(2);
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(_edition.SpringBones[i].gameObject.name, SbSelectorItemGUIStyle, GUILayout.Height(20)))
                {
                    Selection.activeGameObject = _edition.SpringBones[i].gameObject;
                }
                if (_edition.SpringBones[i] != _spingBoneRoot)
                {
                    if (GUILayout.Button("-", SbSelectorItemMinusGUIStyle, GUILayout.Height(20), GUILayout.Width(20)))
                    {
                        SpringBone spingBone = _edition.SpringBones[i];
                        _edition.SpringBones.Remove(spingBone);

                        if (Application.isPlaying)
                        {
                            GameObject.Destroy(spingBone);
                        }
                        else
                        {
                            GameObject.DestroyImmediate(spingBone);
                        }
                        i--;
                    }
                }
                GUILayout.EndHorizontal();
            }

            if(Selection.gameObjects.Length > 0 && hasSpringBones) GUILayout.Space(2);

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {

                if (i > 0)
                {
                    GUILayout.Space(2);
                }

                SpringBone spingBone = Selection.gameObjects[i].GetComponent<SpringBone>();
                if (Selection.gameObjects[i].transform.root == _root && !spingBone)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(Selection.gameObjects[i].name, SbSelectorItemGUIStyle2, GUILayout.Height(20)))
                    {
                        //do nothing
                    }
                    if (GUILayout.Button("+", SbSelectorItemMinusGUIStyle, GUILayout.Height(20), GUILayout.Width(20)))
                    {
                        spingBone = Selection.gameObjects[i].AddComponent<SpringBone>();
                        _edition.SpringBones.Add(spingBone);
                        _edition.FillSpringBones();
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(5);
            GUILayout.EndScrollView();

        }

        private void _draw_mutiEditUI() {

            GUILayout.Label("全局节点参数编辑:", SubTitleGUIStyle);

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.Label("Spring End:");

            GUILayout.Space(5);

            Vector3 seCahce = _edition.springEnd;
            Vector3 nSeCahce = EditorGUILayout.Vector3Field("",seCahce);
            if (!nSeCahce.Equals(seCahce)) {
                _edition.springEnd = nSeCahce;
                foreach (var item in _edition.SpringBones)
                {
                    item.springEnd = nSeCahce;
                }
            }

            GUILayout.Space(12);
            
            GUILayout.Label("轴锁定:");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            bool LockRotaX = _edition.LockRotaX;
            bool nLockRotaX = EditorGUILayout.ToggleLeft("X", LockRotaX, GUILayout.Width(30));
            bool LockRotaY = _edition.LockRotaY;
            bool nLockRotaY = EditorGUILayout.ToggleLeft("Y", LockRotaY, GUILayout.Width(30));
            bool LockRotaZ = _edition.LockRotaZ;
            bool nLockRotaZ = EditorGUILayout.ToggleLeft("Z", LockRotaZ, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            if (!nLockRotaX.Equals(LockRotaX) || !nLockRotaY.Equals(LockRotaY) || !nLockRotaZ.Equals(LockRotaZ))
            {
                _edition.LockRotaX = nLockRotaX;
                _edition.LockRotaY = nLockRotaY;
                _edition.LockRotaZ = nLockRotaZ;
                foreach (var item in _edition.SpringBones)
                {
                    item.LockRotaX = nLockRotaX;
                    item.LockRotaY = nLockRotaY;
                    item.LockRotaZ = nLockRotaZ;
                }
            }

            GUILayout.Space(12);

            bool useSpecifiedRotation = _edition.useSpecifiedRotation;
            bool nUseSpecifiedRotation = EditorGUILayout.ToggleLeft("Use Specified Rotation", useSpecifiedRotation);
            if (!nUseSpecifiedRotation.Equals(useSpecifiedRotation))
            {
                _edition.useSpecifiedRotation = nUseSpecifiedRotation;
                foreach (var item in _edition.SpringBones)
                {
                    item.useSpecifiedRotation = nUseSpecifiedRotation;
                }
            }

            if (nUseSpecifiedRotation)
            {
                GUILayout.Space(5);

                EditorGUI.indentLevel++;

                GUILayout.Label("Custom Rotation:");

                GUILayout.Space(5);

                Vector3 customRotation = _edition.customRotation;
                Vector3 nCustomRotation = EditorGUILayout.Vector3Field("", customRotation);

                EditorGUI.indentLevel--;
                if (!nCustomRotation.Equals(customRotation))
                {
                    _edition.customRotation = nCustomRotation;
                    foreach (var item in _edition.SpringBones)
                    {
                        item.customRotation = nCustomRotation;
                    }
                }
            }

            GUILayout.Space(12);


            bool useMutiEditForces = _edition.useMutiEditForces;
            bool nUseMutiEditForces = EditorGUILayout.ToggleLeft("Forces", useMutiEditForces);
            if (!nUseMutiEditForces.Equals(useMutiEditForces)) {
                _edition.useMutiEditForces = nUseMutiEditForces;
            }

            if (nUseMutiEditForces) {

                EditorGUI.indentLevel++;
                GUILayout.Space(5);

                float inval = 1f / (_edition.SpringBones.Count - 1);

                _edition.stiffness = EditorGUILayout.FloatField("刚度(stiffness)", _edition.stiffness);
                _edition.stiffnessCureve = EditorGUILayout.CurveField(_edition.stiffnessCureve);

                GUILayout.Space(5);

                _edition.bounciness = EditorGUILayout.FloatField("弹性(bounciness)", _edition.bounciness);
                _edition.bouncinessCureve = EditorGUILayout.CurveField(_edition.bouncinessCureve);

                GUILayout.Space(5);

                _edition.dampness = EditorGUILayout.FloatField("湿度(dampness)", _edition.dampness);
                _edition.dampnessCureve = EditorGUILayout.CurveField(_edition.dampnessCureve);

                for (int i = 0; i < _edition.SpringBones.Count; i++)
                {
                    _edition.SpringBones[i].stiffness = _edition.stiffness * _edition.stiffnessCureve.Evaluate(i * inval);
                    _edition.SpringBones[i].bounciness = _edition.bounciness * _edition.bouncinessCureve.Evaluate(i * inval);
                    _edition.SpringBones[i].dampness = _edition.dampness * _edition.dampnessCureve.Evaluate(i * inval);
                }


                EditorGUI.indentLevel--;
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();

        }

    }
}

   
