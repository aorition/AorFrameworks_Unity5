using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using AorBaseUtility;
using UnityEngine;
using YoukiaCore;

namespace YoukiaUnity.View {

    /// <summary>
    /// AnimationPlus :: Animation增强组件 (第三版)
    /// author : aorition
    /// 
    /// 说明: AnimationPlus 所支持的WrapMode只需要 Loop,PingPong,ClampForever,Once(Default),其他一律按Default处理.
    /// 
    /// 已知Bug : 1.处理动画crossFade(crossFadeTime)似乎没有起到应有的效果.
    /// 
    /// </summary>
    [RequireComponent(typeof(Animation))]
    public class AnimationPlus : MonoBehaviour,IEnumerable {

        public static AnimationPlus createAnimtaionPlus(Animation anim) {
            AnimationPlus ap = anim.gameObject.AddComponent<AnimationPlus>();
          //  ap.FinishCall(ap.GetType().ToString());
            return ap;
        }

        /// <summary>
        /// 自定义动画数据TextAsset
        /// </summary>
        [SerializeField]
        private TextAsset _customClipAsset;
        public TextAsset CustomClipAsset {
            get { return _customClipAsset; }
            set {
                if (Application.isEditor) {
                    _customClipAsset = value;
                    init();
                }
                else {
                    if (value != _customClipAsset) {
                        _customClipAsset = value;
                        init();
                    }
                }
            }
        }

        [SerializeField]
        private Animation _anim;
        public Animation anim {
            get { return _anim; }
        }

        private bool _isInit = false;
        public bool isInit {
            get { return _isInit; }
        }

        private Dictionary<string, AnimationPlusState> _states;


        void Awake()
        {
            init();
        }

        //初始化方法
        private void init() {
            if (_anim == null) {
                _anim = GetComponent<Animation>();
            }

            if (_states != null) {
                _states.Clear();
            }
            else {
                _states = new Dictionary<string, AnimationPlusState>();
            }

            //初始化原生Clips
            foreach (AnimationState animState in _anim) {
                AnimationPlusState aps = new AnimationPlusState(AnimationPlusClip.CreatePlusClip(animState.clip));
                if (!_states.ContainsKey(animState.clip.name)) {
                    _states.Add(animState.clip.name, aps);
                }
            }

            //初始化自定义Clips
            if (_customClipAsset != null) {
                string ccaData = _customClipAsset.text;
                if (!string.IsNullOrEmpty(ccaData)) {
                    ccaData = ccaData.Replace("\r\n", "@");
                    string[] ccalist = ccaData.Split('@');
                    if (ccalist.Length > 0) {
                        foreach (string s in ccalist) {
                            if (!string.IsNullOrEmpty(s)) {
                                addCustomClip(s);
                            }
                        }
                    }
                }
            }
            
            _isInit = true;
        }

        private bool _isPlaying;
        public bool isPlaying {
            get { return _isPlaying; }
        }

        public bool IsPlaying(string animName) {
            if (_isPlaying) {
                if (_currentPlusState != null && _currentPlusState.Clip.animName == animName) {
                    return true;
                }
            }
            return false;
        }

        private bool _isPaused;
        public bool isPaused {
            get { return _isPaused; }
        }
        
        private AnimationPlusState _currentPlusState;
        void Update() {

            if (!_isPlaying || _currentPlusState == null) return;

            if (!_isPaused) {
                if (_currentPlusState.timeDirty) {
                    _currentPlusState.timeDirty = false;
                }
                else {
                    _currentPlusState.UpdateDeltaTime(Time.deltaTime);
                }
            }

            animationPlucCore(_currentPlusState.Clip, _currentPlusState.time, _currentPlusState.speed, _currentPlusState.reverse);

        }


