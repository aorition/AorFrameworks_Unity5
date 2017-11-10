using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using YoukiaUnity.CinemaSystem;
using Object = UnityEngine.Object;


public class AnimLinkageEditorSimulate
{
    private Dictionary<int, simAnimData> animDic = new Dictionary<int, simAnimData>();
    static AnimLinkageEditorSimulate _Instance;
    List<AnimLinkage> Targets = new List<AnimLinkage>();

    //节点改变的时间 
    public Dictionary<int, float> ChangeTimeDic = new Dictionary<int, float>();
    public static AnimLinkageEditorSimulate Instance
    {

        get
        {
            if (_Instance == null)
            {
                _Instance = new AnimLinkageEditorSimulate();
                EditorApplication.update += _Instance.Update;
            }


            return _Instance;
        }


    }

    private bool _TargetsChanged = false;
    public void AddSimulateObj(AnimLinkage obj)
    {
        
        if (!Targets.Contains(obj))
        {
            Targets.Add(obj);
            _TargetsChanged = true;
        }
    }

    public void RemoveSimulateObj(AnimLinkage obj)
    {
        
        if (Targets.Remove(obj))
        {
            _TargetsChanged = true;
        }

    }

    //获得animWindow的当前动画播放器
    Component getCurrentPlayer()
    {
        EditorWindow window = EditorWindow.focusedWindow;
        Type t = window.GetType();
        if (t.Name == "AnimationWindow")
        {
            FieldInfo info = t.GetField("m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic);
            object c = info.GetValue(window);
            Type t2 = info.FieldType;
            FieldInfo state = t2.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic);
            object d = state.GetValue(c);
            MethodInfo ms = state.FieldType.GetMethod("get_activeAnimationPlayer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (ms != null)
                return ms.Invoke(d, null) as Component;




        }
        return null;



    }

    //获得animWindow的当前动画剪辑
    AnimationClip getCurrentClip()
    {
        EditorWindow window = EditorPlusMethods.GetPlusDefindWindow(EditorPlusMethods.PlusDefindWindow.AnimationWindow);
        if (window != null)
        {
            Type t = window.GetType();
            FieldInfo info = t.GetField("m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic);
            object c = info.GetValue(window);
            Type t2 = info.FieldType;
            FieldInfo state = t2.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic);
            object d = state.GetValue(c);
            MethodInfo ms = state.FieldType.GetMethod("get_activeAnimationClip", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (ms != null)
                return ms.Invoke(d, null) as AnimationClip;
        }
        return null;
    }

    public void Update()
    {

        if (this == null) return;

        if (Application.isPlaying)
            return;

        if (_TargetsChanged)
        {
            _TargetsChanged = false;
            return;
        }

        EditorWindow window = EditorWindow.focusedWindow;
        if (window == null)
            return;

        float time = 0;

        Type t = window.GetType();
        if (t.Name == "AnimationWindow")
        {
            time = _GetAnimWindowInnerTime(t, window);
        }
        else
        {
            return;
        }

        if (Targets == null || Targets.Count == 0)
            return;

        foreach (AnimLinkage target in Targets)
        {
            if (!target)
            {

                RemoveSimulateObj(target);
                return;
            }

            Simulate(target, time);

        }

    }

    /// <summary>
    /// 获取 编辑器Animation窗口中红线代表的时间（秒）
    /// </summary>
    private float _GetAnimWindowInnerTime(Type windowType, EditorWindow window)
    {
        float time = -1;
        FieldInfo info = windowType.GetField("m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic);
        Type t2 = info.FieldType;
        FieldInfo state = t2.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic);

        //  MethodInfo[] ms = state.FieldType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#if UNITY_5_5_OR_NEWER

        PropertyInfo timeInfo = state.FieldType.GetProperty("currentTime", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
        time = (float)timeInfo.GetValue(state.GetValue(info.GetValue(window)), null);

#elif UNITY_5_4_OR_NEWER
            FieldInfo currentTime = state.FieldType.GetField("m_CurrentTime", BindingFlags.Instance | BindingFlags.NonPublic);
            time = (float)currentTime.GetValue(state.GetValue(info.GetValue(window)));
//            if (time <= 0)
//                ChangeTimeDic.Clear();
#else
            
            //supported Unity 4.x

            // ...待验证

            FieldInfo currentTime = t.GetField("time", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            time = currentTime.GetValue(window);


#endif
        return time;
    }

    private float _timeCache = -1f;
    public void Simulate(AnimLinkage target, float time)
    {

        if (_timeCache!=(time))
        {
            target.ref_SetField_Inst_NonPublic("_time", time);
            SimulateChild(target, target.transform, time);
            _timeCache = time;
        }

    }


    void SimulateChild(AnimLinkage root,Transform tran, float time)
    {
        //Animator解算
        Animator at = tran.GetComponent<Animator>();

        if (at == getCurrentPlayer())
        {
            //当前播放剪辑模拟选中的动画
            AnimationClip clip = getCurrentClip();

            if (clip != null)
                clip.SampleAnimation(tran.gameObject, time - root.GetOffsetTime(time, tran.gameObject.GetHashCode()));
        }
        else if (at != null)
        {
            CinemaCharacter character = null;
            character = at.gameObject.GetComponentInParent<CinemaCharacter>();
            AnimatorController ac = at.runtimeAnimatorController as AnimatorController;

            if (character == null && ac != null)
            {
                //CinemaCharacter 是 animator动画
                for (int i = 0; i < ac.layers.Length; i++)
                {
                    AnimatorStateMachine sm = ac.layers[i].stateMachine;
                    AnimatorState state = sm.defaultState;
                    if (state != null)
                    {
                        AnimationClip clip = state.motion as AnimationClip;
                        if (clip != null)
                        {
                            clip.SampleAnimation(tran.gameObject, time - root.GetOffsetTime(time, tran.gameObject.GetHashCode()));
                        }
                    }
                }

            }
            else
            {
                if (at.gameObject.layer == LayerMask.NameToLayer("character"))
                {
                    //&& at.gameObject.layer == LayerMask.NameToLayer("character")
                    if (!animDic.ContainsKey(at.GetHashCode()))
                    {
                        simAnimData data = new simAnimData(at);
                        data.Character = character;
                        animDic.Add(at.GetHashCode(), data);

                    }

                    animDic[at.GetHashCode()].SimulateAnimator(time - root.GetOffsetTime(time, tran.gameObject.GetHashCode()));
                }
            }

        }


        //粒子解算
        if (tran.GetComponent<ParticleSystem>() != null)
            tran.GetComponent<ParticleSystem>().Simulate(time - root.GetOffsetTime(time, tran.gameObject.GetHashCode()), false, true);


        //Animation解算
        Animation anim = tran.gameObject.GetComponent<Animation>();
        if (anim != null)
        {
            if (!animDic.ContainsKey(anim.GetHashCode()))
            {
                simAnimData data = new simAnimData(anim);

                animDic.Add(anim.GetHashCode(), data);
            }

            animDic[anim.GetHashCode()].SimulateAnimation(time - root.GetOffsetTime(time, tran.gameObject.GetHashCode()));
        }
        
        //可解算脚本处理
        MonoBehaviour[] monos = tran.gameObject.GetComponents<MonoBehaviour>();
        if (monos != null && monos.Length > 0)
        {
            for (int i = 0; i < monos.Length; i++)
            {
                if (monos[i] != null && monos[i] is ISimulateAble)
                {
                    (monos[i] as ISimulateAble).Process(time - root.GetOffsetTime(time, tran.gameObject.GetHashCode()));
                }

            }

        }


        foreach (Transform each in tran)
        {

            if (each.gameObject.activeInHierarchy)
            {
                SimulateChild(root, each, time);
            }

//            if (each.gameObject.activeSelf == false)
//            {
//                AddChangeTime(each.gameObject.GetHashCode(), time);
//            }
//
//            if (ChangeTimeDic.ContainsKey(each.gameObject.GetHashCode()) &&
//                each.gameObject.activeSelf)
//            {
//                SimulateChild(root,each, time - ChangeTimeDic[each.gameObject.GetHashCode()]);
//            }
//            else
//            {
//                SimulateChild(root,each, time);
//            }

        }


    }

    /// <summary>
    /// 剪辑动画的特殊处理类
    /// </summary>
    public class simAnimData
    {
        public AnimationClip oldAnimClip;
        public float AnimStartTime;
        public string oldAnimName;
        public Animation anim;
        public Animator animtor;
        public CinemaCharacter Character;

        public simAnimData(Animation an)
        {
            anim = an;
        }
        public simAnimData(Animator an)
        {
            animtor = an;
        }

 //       private Dictionary<string, int> animDic;
        public void SimulateAnimator(float time)
        {


            if (Character == null)
                Character = animtor.gameObject.GetComponentInParent<CinemaCharacter>();

            if (Character != null)
            {

//                animDic = CinemaCharacterEditor.animationDic;
//
//                if (!animDic.Values.Contains((int)Character.BehaviorID))
//                    return;

                string animName = Character.BehaviorIDToBehaviorName();


                if (string.IsNullOrEmpty(oldAnimName))
                {
                    oldAnimName = animName;
                }
                else
                {
                    if (oldAnimName != animName)
                    {
                        AnimStartTime = time;
                        oldAnimName = animName;
                        //   Debug.Log("change " + animName);
                    }
                }

                AnimationClip clip = null;
                AnimatorController ac = animtor.runtimeAnimatorController as AnimatorController;
                if (ac == null)
                    return;

                for (int i = 0; i < ac.layers.Length; i++)
                {
                    AnimatorStateMachine sm = ac.layers[i].stateMachine;


                    for (int j = 0; j < sm.states.Length; j++)
                    {

                        AnimatorState state = sm.states[j].state;

                        if (state != null)
                        {
                            AnimationClip tmp = state.motion as AnimationClip;
                            if (tmp == null)
                                continue;

                            if (tmp.name == animName)
                            {
                                clip = tmp;
                                break;
                            }
                        }
                    }
                }


                if (clip == null)
                {
                    Debug.Log("没有找到动画" + animName);
                    return;
                }
                if (time < AnimStartTime)
                    AnimStartTime = 0;

                clip.SampleAnimation(animtor.gameObject, Mathf.Max(0, time - AnimStartTime));
            }

        }

        public void SimulateAnimation(float time)
        {

            if (Character == null)
                Character = anim.gameObject.GetComponentInParent<CinemaCharacter>();


            if (Character != null)
            {
                // EditorGUIUtility.PingObject(character);
//                animDic = CinemaCharacterEditor.animationDic;
//                if (!animDic.Values.Contains((int) Character.BehaviorID))
//                    return;

                string animName = Character.BehaviorIDToBehaviorName();

                if (string.IsNullOrEmpty(oldAnimName))
                {
                    oldAnimName = animName;
                }
                else
                {
                    if (oldAnimName != animName)
                    {
                        AnimStartTime = time;
                        oldAnimName = animName;
                        //Debug.Log("change " + animName);
                    }
                }

                AnimationClip clip = anim.GetClip(animName);
                if (clip == null)
                {
                    //Debug.Log("没有找到动画" + animName);
                    return;
                }

                if (time < AnimStartTime)
                    AnimStartTime = 0;

                ClipSampleAnimation(clip, anim.gameObject, time - AnimStartTime);

            }
            else
            {


                AnimationClip animClip = anim.clip;

                if (oldAnimClip == null)
                {
                    oldAnimClip = animClip;
                }
                else
                {
                    if (oldAnimClip != animClip)
                    {
                        AnimStartTime = time;
                        oldAnimClip = animClip;
                    }
                }

                if (animClip != null)
                {

//                animClip.SampleAnimation(anim.gameObject, Mathf.Max(0, time - AnimStartTime));
                    ClipSampleAnimation(animClip, anim.gameObject, time - AnimStartTime);
                }

            }
        }

        private void ClipSampleAnimation(AnimationClip clip, GameObject target, float time)
        {
            if (clip.isLooping)
            {
                clip.SampleAnimation(target, Mathf.Max(0, time % clip.length));
            }
            else
            {
                clip.SampleAnimation(target, Mathf.Max(0, time));
            }
        }


    }
}

