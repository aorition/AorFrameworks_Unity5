using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;

using UnityEditor;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

public class CinemaEventsWindow : EditorWindow
{

    public static void init(Type brigdeType)
    {
        CinemaEventsWindow w = EditorWindow.GetWindow<CinemaEventsWindow>();
        w.minSize = new Vector2(480, 430);
        w.setup(brigdeType);
    }

    /*
    private static void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding,
        AnimationUtility.CurveModifiedType deleted)
    {
        Debug.Log("****************");
    }*/

    private void Awake()
    {
//        if (EditorPlusMethods.AddUsedTag("CinemaEventWindowTag") == 1)
//        {
////            AnimationUtility.onCurveWasModified += OnCurveWasModified;
//        }
    }

    private void OnDestroy()
    {
        //        if (EditorPlusMethods.SubUsedTag("CinemaEventWindowTag") <= 0)
        //        {
        ////            AnimationUtility.onCurveWasModified -= OnCurveWasModified;
        //        }
        _animationWindow = null;
        _cilp = null;
        _brigdeType = null;
    }

    private Type _brigdeType;
    private EditorWindow _animationWindow;
    private AnimationClip _cilp;
    private string[] _funcInfo;

    public List<CinemaBrigdgeInterfaceAttribute> _BrigdgeInterfaceAttributes; 
    private List<MethodInfo> _brigdeMethodInfos;


    public void setup(Type brigdeType)
    {
        _brigdeType = brigdeType;

        if (_brigdeType != null)
        {
            _brigdeMethodInfos = new List<MethodInfo>();
            _BrigdgeInterfaceAttributes = new List<CinemaBrigdgeInterfaceAttribute>();

            MethodInfo[] methodInfos = _brigdeType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            int i, len = methodInfos.Length;
            for (i = 0; i < len; i++)
            {
                MethodInfo m = methodInfos[i];

                Attribute[] a = Attribute.GetCustomAttributes(m);
                for (int u = 0; u < a.Length; u++)
                {
                    if (a[u].GetType() == typeof(CinemaBrigdgeInterfaceAttribute))
                    {

                        _brigdeMethodInfos.Add(m);
                        CinemaBrigdgeInterfaceAttribute aa = a[u] as CinemaBrigdgeInterfaceAttribute;
                        _BrigdgeInterfaceAttributes.Add(aa);

                    }
                }
            }

            if (_BrigdgeInterfaceAttributes.Count > 0)
            {
                _funcInfo = new string[_BrigdgeInterfaceAttributes.Count + 1];
                _funcInfo[0] = "  ";
                len = _BrigdgeInterfaceAttributes.Count;
                for (i = 0; i < len; i++)
                {
                    _funcInfo[i + 1] = _BrigdgeInterfaceAttributes[i].info;
                }
            }

        }
    }

    private float _AWTime = 0f;
    private Vector2 _eventsListPos = Vector2.zero;

    private int _addSelectIndex = -1;
    private float _inserTime = 0f;

    private int _modifyIndex = -1;
    private int _modifySelectIndex = -1;
    private float _modifyinserTime = 0f;
    private string _modifyParam = "";

    private bool _deleteEventDirty = false;

