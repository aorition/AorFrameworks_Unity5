using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AorAnimatorDebugerWindow : UnityEditor.EditorWindow
{
    [MenuItem("FrameworkTools/角色动画工具/Animator预览工具")]
    public static void init()
    {
        EditorWindow w = EditorWindow.GetWindow<AorAnimatorDebugerWindow>();
    }

    private bool _AnimatorLock;

    private Animator _animatorCache;
    private GameObject _select;

    private readonly List<string> _stateName = new List<string>();
    private readonly List<int> _layerByState = new List<int>();

    private int _animatorStateIndex = 0;


    private float _startTime;
    private float _corssTime;
    private float _speed = 1f;

    private void OnGUI()
    {

        if (EditorApplication.isCompiling)
        {
            GUILayout.Label("Compiling code ... Please waiting.");
//            _effectAsset = null;
            Repaint();
            return;
        }

        if (_animatorCache)
        {
            _AnimatorLock = EditorGUILayout.Toggle(new GUIContent("锁定选定Animator"), _AnimatorLock);
        }

        //获取Animator
        if (!_animatorCache || !_AnimatorLock || _stateName.Count == 0)
        {
            _select = Selection.activeGameObject;
            if (_select)
            {
                if (!_AnimatorLock)
                {

                    if (_stateName.Count == 0)
                    {
                        _animatorCache = null; //_stateName没有值时强制执行init
                    }

                    Animator animator = _select.GetComponentInChildren<Animator>();
                    if (animator && _animatorCache != animator)
                    {
                        _animatorCache = animator;

                        //init
                        //获取state
                        _stateName.Clear();
                        _layerByState.Clear();
                        AnimatorController animatorController = _animatorCache.runtimeAnimatorController as AnimatorController;
                        if (!animatorController)
                        {
                            AnimatorOverrideController animatorOverrideController =
                                _animatorCache.runtimeAnimatorController as AnimatorOverrideController;
                            animatorController = animatorOverrideController.runtimeAnimatorController as AnimatorController;
                        }

                        if (animatorController)
                        {
                            _stateName.Add("---");
                            _layerByState.Add(0);

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
                                            _layerByState.Add(m);
                                            _stateName.Add(state.name);
                                        }

                                    }

                                }
                            }
                        }

                        //onInit

                    }
                }
            }
            else
            {
                if (!_AnimatorLock)
                {
                    _animatorCache = null;
                    _select = null;
                }
            }
        }

        if (_animatorCache)
        {
            GUILayout.Label("Target : " + _animatorCache.gameObject.name);
        }
        else
        {
            GUILayout.Label("提示:在Hierarchy中选择带有Animator的GamoObject对象.");
//            _effectAsset = null;
            EditorUtility.UnloadUnusedAssetsImmediate();
            Repaint();
            return;
        }

        //绘制State GUI
        _animatorStateIndex = EditorGUILayout.Popup("动作列表", _animatorStateIndex, _stateName.ToArray());

        GUILayout.BeginHorizontal();
        _speed = EditorGUILayout.FloatField("播放速率", _speed);
        GUILayout.Space(32);
        _corssTime = EditorGUILayout.FloatField("过度时间", _corssTime);
        GUILayout.EndHorizontal();
        _startTime = EditorGUILayout.Slider("帧停位置", _startTime, 0f, 1f);

        GUILayout.Space(5);

        //events
        GUILayout.Space(5);

        if (Application.isPlaying)
        {
            if (_animatorStateIndex > 0)
            {
                draw_activeMenu_runtime(_stateName, _layerByState);
            }
            else
            {
                draw_disableMenu();
            }
        }
        else
        {
            draw_tip_UI();
            draw_disableMenu();
        }

        Repaint();
    }

    private void draw_tip_UI()
    {
        GUILayout.Space(5);
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("本工具为运行时工具,在非运行时不能工作.请单击\"播放\"按钮运行游戏.");
        GUILayout.EndHorizontal();
    }

    //-------------------------------------------------------------

    #region runtime

    private void draw_activeMenu_runtime(List<string> stateName, List<int> layerByState)
    {
        try
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("播放"), GUILayout.Height(42)))
            {
                if (_corssTime > 0)
                {
                    rossFadeState_runtime(stateName[_animatorStateIndex], layerByState[_animatorStateIndex], _startTime);
                }
                else
                {
                    playState_runtime(stateName[_animatorStateIndex], layerByState[_animatorStateIndex], _startTime);
                }

            }
            if (GUILayout.Button(new GUIContent("帧停"), GUILayout.Height(42)))
            {
                pauseState_runtime(stateName[_animatorStateIndex], layerByState[_animatorStateIndex], _startTime);
            }
            GUILayout.EndHorizontal();
        }
        catch (Exception ex)
        {
            //过滤无用提示 ArgumentException: Getting control 5's position in a group with only 5 controls when doing Repaint Aborting
        }

    }

    private void pauseState_runtime(string stateName, int layerId, float t)
    {
        _animatorCache.speed = 0f;
        _animatorCache.Play(stateName, layerId, t);
    }

    private void playState_runtime(string stateName, int layerId, float t)
    {
        _animatorCache.speed = _speed;
        _animatorCache.Play(stateName, layerId, t);
    }

    private void rossFadeState_runtime(string stateName, int layerId, float t)
    {
        _animatorCache.speed = _speed;
        _animatorCache.CrossFade(stateName, t, layerId);
    }
    
    #endregion

    private void draw_disableMenu()
    {
        try
        {
            GUI.color = Color.gray;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("播放"),GUILayout.Height(42)))
            {
                //
            }
            if (GUILayout.Button(new GUIContent("帧停"), GUILayout.Height(42)))
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
}
