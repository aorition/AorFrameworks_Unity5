using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Animation联动
/// 
/// </summary>
[CustomEditor(typeof(AnimLinkage))]
public class AnimLinkageEditor : Editor
{

//    [MenuItem("Youkia/角色动画工具/添加动画联动解算器(编辑器功能)")]
//    public static void AnimLinkageEditorMenu()
//    {
//        if (Selection.activeGameObject != null)
//        {
//            GameObject select = Selection.activeGameObject;
//            select.AddComponent<AnimLinkage>();
//            EditorUtility.DisplayDialog("提示", "添加成功,用完输出成品时，记得删除AnimLinkage", "我知道了");
//        }
//        else
//        {
//            EditorUtility.DisplayDialog("提示", "你好并没有选中一个可以添加该工具的东东.", "OK");
//        }
//    }

    private AnimLinkage _target;
    
    private bool _isRootAnimLinkage = false;
    void Awake()
    {

        _target = this.target as AnimLinkage;

        if ((bool) _target.ref_GetField_Inst_NonPublic("_isAwaked") == false)
        {
            AnimLinkage rootAnimLinkage = _target.transform.root.GetComponentInChildren<AnimLinkage>();
            if (rootAnimLinkage == _target)
            {
                _isRootAnimLinkage = true;
                EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
                //        AnimationUtility.onCurveWasModified += OnCurveWasModified;
            }
        }

        BuildAnimLinkdageInit();
    }

    void OnEnable()
    {
        AnimLinkage rootALK = _target.transform.root.GetComponentInChildren<AnimLinkage>();
        if (!rootALK) return;
        AnimLinkageEditorSimulate.Instance.AddSimulateObj(rootALK);
    }

    void OnDestroy()
    {
        if (_isRootAnimLinkage && !_target)
        {
//            AnimationUtility.onCurveWasModified -= OnCurveWasModified;
            EditorApplication.hierarchyWindowChanged -= HierarchyWindowChanged;
        }
    }

