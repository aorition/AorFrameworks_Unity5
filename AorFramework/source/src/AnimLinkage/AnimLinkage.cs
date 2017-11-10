using System;
using System.Collections.Generic;

using UnityEngine;
using YoukiaUnity.Misc;

[Serializable]
public struct ActiveTimeKey
{
    public ActiveTimeKey(float time, bool actived)
    {
        this.time = time;
        this.actived = actived;
    }
    public float time;
    public bool actived;
}

//[Serializable]
//public class ActiveTimeKeyPackage
//{ 
//    public Dictionary<int, ActiveTimeKey[]> SubsActiveKeyDic;
//}

/// <summary>
/// Animation联动
/// 
/// 
/// Author: Aorition
/// 目前运行时实时解算还有一些问题，为了保证安全，目前运行时仍然采用源生Animator.Play的方案.
/// 
/// </summary>
public class AnimLinkage : MonoBehaviour ,IEditorOnlyScript
{

    public bool AutoPlayOnStarted = false;

    private float _time = -1f;
    public float time
    {
        get { return _time; }
    }

    public float FPS
    {
        get
        {
            Animator at = gameObject.GetComponent<Animator>();
            if (at != null)
            {
                AnimatorStateInfo StateInfo = at.GetCurrentAnimatorStateInfo(0);
                int id = StateInfo.shortNameHash;
                AnimatorClipInfo[] info = at.GetCurrentAnimatorClipInfo(0);
                if (info != null && info.Length > 0)
                {
                    return info[0].clip.frameRate;
                }
                return 0;
            }

            Animation am = gameObject.GetComponent<Animation>();
            if (am != null)
            {
                AnimationState state = am.GetCurrentState();
                return (state != null ? state.clip.frameRate : 0);
            }

            return 0;
        }
    }

    public float length
    {
        get
        {
            Animator at = gameObject.GetComponent<Animator>();
            if (at != null)
            {
                AnimatorStateInfo StateInfo = at.GetCurrentAnimatorStateInfo(0);
                int id = StateInfo.shortNameHash;
                AnimatorClipInfo[] info = at.GetCurrentAnimatorClipInfo(0);
                if (info != null && info.Length > 0)
                {
                    return info[0].clip.length;
                }
                return 0;
            }

            Animation am = gameObject.GetComponent<Animation>();
            if (am != null)
            {
                AnimationState state = am.GetCurrentState();
                return (state != null ? state.length : 0);
            }

            return 0;
        }
    }

    [SerializeField]//[HideInInspector]
    private string _ActiveKeysSerialize;

    [NonSerialized]
    private Dictionary<int, float> _RunTimeSubsActiveKeyDic;

    [NonSerialized]
    private Dictionary<int, ActiveTimeKey[]> _SubsActiveKeyDic;
    public Dictionary<int, ActiveTimeKey[]> SubsActiveKeyDic
    {
        get { return _SubsActiveKeyDic; }
    }

    private bool _isAwaked = false;
    void Awake()
    {

        if (Application.isPlaying)
        {
            GameObject.Destroy(this);
            return;
        }

        //运行时建立SubsActiveKeyDic
        if (!string.IsNullOrEmpty(_ActiveKeysSerialize))
        {
            buildSubsActiveKeyDic();
        }

        if (_RunTimeSubsActiveKeyDic == null)
        {
            _RunTimeSubsActiveKeyDic = new Dictionary<int, float>();
        }


        _isAwaked = true;
    }

    private void buildSubsActiveKeyDic()
    {
        _SubsActiveKeyDic = new Dictionary<int, ActiveTimeKey[]>();

        string all = _ActiveKeysSerialize.Substring(1, _ActiveKeysSerialize.Length - 2);
        all = all.Replace("]},{", "}}|{");
        string[] allHash = all.Split('|');

        int i, len = allHash.Length;
        for (i = 0; i < len; i++)
        {
            int keyStrIdx = allHash[i].IndexOf("\"key\":") + 6;
            int keyStrLen = allHash[i].IndexOf(",\"value\"") - keyStrIdx;
            string keyStr = allHash[i].Substring(keyStrIdx, keyStrLen);

            int key = int.Parse(keyStr);

            string allValueStr = allHash[i].Substring(allHash[i].IndexOf("\"value\":") + 9);
            allValueStr = allValueStr.Substring(0, allValueStr.Length - 2);
            allValueStr = allValueStr.Replace("},{", "}|{");

            string[] values = allValueStr.Split('|');

            int u, ulen = values.Length;
            ActiveTimeKey[] keys = new ActiveTimeKey[ulen];
            for (u = 0; u < ulen; u++)
            {
                keys[u] = JsonUtility.FromJson<ActiveTimeKey>(values[u]);
            }

            _SubsActiveKeyDic.Add(key, keys);
        }
    }

    void OnEnable()
    {
        /*
         * 暂不开放 运行时解算
         * 
        if (_isStarted)
        {
            if (AutoPlayOnStarted && Application.isPlaying)
            {
                Play();
            }
            else
            {
                _Sample(0);
            }
        }*/
    }

    private bool _isStarted = false;
    private void Start()
    {

        _isStarted = true;

        /*
         * 暂不开放 运行时解算
         * 
        if (AutoPlayOnStarted && Application.isPlaying)
        {
            Play();
        }
        else
        {
            _Sample(0);
        }*/
    }

