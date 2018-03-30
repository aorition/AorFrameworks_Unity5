using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
//using YoukiaUnity.Graphics;
//using YoukiaUnity.View;

public class SimpleAnimatorDebugerWindow : UnityEditor.EditorWindow
{
    [MenuItem("FrameworkTools/辅助工具/AnimatorRuntimeDebuger")]
    public static void init()
    {
        EditorWindow w = EditorWindow.GetWindow<SimpleAnimatorDebugerWindow>();
    }

    private void Awake()
    {
//        _initeffectTime = EditorApplication.timeSinceStartup;
    }

    private bool _AnimatorLock;

    private Animator _animator;
    private GameObject _selectGO;

    private int _animatorStateIndex = 0;

    private float _startTime;
    private float _corssTime;

//    private GameObject _effectAsset;
//    private GameObject _effectGo;

//    private double _initeffectTime;
//    private void Update()
//    {
//        if (_effectGo)
//        {
//            EffectDescript evb = _effectGo.GetComponent<EffectDescript>();
//            if (evb)
//            {
//                if (evb.SurvivalTime > 0 && EditorApplication.timeSinceStartup - _initeffectTime >= evb.SurvivalTime)
//                {
//                    GameObject del = _effectGo;
//                    GameObject.Destroy(del);
//                    _effectGo = null;
//                }
//            }
//        }
//    }
    
    private void OnGUI()
    {

        if (EditorApplication.isCompiling)
        {
            GUILayout.Label("Compiling code ... Please waiting.");
//            _effectAsset = null;
            Repaint();
            return;
        }

        if (!Application.isPlaying)
        {
            GUILayout.Label("SimpleAnimatorDebuger只能工作在编辑器运行时.");
            Repaint();
            return;
        }

        if (_animator)
        {
            _AnimatorLock = EditorGUILayout.Toggle(new GUIContent("锁定选定Animator"), _AnimatorLock);
        }

        //获取Animator
        if (!_animator || !_AnimatorLock)
        {
            _selectGO = Selection.activeGameObject;
            if (_selectGO)
            {
                if (!_AnimatorLock)
                {
                    _animator = _selectGO.GetComponentInChildren<Animator>();
                }
            }
            else
            {
                if (!_AnimatorLock)
                {
                    _animator = null;
                    _selectGO = null;
                }
            }
        }

        if (_animator)
        {
            GUILayout.Label("Target : " + _animator.gameObject.name);
        }
        else
        {
            GUILayout.Label("提示:在Hierarchy中选择带有Animator的GamoObject对象.");
//            _effectAsset = null;
            EditorUtility.UnloadUnusedAssetsImmediate();
            Repaint();
            return;
        }

        //获取state
        List<string> stateName = new List<string>();
        List<int> layerByState = new List<int>();
        AnimatorController animatorController = _animator.runtimeAnimatorController as AnimatorController;
        if (animatorController)
        {

            stateName.Add("---");
            layerByState.Add(0);

            int m, mlen = animatorController.layers.Length;
            for (m = 0; m < mlen; m++)
            {
                AnimatorStateMachine stateMachine = animatorController.layers[m].stateMachine;
                if (stateMachine.states != null)
                {
                    int s, slen = stateMachine.states.Length;
                    for (s = 0; s < slen; s++)
                    {
                        AnimatorState state = stateMachine.states[s].state;
                        if (state)
                        {
                            layerByState.Add(m);
                            stateName.Add(state.name);
                        }

                    }

                }
            }

        }

        //绘制State GUI
        _animatorStateIndex = EditorGUILayout.Popup("动作列表", _animatorStateIndex, stateName.ToArray());

        _startTime = EditorGUILayout.Slider(new GUIContent("播放进度"), _startTime, 0f, 1f);
        _corssTime = EditorGUILayout.FloatField(new GUIContent("过度时间"), _corssTime);

//        try
//        {
//            GUILayout.Space(10);
//            GUILayout.BeginHorizontal();
//            _effectAsset =
//                (GameObject)
//                    EditorGUILayout.ObjectField(new GUIContent("附加Effect"), _effectAsset, typeof (GameObject), false);
//            if (GUILayout.Button("-", GUILayout.Width(12)))
//            {
//                if (_effectAsset != null)
//                {
//                    _effectAsset = null;
//                    EditorUtility.UnloadUnusedAssetsImmediate();
//                }
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.Space(10);
//        }
//        catch (Exception ex)
//        {
//            //
//        }
        //
        if (_animatorStateIndex > 0)
        {
            _drawAcitvieMenu(stateName, layerByState);
        }
        else
        {
            _drawDisabledMenu();
        }

        // 

        //        GUILayout.FlexibleSpace();
        //        GUILayout.Label(Screen.width + " , " + Screen.height);
        //
        Repaint();
    }