    private void BuildAnimLinkdageInit()
    {

        if (Application.isPlaying) return;

        AnimLinkage[] AnimLinkages = _target.transform.root.GetComponentsInChildren<AnimLinkage>();

        int l, llen = AnimLinkages.Length;
        for (l = 0; l < llen; l++)
        {

            AnimLinkage animLinkage = AnimLinkages[l];

            Dictionary<int, ActiveTimeKey[]> subsActiveKeyDic = new Dictionary<int, ActiveTimeKey[]>();

            //这里需要判定是否存在子AnimLinkage

            Animator animator = animLinkage.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorController animatorController = (AnimatorController) animator.runtimeAnimatorController;

                if (animatorController != null && animatorController.layers != null &&
                    animatorController.layers.Length > 0 &&
                    animatorController.layers[0].stateMachine != null &&
                    animatorController.layers[0].stateMachine.defaultState != null)
                {

                    AnimationClip clip = (AnimationClip) animatorController.layers[0].stateMachine.defaultState.motion;
                    if (clip != null)
                    {

                        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
                        int i, len = bindings.Length;
                        for (i = 0; i < len; i++)
                        {
                            if (bindings[i].type == typeof (GameObject) && bindings[i].propertyName == "m_IsActive")
                            {

                                GameObject AnimatedObject =
                                    (GameObject) AnimationUtility.GetAnimatedObject(animLinkage.gameObject, bindings[i]);

                                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, bindings[i]);
                                if (curve != null)
                                {
                                    int c, clen = curve.keys.Length;
                                    ActiveTimeKey[] keys = new ActiveTimeKey[clen];
                                    for (c = 0; c < clen; c++)
                                    {
                                        Keyframe kf = curve.keys[c];
                                        keys[c] = new ActiveTimeKey(kf.time, (Mathf.RoundToInt(kf.value) != 0));
                                    }

                                    if (AnimatedObject != null)
                                    {
                                        subsActiveKeyDic.Add(AnimatedObject.GetHashCode(), keys);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (subsActiveKeyDic.Count > 0)
            {
                string jsonStr = subsActiveKeyDicToJson(subsActiveKeyDic);
                animLinkage.ref_SetField_Inst_NonPublic("_ActiveKeysSerialize", jsonStr);
            }


            if (!Application.isPlaying)
            {
                animLinkage.ref_InvokeMethod_Inst_NonPublic("Awake", null);
                // _target.ref_InvokeMethod_Inst_NonPublic("Start", null);
            }

        }
    }

    private void HierarchyWindowChanged()
    {
        if (_isRootAnimLinkage && !_target)
        {
            EditorApplication.hierarchyWindowChanged -= HierarchyWindowChanged;
            return;
        }

        BuildAnimLinkdageInit();
    }

    /*
    private bool _curveIsModify = false;
    private void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding,
        AnimationUtility.CurveModifiedType type)
    {

        if (_isRootAnimLinkage && !_target)
        {
            AnimationUtility.onCurveWasModified -= OnCurveWasModified;
            return;
        }

        if (_curveIsModify) return;

        if (binding.type == typeof (GameObject) && binding.propertyName == "m_IsActive")
        {

            GameObject AnimatedObject = (GameObject)AnimationUtility.GetAnimatedObject(_target.gameObject, binding);
            if (!AnimatedObject) return;
            
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve != null)
            {

                _curveIsModify = true;

                int i, len = curve.keys.Length;
                ActiveTimeKey[] keys = new ActiveTimeKey[len];
                for (i = 0; i < len; i++)
                {
                    Keyframe kf = curve.keys[i];
                    keys[i] = new ActiveTimeKey(kf.time, (Mathf.RoundToInt(kf.value) != 0));
                }

                setActiveKeyData(AnimatedObject, keys);

                _curveIsModify = false;
            }

        }

    }*/

    public override void OnInspectorGUI()
    {

        if (!_target) return;

        base.OnInspectorGUI();

        GUILayout.Label("Play : " + _target.isPlaying);
        GUILayout.Label("Time :" + _target.time);

        //编辑器运行时测试模块
        if (Application.isPlaying && _isRootAnimLinkage)
        {
            _draw_editorPlayModeTestUnit();
        }

        Repaint();
    }

//    private ActiveTimeKeyPackage _atkPackage;
    /// <summary>
    /// 插入 OffsetActive 数据记录
    /// </summary>
//    public void setActiveKeyData(GameObject gameObject, ActiveTimeKey[] keys)
//    {
//
//        Dictionary<int, ActiveTimeKey[]> subsActiveKeyDic = (Dictionary <int, ActiveTimeKey[]>)_target.ref_GetField_Inst_NonPublic("_SubsActiveKeyDic");
//
//
//        if (subsActiveKeyDic == null)
//        {
//            subsActiveKeyDic = new Dictionary<int, ActiveTimeKey[]>();
//        }
//
//        int hash = gameObject.GetHashCode();
//
//        if (subsActiveKeyDic.ContainsKey(hash))
//        {
//            subsActiveKeyDic[hash] = keys;
//        }
//        else
//        {
//            subsActiveKeyDic.Add(hash, keys);
//        }
//
//        _target.ref_SetField_Inst_NonPublic("_SubsActiveKeyDic", subsActiveKeyDic);
//
////        if (_atkPackage == null)
////        {
////            _atkPackage = new ActiveTimeKeyPackage();
////        }
// //       _atkPackage.SubsActiveKeyDic = subsActiveKeyDic;
////        string jsonStr = JsonUtility.ToJson(subsActiveKeyDic);
//        string jsonStr = subsActiveKeyDicToJson(subsActiveKeyDic);
//        _target.ref_SetField_Inst_NonPublic("_ActiveKeysSerialize", jsonStr);
//
//        _target.ref_InvokeMethod_Inst_NonPublic("Awake", null);
//        //_target.ref_InvokeMethod_Inst_NonPublic("Start", null);
//
//    }

    private StringBuilder _stringBuilder;
    private string subsActiveKeyDicToJson(Dictionary<int, ActiveTimeKey[]> subsActiveKeyDic)
    {

        if (_stringBuilder == null)
        {
            _stringBuilder = new StringBuilder();
        }
        else
        {
            _stringBuilder.Length = 0;
        }

        int sp = 0;
        _stringBuilder.Append("[");
        foreach (KeyValuePair<int, ActiveTimeKey[]> activeTimeKeyse in subsActiveKeyDic)
        {
            if (sp > 0)
            {
                _stringBuilder.Append(',');
            }
            _stringBuilder.Append("{\"key\":" + activeTimeKeyse.Key + ",\"value\":[");
            int i, len = activeTimeKeyse.Value.Length;
            for (i = 0; i < len; i++)
            {
                if (i > 0)
                {
                    _stringBuilder.Append(",");
                }
                _stringBuilder.Append("{\"time\":" + activeTimeKeyse.Value[i].time + ",\"actived\":" + (activeTimeKeyse.Value[i].actived ? 1 : 0) + "}");
            }
            _stringBuilder.Append("]}");
            sp ++;
        }
        _stringBuilder.Append("]");

        return _stringBuilder.ToString();
    }

    private void _draw_editorPlayModeTestUnit()
    {
        GUI.color = Color.yellow;
        AorGUILayout.Vertical("box", () =>
        {

            if (_target.isPlaying)
            {
                if (_target.isPaused)
                {

                    float newTime = EditorGUILayout.Slider(_target.time, 0, _target.length);
                    if (newTime!=(_target.time))
                    {
                        _target.Sample(newTime);
                    }

                    AorGUILayout.Horizontal(() =>
                    {
                        //上一帧
                        if (GUILayout.Button(new GUIContent("|< previous frame")))
                        {
                            float farmeTime = 1.0f / _target.FPS;
                            _target.Sample(newTime -= farmeTime);
                        }
                        //上一帧
                        if (GUILayout.Button(new GUIContent("next frame >|")))
                        {
                            float farmeTime = 1.0f/_target.FPS;
                            _target.Sample(newTime += farmeTime);
                        }
                    });

                    AorGUILayout.Horizontal(() =>
                    {
                        if (GUILayout.Button(new GUIContent("Play")))
                        {
                            _target.Play();
                        }
                        if (GUILayout.Button(new GUIContent("Stop")))
                        {
                            _target.Stop();
                        }
                    });
                }
                else
                {
                    AorGUILayout.Horizontal(() =>
                    {
                        if (GUILayout.Button(new GUIContent("Pause")))
                        {
                            _target.Pause();
                        }
                        if (GUILayout.Button(new GUIContent("Stop")))
                        {
                            _target.Stop();
                        }
                    });
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Play")))
                {
                    _target.Play();
                }
            }

        });
        GUI.color = Color.white;
    }

}