    private AnimationState GetCurrentState(Animation anim)
    {

        //TODO:注意,多通道播放时(比如使用CrossFade)可能同时有多个动画在不同通道上播放

        if (anim == null)
        {
            return null;
        }

        foreach (AnimationState state in anim)
        {
            if (anim.IsPlaying(state.name))
            {
                return state;
            }
        }
        return null;
    }

    void Update()
    {
        if (this == null) return;

        if (!_isStarted || !_isPlaying) return;

        if (_isPlaying && !_isPaused) 
        {
            Sample(_time + Time.deltaTime);
        }

    }

    public void CheckOffsetIime(float nowTime, int HashCode)
    {
        if (!_SubsActiveKeyDic.ContainsKey(HashCode))
        {
            if (_RunTimeSubsActiveKeyDic.ContainsKey(HashCode))
            {
                _RunTimeSubsActiveKeyDic[HashCode] = nowTime;
            }
            else
            {
                _RunTimeSubsActiveKeyDic.Add(HashCode, nowTime);
            }
        }
    }

    public float GetOffsetTime(float nowTime, int HashCode)
    {
        if (_SubsActiveKeyDic == null) return 0;

        if (_SubsActiveKeyDic.ContainsKey(HashCode))
        {

            bool f = false;
            float r = 0;
            int i, len = _SubsActiveKeyDic[HashCode].Length;
            for (i = 0; i < len; i++)
            {
                if (i > 0)
                {
                    if (_SubsActiveKeyDic[HashCode][i].actived && (nowTime + float.Epsilon) < _SubsActiveKeyDic[HashCode][i].time && nowTime >= r)
                    {
                        return r;
                    }
                }

                if (_SubsActiveKeyDic[HashCode][i].actived)
                {
                    r = _SubsActiveKeyDic[HashCode][i].time;
                }

            }
            
            return r;
        }
        else if (_RunTimeSubsActiveKeyDic.ContainsKey(HashCode))
        {
            return _RunTimeSubsActiveKeyDic[HashCode];
        }
        else
        {
            CheckOffsetIime(nowTime, HashCode);
            return nowTime;
        }

    }

    private bool _isPlaying = false;
    public bool isPlaying
    {
        get { return _isPlaying; }
    }

    public void Play()
    {
        if(!_isStarted) return;
        //
        if (!_isPaused)
        {
            _time = 0;
        }
        else
        {
            _isPaused = false;
        }
        _isPlaying = true;
    }

    private bool _isPaused = false;

    public bool isPaused
    {
        get { return _isPaused;}
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Stop()
    {
        _time = 0;
        _isPlaying = false;
        _isPaused = false;
    }


    public void Sample(float time)
    {
        if (_time==(time) || !_isStarted) return;
        _Sample(time);
    }

    private void _Sample(float time)
    {
        _time = time;
        //
        Simulate(transform, time);
        SimulateChildren(transform, time);
    }
    
    void SimulateChildren(Transform tran, float time)
    {
        if (tran.childCount > 0)
        {
            int i, len = tran.childCount;
            for (i = 0; i < len; i++)
            {

                Transform subTransform = tran.GetChild(i);

                if (subTransform.gameObject.activeInHierarchy)
                {
                    //Sub AnimLinkage
                    AnimLinkage subAnimLinkage = subTransform.GetComponent<AnimLinkage>();
                    if (subAnimLinkage != null)
                    {
                        subAnimLinkage.Sample(time - GetOffsetTime(time, subTransform.gameObject.GetHashCode()));
                        continue;
                    }
                    
                    Simulate(subTransform, time - GetOffsetTime(time, subTransform.gameObject.GetHashCode()));

                    if (subTransform.childCount > 0)
                    {
                        SimulateChildren(subTransform, time);
                    }

                }
                else
                {
                    //s记录运行时加入的动画节点的关闭持续时间
                    CheckOffsetIime(time, subTransform.gameObject.GetHashCode());
                }
            }
        }
    }

    void Simulate(Transform tran, float time)
    {
        //Animator
        Animator at = tran.GetComponent<Animator>();
        if (at != null)
        {
            
            AnimatorStateInfo StateInfo = at.GetCurrentAnimatorStateInfo(0);
            int id = StateInfo.shortNameHash;
            AnimatorClipInfo[] info = at.GetCurrentAnimatorClipInfo(0);
            if (info != null && info.Length > 0)
            {
                if (info[0].clip != null)
                {
                    at.Play(id, 0, time/info[0].clip.length);
                    at.speed = 0;
                }
            }
        }

        //Animation
        Animation am = tran.GetComponent<Animation>();
        if (am != null)
        {
            if (!am.isPlaying)
            {
                am.Play();
            }

            AnimationState amState = GetCurrentState(am);
            if (amState == null) return;

            amState.speed = 0;
            amState.time = time;
            am.Sample();
        }

        //ParticleSystem
        ParticleSystem particle = tran.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            if (!particle.isPaused)
                particle.Pause();

            //            if (lastTime != time)
            //                tran.GetComponent<ParticleSystem>().Simulate(time - lastTime, false, false);

            particle.Simulate(time);
        }

        //ISimulateAble
        ISimulateAble script = tran.GetInterface<ISimulateAble>();
        if (script != null)
        {
            script.Process(time);
        }

    }

    

}
