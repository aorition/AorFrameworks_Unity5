using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Framework.Editor.Misc;
using Framework.module;

[CustomEditor(typeof(SplineAnimation)), CanEditMultipleObjects]
public class SplineAnimationInspector : UnityEditor.Editor 
{
    private SplineAnimation _splineAniamtion;
    private float _progress;
    private float _selectProcess;
    void Awake() {
        _splineAniamtion = target as SplineAnimation;
    }

    public override void OnInspectorGUI() {

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_spline"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_offsetPos"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isTimeAnimation"));
        if (serializedObject.FindProperty("_isTimeAnimation").boolValue) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_animationTime"));   
        }

        if (serializedObject.FindProperty("_animationTime").floatValue <= 0f) {
            serializedObject.FindProperty("_animationTime").floatValue = 1f;
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_mode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_easeCurve"));
        if (serializedObject.FindProperty("_isTimeAnimation").boolValue) {
            EditorGUILayout.LabelField("velocity", serializedObject.FindProperty("_velocity").floatValue.ToString());
        } else {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_velocity"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_ApplyLineDirection"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_lockUPDirection"));

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying) {

            GUILayout.BeginHorizontal(GUILayout.Height(40));

            if (!_splineAniamtion.isPlaying) {
                if (GUILayout.Button("播放", GUILayout.Height(40))) {
                    _splineAniamtion.Play();
                }
            } else {

                if (_splineAniamtion.isPause) {
                    if (GUILayout.Button("继续", GUILayout.Height(40))) {
                        _splineAniamtion.Continue();
                    }
                    if (GUILayout.Button("单帧", GUILayout.Height(40))) {
                        _splineAniamtion.runAnimationOneFrame();
                    }
                    if (GUILayout.Button("停止", GUILayout.Height(40))) {
                        _splineAniamtion.Stop();
                    }
                } else {
                    if (GUILayout.Button("暂停", GUILayout.Height(40))) {
                        _splineAniamtion.Pause();
                    }
                    if (GUILayout.Button("停止", GUILayout.Height(40))) {
                        _splineAniamtion.Stop();
                    }
                }

            }
            GUILayout.EndHorizontal();
        }

        if (_splineAniamtion.spline != null) {
        
            GUILayout.Space(10);

            GUILayout.BeginVertical("helpbox");

            GUILayout.Label("关键帧设置", "HeaderLabel");
            GUILayout.BeginHorizontal("ProgressBarBack");
            _progress = EditorGUILayout.FloatField("Progress", _progress);
            _progress = Mathf.Clamp01(_progress);
            if (GUILayout.Button("添加"))
            {
                Vector3 direction = _splineAniamtion.spline.GetPoint(_progress) + _splineAniamtion.spline.GetDirection(_progress);
                direction.y = _splineAniamtion.spline.GetPoint(_progress).y;
                SplineAnimation.SplineAnimKeyInfo keyInfo = new SplineAnimation.SplineAnimKeyInfo() { direction = direction };
                _splineAniamtion.AddKey(_progress, keyInfo);
                EditorUtility.SetDirty(_splineAniamtion);
            }
            if (GUILayout.Button("清除"))
            {
                _splineAniamtion.ClearKey();
                EditorUtility.SetDirty(_splineAniamtion);
            }
            GUILayout.EndHorizontal();

            if (NGUIEditorGUIUtility.DrawHeader("keyList"))
            {
                NGUIEditorGUIUtility.BeginContents();
                List<float> keyList = _splineAniamtion.keyList;
                List<SplineAnimation.SplineAnimKeyInfo> infoList = _splineAniamtion.InfoList;
                string animationName = null;
                Vector3 direction;
                float delayTime = 0;
                for (int i = 0; i < keyList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Progress:" + keyList[i].ToString());
                    if (GUILayout.Button("删除"))
                    {
                        _splineAniamtion.DeleteKey(keyList[i]);
                        EditorUtility.SetDirty(_splineAniamtion);
                        break;
                    }
                    GUILayout.EndHorizontal();
                    animationName = EditorGUILayout.TextField("动作：", infoList[i].animationName);
                    direction = EditorGUILayout.Vector3Field("方向:", infoList[i].direction);
                    delayTime = EditorGUILayout.FloatField("延迟:", infoList[i].delayTime);
                    infoList[i].animationName = animationName;
                    infoList[i].direction = direction;
                    infoList[i].delayTime = delayTime;
                    NGUIEditorGUIUtility.DrawOutline(GUILayoutUtility.GetRect(GUILayoutUtility.GetLastRect().width, 1), Color.grey);
                    EditorUtility.SetDirty(_splineAniamtion);
                }

                NGUIEditorGUIUtility.EndContents();
            }
            GUILayout.EndVertical();
        }
    }
    /*
    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
        _splineAniamtion = target as SplineAnimation;
        _handleTransform = _splineAniamtion.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
         _handleTransform.rotation : Quaternion.identity;
        if (_splineAniamtion.spline.showNumbers && _splineAniamtion.animationObj != null && _splineAniamtion.animationObj.activeSelf)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = _splineAniamtion.spline.color;
            style.fontSize = 22;

            Vector3 pointTemp = _splineAniamtion.animationObj.transform.position;
            float sizeTemp = HandleUtility.GetHandleSize(pointTemp);
            Handles.Label(pointTemp + Vector3.right * sizeTemp * 0.1f, "Time:" + _splineAniamtion.usedTime + "\nProgress:" + _splineAniamtion.Progress, style);
        }

        List<float> keyList = _splineAniamtion.keyList;
        List<SplineAnimation.SplineAnimKeyInfo> infoList = _splineAniamtion.InfoList;
        for (int i = 0; i < infoList.Count; i++)
        {
            ShowDirectionPoint(keyList[i], infoList[i]);
        }
    }
    
    private void ShowDirectionPoint(float _progress, SplineAnimation.SplineAnimKeyInfo info)
    {
        Vector3 directPoint = info.direction;
        float size = HandleUtility.GetHandleSize(directPoint);

        Handles.color = Color.red;

        if (Handles.Button(directPoint, _handleRotation, size * HANDLE_SIZE, size * PICK_SIZE, Handles.DotCap))
        {
            _selectProcess = _progress;
            Repaint();
        }
        if (_selectProcess == _progress)
        {
            EditorGUI.BeginChangeCheck();
            directPoint = Handles.PositionHandle(directPoint, _handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_splineAniamtion, "Move DirectionPoint");
                EditorUtility.SetDirty(_splineAniamtion);
                info.direction = directPoint;
            }
        }
        Vector3 srcPoint = _splineAniamtion.spline.GetPoint(_progress);
        Handles.DrawLine(srcPoint, directPoint);
    }
    */
}