        //******* 核心算法-递归
        protected void animationPlucCore(AnimationPlusClip apc, float time, float ActualSpeed, bool ActualReverse) {

            AnimationState animatState;

            float speed = ActualSpeed * apc.speed;
            if (speed == 0) {
                Debug.LogError("AnimationPlus.animationPlucCore Error : speed == 0");
            }

            bool reverse = (apc.reverse ? !ActualReverse : ActualReverse);

            if (apc.isOrginClip) {

                SendValueToAnimation(_anim[apc.animName], apc.wrapMode, speed, time, apc.clampL, apc.clampR, reverse, apc.crossFadeTime, apc.loopTimeLimit);

            }
            else {

                if (apc.SubClips != null && apc.SubClips.Length > 0) {

                    int i, len;
                    float s = 0, e;
                    if (reverse) {
                        for (i = apc.SubClips.Length - 1; i >= 0; i--) {

                            speed *= apc.SubClips[i].speed;

                            if (apc.SubClips[i].wrapMode == WrapMode.ClampForever || apc.SubClips[i].wrapMode == WrapMode.Loop || apc.SubClips[i].wrapMode == WrapMode.PingPong) {
                                if (apc.SubClips[i].loopTimeLimit > 0) {
                                    e = apc.SubClips[i].loopTimeLimit / speed + s;
                                } else {
                                    e = (apc.SubClips[i].length - apc.SubClips[i].clampL - apc.SubClips[i].clampR - apc.SubClips[i].crossFadeTime) / speed + s;
                                }
                            } else {
                                e = (apc.SubClips[i].length - apc.SubClips[i].clampL - apc.SubClips[i].clampR - apc.SubClips[i].crossFadeTime) / speed + s;
                            }
                            if (time >= s && time < e) {
                                animationPlucCore(apc.SubClips[i], time - s, speed, reverse);
                                return;
                            }
                            s = e;
                        }
                    }
                    else {
                        len = apc.SubClips.Length;
                        for (i = 0; i < len; i++) {

                            speed *= apc.SubClips[i].speed;

                            if (apc.SubClips[i].wrapMode == WrapMode.ClampForever || apc.SubClips[i].wrapMode == WrapMode.Loop || apc.SubClips[i].wrapMode == WrapMode.PingPong) {
                                if (apc.SubClips[i].loopTimeLimit > 0) {
                                    e = apc.SubClips[i].loopTimeLimit / speed + s;
                                }
                                else {
                                    e = (apc.SubClips[i].length - apc.SubClips[i].clampL - apc.SubClips[i].clampR - apc.SubClips[i].crossFadeTime) / speed + s;
                                }
                            }
                            else {
                                e = (apc.SubClips[i].length - apc.SubClips[i].clampL - apc.SubClips[i].clampR - apc.SubClips[i].crossFadeTime) / speed + s;
                            }
                            if (i == (len - 1)) {
                                if (time >= s) {
                                    animationPlucCore(apc.SubClips[i], time - s, speed, reverse);
                                    return;
                                }
                            }
                            else {
                                if (time >= s && time < e) {
                                    animationPlucCore(apc.SubClips[i], time - s, speed, reverse);
                                    return;
                                }
                            }
                            s = e;
                        }

                    }

                }

            }

        }

        //******* 核心算法-设值
        protected void SendValueToAnimation(AnimationState animatState, WrapMode wrapmode, float speed, float time, float clampL = 0, float clampR = 0, bool reverse = false, float crossFadeTime = 0, float loopTimeLimit = 0) {

            animatState.wrapMode = wrapmode;

            if (!_anim.IsPlaying(animatState.name)) {
                if (crossFadeTime > 0) {
                    _anim.CrossFade(animatState.name, crossFadeTime / speed);
                } else {
                    _anim.Play(animatState.name);
                }
            }

            float progress;

            switch (wrapmode) {
                case WrapMode.Loop:
                    if (reverse) {
                        progress = ((animatState.length - clampR) - ((time * speed) % (animatState.length - clampL - clampR))) + (clampL%speed);
                    } else {
                        progress = (time*speed)%(animatState.length - clampL - clampR) + (clampL%speed);
                    }
                    if (loopTimeLimit > 0 && time >= (loopTimeLimit * speed)) {
                        if (_anim.IsPlaying(animatState.name)) {
//                            _anim.Stop(animatState.name);
                            Stop(animatState.name);
                        }
                        return;
                    }
                    break;
                case WrapMode.PingPong:
                    if (reverse) {
                        progress = Mathf.Abs((animatState.length - clampR) - ((time*speed)%((animatState.length - clampL - clampR)*2))) + (clampL%speed);
                    } else {
                        progress = (animatState.length - clampR) - Mathf.Abs((animatState.length - clampR) - ((time * speed) % ((animatState.length - clampL - clampR) * 2))) + (clampL % speed);
                    }
                    if (loopTimeLimit > 0 && time >= (loopTimeLimit * speed)) {
                        if (_anim.IsPlaying(animatState.name)) {
//                            _anim.Stop(animatState.name);
                            Stop(animatState.name);
                        }
                        return;
                    }
                    break;
                case WrapMode.ClampForever:
                    if (reverse) {
                        progress = (animatState.length - clampR) - (time + clampL) * speed;
                        progress = Mathf.Max(progress, clampL);
                    } else {
                        progress = (time + clampL) * speed;
                        progress = Mathf.Min(progress, animatState.length - clampR);
                    }
                    if (loopTimeLimit > 0 && time >= (loopTimeLimit * speed)) {
                        if (_anim.IsPlaying(animatState.name)) {
//                            _anim.Stop(animatState.name);
                            Stop(animatState.name);
                        }
                        return;
                    }
                    break;
                default:
                    if (reverse) {
                        progress = (animatState.length - clampR) - (time + clampL) * speed;
                        progress = Mathf.Max(progress, clampL);
                        if (progress.Equals(clampL)) {
                            Stop(animatState.name);
                            return;
                        }
                    } else {
                        progress = (time + clampL)*speed;
                        progress = Mathf.Min(progress, animatState.length - clampR);
                        if (progress.Equals(animatState.length - clampR)) {
                            Stop(animatState.name);
                            return;
                        }
                    }
                    break;
            }

            animatState.time = progress;
 //           animatState.time += Time.deltaTime;
            animatState.speed = 0;
 //           Debug.Log(Time.deltaTime);
            _anim.Sample();

        }

