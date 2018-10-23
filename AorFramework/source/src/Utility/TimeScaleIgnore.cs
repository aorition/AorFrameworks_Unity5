using System;
using System.Collections.Generic;
using Framework.Extends;
using UnityEngine;

namespace Framework.Utility
{
    /// <summary>
    /// 忽略 TimeScale
    /// @ 支持: ParticleSystem / Animation / Animator / ICustomTimeScale
    /// 
    /// </summary>
    public class TimeScaleIgnore : MonoBehaviour
    {
        private double _lastTime;
        private ParticleSystem[] _particles;
        private Animation[] _animations;
        private Animator[] _animators;
        private AnimatorUpdateMode[] _AnimatorUpdateModes;
        private ICustomTimeScale[] _customTimeScales;
        
        protected void OnEnable()
        {
            _particles = GetComponentsInChildren<ParticleSystem>(true);
            _animations = GetComponentsInChildren<Animation>(true);
            _animators = GetComponentsInChildren<Animator>(true);
            _customTimeScales = GetComponentsInChildren<ICustomTimeScale>(true);

            if (_animators != null)
            {
                _AnimatorUpdateModes = new AnimatorUpdateMode[_animators.Length];
                for (int i = 0; i < _animators.Length; i++)
                {
                    AnimatorUpdateMode orginal = _animators[i].updateMode;
                    _AnimatorUpdateModes[i] = orginal;
                    _animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }

            if (_customTimeScales != null)
            {
                for (int i = 0; i < _customTimeScales.Length; i++)
                {
                    _customTimeScales[i].Pause = true;
                }
            }
            
            _lastTime = Time.realtimeSinceStartup;
        }

        protected void OnDisable()
        {

            if (_particles != null)
            {
                for (int i = 0; i < _particles.Length; i++)
                {
                    if (_particles[i]) _particles[i].Play();
                }
            }

            if (_animators != null)
            {
                for (int i = 0; i < _animators.Length; i++)
                {
                    if (_animators[i]) _animators[i].updateMode = _AnimatorUpdateModes[i];
                }
            }

            if (_customTimeScales != null)
            {
                for (int i = 0; i < _customTimeScales.Length; i++)
                {
                    _customTimeScales[i].Pause = false;
                }
            }

            _particles = null;
            _animations = null;
            _animators = null;
            _customTimeScales = null;
        }

        protected virtual void Update()
        {
            float tpf = Time.realtimeSinceStartup - (float)_lastTime;
            float absTpf = tpf - Time.deltaTime;

            for (int i = 0; i < _particles.Length; i++)
            {
                if (_particles[i].gameObject.activeInHierarchy && (_particles[i].isPlaying || _particles[i].isPaused))
                {
                    _particles[i].Simulate(tpf, false, false);
                }
            }

            for (int i = 0; i < _animations.Length; i++)
            {
                if (_animations[i].gameObject.activeInHierarchy)
                {
                    AnimationState currentState = _animations[i].GetCurrentState();
                    if (currentState != null)
                    {
                        currentState.time += absTpf * currentState.speed;
                        _animations[i].Sample();
                    }
                }
            }

            for (int i = 0; i < _animators.Length; i++)
            {
                if (_animators[i].gameObject.activeInHierarchy)
                {
                    _animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }

            for (int i = 0; i < _customTimeScales.Length; i++)
            {
                if (((MonoBehaviour)_customTimeScales[i]).gameObject.activeInHierarchy)
                {
                    _customTimeScales[i].Update(tpf);
                }
            }


            _lastTime = Time.realtimeSinceStartup;
        }
    }
}
