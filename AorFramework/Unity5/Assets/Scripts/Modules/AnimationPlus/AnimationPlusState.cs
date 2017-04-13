using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.View {
    public class AnimationPlusState {


        public AnimationPlusState(AnimationPlusClip clip) {
            _clip = clip;
            wrapMode = _clip.wrapMode;
        }

        /// <summary>
        /// 引用的Clip
        /// </summary>
        private AnimationPlusClip _clip;
        public AnimationPlusClip Clip {
            get { return _clip; }
        }

        public float length {
            get { return (_clip.length - _clip.clampL - _clip.clampR) / _clip.getActualSpeed(); }
        }

        public WrapMode wrapMode;

        private bool _reverse;
        public bool reverse {
            get { return _reverse; }
            set {
                if (!value.Equals(_reverse)) {
                    _reverse = value;
                }
            }
        }

        /// <summary>
        /// 播放速度(倍率, state速度倍率)
        /// 注意,此值不为负数. 兼容逻辑: 如果对齐设置负数,将取此值的绝对值并将reverse属性取非;
        /// </summary>
        private float _speed = 1f;
        public float speed {
            get { return _speed; }
            set {
                if (!value.Equals(speed)) {

                    if (value < 0) {
                        _speed = -(value);
                        _reverse = !_reverse;
                    }
                    else {
                        _speed = value;
                    }
                }
            }
        }
        
        /// <summary>
        /// 播放时长
        /// </summary>
        private float _time = 0;
        public float time {
            get { return _time; }
            set {
                if (!_time.Equals(value)) {
                    _time = value;
                    timeDirty = true;
                }
            }
        }

        public void UpdateDeltaTime(float deltaTime) {
            _time += deltaTime;
        }

        /// <summary>
        /// 播放头变更的标记 ************ ??
        /// </summary>
        public bool timeDirty;


    }
}
