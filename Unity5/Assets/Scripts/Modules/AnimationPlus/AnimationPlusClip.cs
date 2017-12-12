using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.View {
    
    //运行时Model类
    public class AnimationPlusClip {

        /// <summary>
        /// 从字符串数据里获取ClipName
        /// </summary>
        /// <param name="customClipData"></param>
        /// <returns></returns>
        public static string FindNameInData(string customClipData) {
            ///     {animName:string,isOrginClip:bool}
            customClipData = customClipData.Substring(1, customClipData.Length - 2);
            string n = customClipData.Substring(0, customClipData.IndexOf(','));
            return n;
        }

        /// <summary>
        /// 通过原生AnimationClip,创建AnimationPlusClip
        /// </summary>
        public static AnimationPlusClip CreatePlusClip(AnimationClip ac) {
            return new AnimationPlusClip(ac.name, true, ac.wrapMode, ac.length, 1f, ac);
        }

        /// <summary>
        /// 通过StringData,创建AnimationPlusClip
        /// </summary>
        /// <param name="data">
        /// 
        /// 数据结构:
        ///     {animName:string,isOrginClip:bool,wrapMode:string,
        ///         length:float,loopTimeLimit:float,crossFadeTime:float,reverse:bool,
        ///         clampL:float,clampR:float,[{animName:string,isOrginClip:bool,...},...]
        ///     } //自定义Clip
        /// </param>
        public static AnimationPlusClip CreatePlusClip(string data, AnimationPlus manager, int subLayerId = 0) {

            data = data.Substring(1, data.Length - 2);

            string[] ctDatas = data.Substring(0, data.IndexOf("[") - 1).Split(',');
            string subDataStr = data.Substring(data.IndexOf("[") + 1, data.Length - data.IndexOf("[") - 2);
            string[] subData = null;
            if (!string.IsNullOrEmpty(subDataStr)) {
                string ft = "\n\t" + getTformat(subLayerId);
                subDataStr = subDataStr.Substring(ft.Length); //去前
                subDataStr = subDataStr.Substring(0, subDataStr.Length - 1); //去后
                string l = "}," + ft + "{";
                if (subDataStr.IndexOf(l) != -1) {
                    subData = subDataStr.Replace(l, "}|{").Split('|');
                }
                else {
                    subData = new[] {subDataStr};
                }
            }
            AnimationPlusClip[] apcs = null;
            if (subData != null) {
                int len = subData.Length;
                if (len > 0) {
                    apcs = new AnimationPlusClip[subData.Length];
                    for (int i = 0; i < apcs.Length; i++) {
                        apcs[i] = CreatePlusClip(subData[i], manager, subLayerId + 1);
                    }
                }
            }
            string clipName = ctDatas[0];
            bool isOrgin = bool.Parse(ctDatas[1]);
            AnimationClip orginClip = null;
            if (isOrgin) {
                orginClip = manager.anim.GetClip(clipName);
            }
            AnimationPlusClip apc = new AnimationPlusClip(
                clipName, isOrgin, (WrapMode)Enum.Parse(typeof(WrapMode), ctDatas[2]),
                float.Parse(ctDatas[3]), float.Parse(ctDatas[4]), orginClip, float.Parse(ctDatas[5]), float.Parse(ctDatas[6]),
                bool.Parse(ctDatas[7]), float.Parse(ctDatas[8]), float.Parse(ctDatas[9]), apcs
            );

            if (apcs != null && apcs.Length > 0) {
                for (int i = 0; i < apcs.Length; i++) {
                    apcs[i].parentPlusClip = apc;
                }
            }

            return apc;

        }

        /// <summary>
        /// 构造
        /// </summary>
        public AnimationPlusClip(
            string animName, bool isOrginClip,
            WrapMode wrapMode, float length, float speed,
            AnimationClip orginClip,
            float loopTimeLimit = 0, float crossFadeTime = 0, bool reverse = false,
            float clampL = 0, float clampR = 0, AnimationPlusClip[] SubClips = null
        ) {
            _orginClip = orginClip;
            _animName = animName;
            _isOrginClip = isOrginClip;
            _wrapMode = wrapMode;
            _length = length;
            _speed = speed;
            _loopTimeLimit = loopTimeLimit;
            _crossFadeTime = crossFadeTime;
            _reverse = reverse;
            _clampL = clampL;
            _clampR = clampR;
            _SubClips = SubClips;
        }

        public AnimationPlusClip clone() {
             AnimationPlusClip n = new AnimationPlusClip(
                _animName, _isOrginClip, _wrapMode, 
                _length, _speed, _orginClip, _loopTimeLimit, 
                _crossFadeTime, _reverse, _clampL, _clampR, _SubClips
                );
            n.parentPlusClip = (this.parentPlusClip != null ? this.parentPlusClip.clone() : null);
            return n;
        }

        /// <summary>
        /// 将数据导出为字符串数据
        /// </summary>
        public string toDataString(int subLayerId = 0) {
            string data = "{"
                            + animName + ","
                            + _isOrginClip.ToString() + ","
                            + _wrapMode.ToString() + ","
                            + _length + ","
                            + _speed + ","
                            + _loopTimeLimit + ","
                            + _crossFadeTime + ","
                            + _reverse.ToString() + ","
                            + _clampL + ","
                            + _clampR + ",";
            string subStr = "[";
            if (SubClips != null) {
                if (SubClips.Length > 0) {
                    subStr += "\n";
                    for (int i = 0; i < SubClips.Length; i++) {
                        if (i > 0) {
                            subStr += ",\n";
                        }
                        subStr += "\t" + getTformat(subLayerId) + SubClips[i].toDataString(subLayerId + 1);
                    }
                    subStr += "\n";
                }
            }
            subStr += "]";
            data += subStr + "}";
            return data;
        }

        private static string getTformat(int subLayerId) {
            string o = "";
            for (int i = 0; i < subLayerId; i++) {
                o += "\t";
            }
            return o;
        }

        //

        //==================================================================================== 属性定义

        private string _animName;
        public string animName {
            get { return _animName; }
        }

        /// <summary>
        /// 是否是原生动画片段
        /// </summary>
        private bool _isOrginClip;
        public bool isOrginClip {
            get { return _isOrginClip; }
        }

        private AnimationClip _orginClip;

        public AnimationClip orginClip {
            get { return _orginClip; }
        }

        /// <summary>
        /// 动画模式
        /// </summary>
        private WrapMode _wrapMode;
        public WrapMode wrapMode {
            get {
                return _wrapMode;
            }
        }

        /// <summary>
        /// 动画长度(秒)
        /// 
        /// </summary>
        private float _length;
        public float length {
            get {
                return _length;
            }
        }
        
        /// <summary>
        /// 动画速度(倍率)
        /// </summary>
        private float _speed;
        public float speed {
            get { return _speed; }
        }

        /// <summary>
        /// 循环动画播放时间限制(秒),定义该动画最大播放多少秒后就会停止,该值为<0时为不限制.
        /// </summary>
        private float _loopTimeLimit;
        public float loopTimeLimit {
            get { return _loopTimeLimit; }
        }

        /// <summary>
        /// 动画融合时间(秒)
        /// </summary>
        private float _crossFadeTime;
        public float crossFadeTime {
            get { return _crossFadeTime; }
        }

        /// <summary>
        /// 反向播放
        /// </summary>
        private bool _reverse;
        public bool reverse {
            get { return _reverse; }
        }

        private float _clampL;
        public float clampL {
            get { return _clampL; }
        }

        private float _clampR;
        public float clampR {
            get { return _clampR; }
        }

        public AnimationPlusClip parentPlusClip;

        private AnimationPlusClip[] _SubClips;
        public AnimationPlusClip[] SubClips {
            get { return _SubClips; }
        }

        //==================================================================================== 方法定义

        /// <summary>
        /// 获取实际计算后的Length
        /// </summary>
        /// <returns></returns>
//        public float getActualLength() {
//            return (_length - _clampL - _clampR)/_speed;
//        }

        /// <summary>
        /// 获取实际计算后的Speed
        /// </summary>
        public float getActualSpeed() {
            return getActualSpeed(this, _speed);
        }
        private static float getActualSpeed(AnimationPlusClip sub, float value) {
            if (sub.parentPlusClip != null) {
                return getActualSpeed(sub.parentPlusClip, value * sub.parentPlusClip.speed);
            } else {
                return value;
            }
        }

        /// <summary>
        /// 获取实际计算后的Reverse
        /// </summary>
//        public bool getActualReverse() {
//            return getActualReverse(this, _reverse);
//        }
//        private static bool getActualReverse(AnimationPlusClip sub, bool value) {
//            if (sub.parentPlusClip != null) {
//                if (sub.parentPlusClip.reverse) {
//                    bool r = !value;
//                    return getActualReverse(sub.parentPlusClip, r);
//                }
//                else {
//                    return getActualReverse(sub.parentPlusClip, value);
//                }
//            }
//            else {
//                return value;
//            }
//        }

    }
}