        //=============================================
        public IEnumerator GetEnumerator() {
            return _states.Values.GetEnumerator();
        }

        protected Action<CallbackEventInfo> onPlayEventCallbck;

        //索引器
        public AnimationPlusState this[string index] {
            get {
                if (_states.ContainsKey(index)) {
                    return _states[index];
                }
                else {
                    return null;
                }
            }
        }

        public bool ContainsState(string key) {
            return _states.ContainsKey(key);
        }

        public int GetClipCount() {
            if (_isInit && _states != null) {
                return _states.Count;
            }
            else {
                return 0;
            }
        }

        public AnimationPlusClip GetClip(string clipName) {
            if (_states.ContainsKey(clipName)) {
                return _states[clipName].Clip;
            }
            return null;
        }

        public AnimationPlusClip GetClip(int index) {
            if (_states != null && _states.Count > 0) {
                int idx = 0;
                foreach (KeyValuePair<string, AnimationPlusState> animationPlusState in _states) {
                    if (idx == index) {
                        return animationPlusState.Value.Clip;
                    }
                    idx ++;
                }
            }
            return null;
        }

        /// <summary>
        /// 运行时_添加自定义Clip数据
        /// 
        /// 注意:重名添加将会覆盖原有数据
        /// 
        /// </summary>
        public bool addCustomClip(string customClipData) {
            string nClipName = AnimationPlusClip.FindNameInData(customClipData);
            if (_states != null) {
                AnimationPlusClip apc = AnimationPlusClip.CreatePlusClip(customClipData, this);
                AnimationPlusState aps = new AnimationPlusState(apc);
                if (_states.ContainsKey(nClipName)) {
                    _states[nClipName] = aps;
                }
                else {
                    _states.Add(apc.animName, aps);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 运行时_移除自定义Clip数据
        /// </summary>
        public void reamoveCustomClip(string customClipName) {
            if (_states.ContainsKey(customClipName)) {
                _states.Remove(customClipName);
            }
        }

        /// <summary>
        /// 立即播放    ***注意Animation兼容模式无法响应动画模式为Loop或者PingPong的onEventCallback回调函数
        /// 
        /// </summary>
        /// <param name="animTagName"></param>
        /// <param name="onEventCallback"></param>
        /// <param name="overrideInfo"></param>
        public void Play(string animTagName, Action<CallbackEventInfo> onEventCallback = null) {

            if (!_states.ContainsKey(animTagName)) {
                Log.Error("AnimationPlus.Play Error : 没有找到[" + transform.parent.name + "." + animTagName + ".");
                return;

            }

            if (onPlayEventCallbck != null) {
                Action<CallbackEventInfo> callback = onPlayEventCallbck;
                onPlayEventCallbck = null;
                callback(new CallbackEventInfo(this.gameObject, _currentPlusState.Clip.animName, false));
            }

            if (_anim && _states != null) {

                _currentPlusState = _states[animTagName];

                if (_currentPlusState != null) {

                    if (onEventCallback != null) {
                            onPlayEventCallbck = onEventCallback;
                    }
                    
                    _currentPlusState.time = 0f;
                    _isPaused = false;
                    _isPlaying = true;
                }
            }
        }

        /// <summary>
        /// 非安全方式的播放接口
        /// </summary>
        public void UnsafePlay(AnimationPlusState state, float time, bool paused = false) {
            if (_anim != null && state != null && state.Clip != null) {
                if (state.Clip.isOrginClip || (state.Clip.SubClips != null && state.Clip.SubClips.Length > 0)) {
                    _currentPlusState = state;
                    _currentPlusState.time = time;
                    _isPaused = paused;
                    _isPlaying = true;
                }
            }
        }

        public void Pause() {
            _isPaused = true;
        }

        public void Stop(string name) {
            if (_currentPlusState == null) return;

            if (_anim && _anim.isPlaying) {
                _isPlaying = false;
                _anim.Stop(name);

                if (onPlayEventCallbck != null) {
                    Action<CallbackEventInfo> callback = onPlayEventCallbck;
                    onPlayEventCallbck = null;
                    if (_currentPlusState != null) {
                        callback(new CallbackEventInfo(this.gameObject, _currentPlusState.Clip.animName, true));
                    }
                }

                _currentPlusState = null;

            }
        }

        public void Stop() {

            if (_currentPlusState == null) return;

            if (_anim && _anim.isPlaying) {
                _isPlaying = false;
                _anim.Stop();

                if (onPlayEventCallbck != null) {
                    Action<CallbackEventInfo> callback = onPlayEventCallbck;
                    onPlayEventCallbck = null;
                    if (_currentPlusState != null) {
                        callback(new CallbackEventInfo(this.gameObject, _currentPlusState.Clip.animName, true));
                    }
                }

                _currentPlusState = null;

            }
        }

    }

}