    private void OnGUI()
    {

        _cilp = getCurrentClip();

        if (_cilp == null || _brigdeType == null || _funcInfo == null) return;

        // GUILayout.Label("*** " + _cilp.events.Length + " ** ");

        if (_animationWindow != null)
        {
            Type t = _animationWindow.GetType();
            FieldInfo info = t.GetField("m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic);
            Type t2 = info.FieldType;
            FieldInfo state = t2.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic);

#if UNITY_5_6_OR_NEWER

            PropertyInfo timeInfo = state.FieldType.GetProperty("currentTime", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            _AWTime = (float)timeInfo.GetValue(state.GetValue(info.GetValue(_animationWindow)), null);

#elif UNITY_5_4_OR_NEWER
            FieldInfo currentTime = state.FieldType.GetField("m_CurrentTime", BindingFlags.Instance | BindingFlags.NonPublic);
            _AWTime = (float)currentTime.GetValue(state.GetValue(info.GetValue(_animationWindow)));
//            if (time <= 0)
//                ChangeTimeDic.Clear();
#else
            
            //supported Unity 4.x
            PropertyInfo currentTime = windowType.GetProperty ("time", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _AWTime = (float) currentTime.GetValue (window,null) ;
#endif
        }

        AorGUILayout.Horizontal("box", () =>
        {
            GUILayout.Label("AnimationWindow Time : ");
            GUILayout.FlexibleSpace();
            GUILayout.Label(_AWTime + " (秒)");
        });

        if (_cilp.events != null)
        {

            _eventsListPos = GUILayout.BeginScrollView(_eventsListPos, "box");

                int i, len = _cilp.events.Length;
                for (i = 0; i < len; i++)
                {
                    int idx = i;
                    if (_deleteEventDirty)
                    {
                        _deleteEventDirty = false;
                        break;
                    }
                    else { 
                        Draw_EventUI(_cilp.events[i], idx, _cilp);
                    }
                }


                Draw_editUI("ADD", ref _inserTime, ref _addSelectIndex, (param) =>
                {

                    if (_addSelectIndex > 0)
                    {
                        AnimationEvent ne = new AnimationEvent();
                        ne.time = _inserTime;
                        ne.stringParameter = param;
                        ne.functionName = _brigdeMethodInfos[_addSelectIndex - 1].Name;

                        _addNewEvent(_cilp, ne);

                        _inserTime = 0;
                        _addSelectIndex = 0;
                        Repaint();
                    }
                    else
                    {

                        _inserTime = 0;
                        _addSelectIndex = 0;
                    }

                });

            GUILayout.EndScrollView();
            
        }

//        GUILayout.FlexibleSpace();
//
//        GUILayout.Label(Screen.width + " . " + Screen.height);

        Repaint();
    }

    private void Draw_editUI(string btnLabel, ref float time, ref int index, Action<string> onbtnClick)
    {

        GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            time = EditorGUILayout.FloatField("时间", time);
            GUILayout.EndHorizontal();

            index = EditorGUILayout.Popup("动作", index, _funcInfo);

            if (index > 0)
            {
                GUILayout.BeginHorizontal();
                    string pinfo = _BrigdgeInterfaceAttributes[index - 1].parmInfo;
                    if (string.IsNullOrEmpty(pinfo))
                    {
                        pinfo = "无参数";
                    }
                    GUILayout.Label("参数说明：");
                    GUILayout.Label(pinfo);
                GUILayout.EndHorizontal();

                ParameterInfo[] pinfos = _brigdeMethodInfos[index - 1].GetParameters();
                if (pinfos.Length == 1 && pinfos[0].ParameterType == typeof(string))
                {
                    _modifyParam = EditorGUILayout.TextField("参数: ", _modifyParam);
                }
                else
                {
                    AorGUILayout.Horizontal("box", () =>
                    {
                        GUILayout.Label("该动作参数输入不规范！请检查&修正！");
                    });
                }

            }

            if (GUILayout.Button(btnLabel, GUILayout.Height(20)))
            {

                if (onbtnClick != null) onbtnClick(_modifyParam);

                Repaint();

            
            }
        GUILayout.EndVertical();
    }

    private void Draw_EventUI(AnimationEvent evt, int index, AnimationClip clip)
    {

        if (index == _modifyIndex)
        {

            Draw_editUI("Done",ref _modifyinserTime,ref _modifySelectIndex, (param) =>
            {
                if (_modifySelectIndex > 0)
                {
                    AnimationEvent ne = new AnimationEvent();
                    ne.time = _modifyinserTime;
                    ne.stringParameter = param;
                    ne.functionName = _brigdeMethodInfos[_modifySelectIndex - 1].Name;

                    _modifyEvent(index, clip, ne);

                    _modifyIndex = -1;
                    _modifyinserTime = 0;
                    _modifySelectIndex = 0;
                    Repaint();
                }
                else
                {
                    _modifyIndex = -1;
                    _modifyinserTime = 0;
                    _modifySelectIndex = 0;
                }
            });

            return;
        }


            GUILayout.BeginHorizontal("box");

                Color c = (evt.time <= _AWTime ? Color.yellow : Color.white);
                GUI.color = c;
                GUILayout.Label(evt.time.ToString(),GUILayout.Width(80));
                GUILayout.Space(50);

                GUILayout.BeginHorizontal("box");

                    int idx = _getBrigdeMethodInfoIndex(evt.functionName);
                    if (idx != -1)
                    {
                        CinemaBrigdgeInterfaceAttribute cbia = _BrigdgeInterfaceAttributes[idx];
                        GUILayout.Label(cbia.info + (string.IsNullOrEmpty(evt.stringParameter) ? "" : " : " + evt.stringParameter));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label("空事件");
                        GUI.color = Color.white;
                    }
                    
                    GUILayout.FlexibleSpace();
                
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("C", GUILayout.Width(26)))
                {
                    _copyEvent(index, clip);
                    Repaint();
                }
                if (GUILayout.Button("-", GUILayout.Width(26)))
                {
                    if (EditorUtility.DisplayDialog("警告", "确定要移除此事件?", "确定", "取消"))
                    {
                        _deleteEvent(index, clip);
                        Repaint();
                    }
                }
                if (GUILayout.Button("E", GUILayout.Width(26)))
                {
                    _modifyinserTime = evt.time;
                    _modifySelectIndex = _getBrigdeMethodInfoIndex(evt.functionName) + 1;
                    _modifyIndex = index;
                    _modifyParam = evt.stringParameter;
                    Repaint();
                }
                GUI.color = Color.white;

        GUILayout.EndHorizontal();
    }

    private int _getBrigdeMethodInfoIndex(string functionName)
    {

        if (_brigdeMethodInfos == null) return -1;

        for (int i = 0; i < _brigdeMethodInfos.Count; i++)
        {
            if (_brigdeMethodInfos[i].Name == functionName)
            {
                return i;
            }
        }

        return -1;
    }

    private AnimationEvent[] _sortEvents(List<AnimationEvent> list)
    {
        list.Sort((a, b) =>
        {
            if (a.time >= b.time)
            {
                return 1;
            }
            else
            {
                return -1;
            }

        });

        return list.ToArray();
    }

    private void _addNewEvent(AnimationClip clip, AnimationEvent newEvent)
    {
        List<AnimationEvent> nList = new List<AnimationEvent>();

        if (clip.events == null)
        {
            nList.Add(newEvent);
        }
        else
        {
            int i, len = clip.events.Length;
            for (i = 0; i < len; i++)
            {
                nList.Add(clip.events[i]);
            }
            nList.Add(newEvent);
        }
        AnimationUtility.SetAnimationEvents(clip, _sortEvents(nList));
        EditorApplication.RepaintAnimationWindow();
    }

    private void _modifyEvent(int index, AnimationClip clip, AnimationEvent newEvent)
    {
        if (clip.events == null || clip.events.Length == 0) return;
        List<AnimationEvent> nList = new List<AnimationEvent>();
        int i, len = clip.events.Length;
        for (i = 0; i < len; i++)
        {
            if (i != index)
            {
                nList.Add(clip.events[i]);
            }
            else
            {
                nList.Add(newEvent);
            }
        }
        AnimationUtility.SetAnimationEvents(clip, _sortEvents(nList));
        EditorApplication.RepaintAnimationWindow();
    }

    private void _deleteEvent(int index, AnimationClip clip)
    {

        if (clip.events == null || clip.events.Length == 0) return;

        List<AnimationEvent> nList = new List<AnimationEvent>();

        int i, len = clip.events.Length;
        for (i = 0; i < len; i++)
        {
            if (i != index)
            {
                nList.Add(clip.events[i]);
            }
        }

        _deleteEventDirty = true;

        AnimationUtility.SetAnimationEvents(clip, _sortEvents(nList));
        EditorApplication.RepaintAnimationWindow();

    }

    private void _copyEvent(int index, AnimationClip clip)
    {

        if (clip.events == null || clip.events.Length == 0) return;

        List<AnimationEvent> nList = new List<AnimationEvent>();

        int i, len = clip.events.Length;
        for (i = 0; i < len; i++)
        {
            if (i == index)
            {
                AnimationEvent src = clip.events[i];
                AnimationEvent ne = new AnimationEvent();

                float f = src.time*clip.frameRate;

                ne.time = (f + 1) / clip.frameRate;
                ne.stringParameter = src.stringParameter;
                ne.functionName = src.functionName;

                nList.Add(ne);

            }

            nList.Add(clip.events[i]);

        }

        _deleteEventDirty = true;

        AnimationUtility.SetAnimationEvents(clip, _sortEvents(nList));
        EditorApplication.RepaintAnimationWindow();


    }

    //获得animWindow的当前动画剪辑
    AnimationClip getCurrentClip()
    {

        if (_animationWindow == null)
        {
            _animationWindow = EditorPlusMethods.GetPlusDefindWindow(EditorPlusMethods.PlusDefindWindow.AnimationWindow);
        }

        if (_animationWindow != null)
        {
            Type t = _animationWindow.GetType();
            FieldInfo info = t.GetField("m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic);
            object c = info.GetValue(_animationWindow);
            Type t2 = info.FieldType;
            FieldInfo state = t2.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic);
            object d = state.GetValue(c);
            MethodInfo ms = state.FieldType.GetMethod("get_activeAnimationClip", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (ms != null)
                return ms.Invoke(d, null) as AnimationClip;
        }
        return null;
        
    }

}