    private void _drawAcitvieMenu( List<string> stateName, List<int> layerByState)
    {
        try
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("播放")))
            {
                if (_corssTime > 0)
                {
                    rossFadeState(stateName[_animatorStateIndex], layerByState[_animatorStateIndex], _startTime);
                }
                else
                {
                    playState(stateName[_animatorStateIndex], layerByState[_animatorStateIndex], _startTime);
                }

//                if (_effectGo)
//                {
//                    GameObject del = _effectGo;
//                    GameObject.Destroy(del);
//                    _effectGo = null;
//                }

//                if (_effectAsset)
//                {
//                    _effectGo = GameObject.Instantiate(_effectAsset);
//                    _effectGo.name = _effectAsset.name;
//
//                    EffectDescript evb = _effectGo.GetComponent<EffectDescript>();
//                    if (evb == null)
//                    {
//                        evb = _effectGo.AddComponent<EffectDescript>();
//                    }
//                    else
//                    {
//                        evb.EffectRootGameObject = _effectGo;
//                    }
//
//                    Transform pivotT = GetPivot(_selectGO,evb.EffectPivot);
//                    switch (evb.effectPivotType)
//                    {
//                        case EffectPivotType.Follow:
//                            _effectGo.transform.SetParent(pivotT, false);
//                            _effectGo.transform.position = pivotT.position;
//                            _effectGo.transform.eulerAngles = _selectGO.transform.eulerAngles;
//                            break;
//                        case EffectPivotType.World:
//                            _effectGo.transform.position = pivotT.position;
//                            _effectGo.transform.eulerAngles = _selectGO.transform.eulerAngles;
//                            break;
//                        case EffectPivotType.WorldPos:
//                            _effectGo.transform.position = pivotT.position;
//                            break;
//                        case EffectPivotType.Screen:
//                            if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().EffectCamera)
//                            {
//                                _effectGo.transform.SetParent(GraphicsManager.GetInstance().EffectCamera.transform, false);
//                                _effectGo.transform.localEulerAngles = Vector3.zero;
//                                _effectGo.transform.localPosition = new Vector3(0, 0, 1);
//                            }
//                            break;
//                        default:
//                            _effectGo.transform.position = _selectGO.transform.position;
//                            _effectGo.transform.rotation = _selectGO.transform.rotation;
//                            break;
//                    }
//                    _effectGo.SetActive(true);
//
//                    _initeffectTime = EditorApplication.timeSinceStartup;
//
//                }

            }
            if (GUILayout.Button(new GUIContent("帧停")))
            {
                pauseState(stateName[_animatorStateIndex], layerByState[_animatorStateIndex], _startTime);
            }
            GUILayout.EndHorizontal();
        }
        catch (Exception ex)
        {
            //过滤无用提示 ArgumentException: Getting control 5's position in a group with only 5 controls when doing Repaint Aborting
        }

    }

//    private Transform GetPivot(GameObject go, string pName)
//    {
//        PivotPointData PivotData = go.GetComponentInChildren<PivotPointData>();
//        if (PivotData == null)
//            return go.transform;
//        else
//        {
//            GameObject go1 = PivotData.GetPivot<GameObject>(pName);
//
//            if (go1 == null)
//            {
//                return go.transform;
//            }
//            else
//            {
//                return go1.transform;
//            }
//        }
//    }

    private void _drawDisabledMenu()
    {
        try
        {
            GUI.color = Color.gray;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("播放")))
            {
                //
            }
            if (GUILayout.Button(new GUIContent("帧停")))
            {
                //
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }
        catch (Exception ex)
        {
            //过滤无用提示 ArgumentException: Getting control 5's position in a group with only 5 controls when doing Repaint Aborting
        }
    }

    //---------------------------------------------

    private void pauseState(string stateName, int layerId, float t)
    {
        _animator.speed = 0f;
        _animator.Play(stateName, layerId, t);
    }

    private void playState(string stateName,int layerId, float t)
    {
        _animator.speed = 1f;
        _animator.Play(stateName, layerId, t);
        
    }

    private void rossFadeState(string stateName, int layerId, float t)
    {
        _animator.speed = 1f;
        _animator.CrossFade(stateName, t, layerId);
    }

}
