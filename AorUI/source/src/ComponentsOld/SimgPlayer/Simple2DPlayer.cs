using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExoticCore;
using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.Debug;
using ExoticUnity.Resource;

namespace ExoticUnity.GUI.AorUI.Components
{
    /// <summary>
    /// SimgPlayer 
    /// author : Aorition
    /// 
    /// 简单的Sprite动画播放器
    /// 
    /// 需求: 
    ///     AtlasTexture2D需求一张Sprite图集,内含Sprite命名规则为> [动作名]_[序列], 例如: idle_0,idle_1,run_0,run_1 ... 播放时,传入动作名即可实现相关动画序列的播放.
    /// 
    /// 
    /// 注意 : WrapMode为 Loop,PingPong,ClampForever时播放会一直持续至于手动调用Stop方法
    /// 
    /// </summary>
    public class Simple2DPlayer : MonoSwitch
    {

        public float FPS = 30f;

        [SerializeField]
        [HideInInspector]
        private string _AtlasPath;

        [SerializeField]
        private Texture2D _AtlasTexture2D;

        public bool UseFixedUpdate;

        protected List<Sprite> _PimgList;
        private ResourcesManager.ResourceRef _atlasRef;
        //[SerializeField]
        //   [HideInInspector]
        private Sprite[] allSprite;

        [Header("默认动画")]
        public string AnimName;
        public WrapMode ClipWrapMode = WrapMode.Loop;


        private WrapMode _currentWrapMode;
        private string _currentAnimName;
        private bool _currentReverse;
        private bool _isPlaying;
        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        private float _currentClipLength;
        public float currentClipLength
        {
            get { return _currentClipLength; }
        }

        private float _f;
        protected int _frameNum;

        private float _time;
        public float time
        {
            get { return _time; }
        }



        public override void OnAwake()
        {
            base.OnAwake();

            _PimgList = new List<Sprite>();

        }

        private bool _isInit;

        protected override void Initialization()
        {
            base.Initialization();

            if ((allSprite == null || allSprite.Length == 0) && !string.IsNullOrEmpty(_AtlasPath))
            {
                AorUIAssetLoader.LoadAllSprites(_AtlasPath, (sps, objs) =>
                {
                    if (this == null) return;

                    if (objs != null && objs.Length > 0)
                    {
                        _atlasRef = objs[0] as ResourcesManager.ResourceRef;
                    }

                    allSprite = sps;

                    _isInit = true;
                    if (AnimName != null)
                    {
                        Play(AnimName, ClipWrapMode);
                    }


                });
            }

            _isInit = true;
            if (AnimName != null)
            {
                Play(AnimName, ClipWrapMode);
            }

        }

        protected override void OnDestroy()
        {

            if (_PimgList != null)
            {
                _PimgList.Clear();
                _PimgList = null;
            }
            _atlasRef = null;

            base.OnDestroy();
        }

        public void setAtlasPath(string atlasPath)
        {
            _AtlasPath = atlasPath;
        }

        public void Stop()
        {
            _isPlaying = false;
            _frameNum = 0;
            _time = 0;
            _f = 0;

            OnStop();

            if (onStop != null)
            {
                onStop();
            }
        }

        protected virtual void OnStop()
        {

        }

        public CallBack onStop;


        private bool _isWaitForInit;
        private IEnumerator waitForInitDo(string clipName, WrapMode wrapMode, bool revers)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (_isInit)
                {
                    _isWaitForInit = false;
                    Play(clipName, wrapMode, revers);
                    break;
                }
            }
        }

        public bool Play(string clipName, WrapMode wrapMode = WrapMode.Loop, bool reverse = false)
        {

            if (!_isInit)
            {
                if (!_isWaitForInit)
                {
                    _isWaitForInit = true;
                    StartCoroutine(waitForInitDo(clipName, wrapMode, reverse));
                }
                return false;
            }

            _currentAnimName = clipName;
            AnimName = _currentAnimName;

            _currentWrapMode = wrapMode;
            _currentReverse = reverse;

            //            if (_atlasRef == null)
            //            {
            //                return false;
            //            }

            if (_PimgList.Count > 0)
            {
                _PimgList.Clear();
            }




            //            if (_atlasRef != null && allSprite!=null && allSprite.Length>0)
            //            {
            if (allSprite != null && allSprite.Length > 0)
            {
                int i, len = allSprite.Length;
                for (i = 0; i < len; i++)
                {
                    Sprite s = allSprite[i];
                    //string[] cks = s.name.Split('_');
                    if (s.name.StartsWith(clipName))
                    {
                        _PimgList.Add(s);
                    }
                }

                if (_PimgList.Count == 0)
                {
                    UiLog.Error("SimgPlayer.Play Error :: 找不到clipName用于播放, clipName == " + clipName);
                    return false;
                }

                _currentClipLength = _PimgList.Count * (1 / FPS);

                //排序
                _PimgList.Sort((x, y) =>
                {

                    string[] xStrings = x.name.Split('_');
                    string[] yStrings = y.name.Split('_');

                    int xNum = int.Parse(xStrings[xStrings.Length - 1]);
                    int yNum = int.Parse(yStrings[xStrings.Length - 1]);
                    if (xNum > yNum)
                    {
                        return 1;
                    }
                    else if (xNum == yNum)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                });

                if (_currentReverse)
                {
                    _frameNum = _PimgList.Count - 1;
                }
                else
                {
                    _frameNum = 0;
                }

                _time = 0;
                _f = 0;

                updateSprite();


                _isPlaying = true;


            }

            return true;
        }



        protected virtual void updateSprite()
        {
            // _image.sprite = _PimgList[_frameNum];
        }

        private void LoopCore()
        {
            if (_f >= 1 / FPS)
            {
                _f = 0;

                _frameNum = (_currentReverse == true ? _frameNum - 1 : _frameNum + 1);

                if (_frameNum >= _PimgList.Count || _frameNum < 0)
                {
                    //单次结束
                    if (_currentReverse)
                    {
                        //反向
                        switch (_currentWrapMode)
                        {
                            case WrapMode.Loop:
                                if (_frameNum < 0)
                                {
                                    _frameNum = _PimgList.Count - 1;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.PingPong:
                                if (_frameNum < 0)
                                {
                                    _currentReverse = false;
                                    _frameNum = 0;
                                    updateSprite();
                                }
                                else if (_frameNum > _PimgList.Count - 1)
                                {
                                    _currentReverse = true;
                                    _frameNum = _PimgList.Count - 1;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.ClampForever:
                                if (_frameNum < 0)
                                {
                                    _frameNum = 0;
                                }
                                break;
                            default:
                                if (_frameNum < 0)
                                {
                                    Stop();
                                }
                                break;
                        }
                    }
                    else
                    {
                        //正向
                        switch (_currentWrapMode)
                        {
                            case WrapMode.Loop:
                                if (_frameNum > _PimgList.Count - 1)
                                {
                                    _frameNum = 0;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.PingPong:
                                if (_frameNum < 0)
                                {
                                    _currentReverse = false;
                                    _frameNum = 0;
                                    updateSprite();

                                }
                                else if (_frameNum > _PimgList.Count - 1)
                                {
                                    _currentReverse = true;
                                    _frameNum = _PimgList.Count - 1;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.ClampForever:
                                if (_frameNum > _PimgList.Count - 1)
                                {
                                    _frameNum = _PimgList.Count - 1;
                                }
                                break;
                            default:
                                if (_frameNum > _PimgList.Count - 1)
                                {
                                    Stop();
                                }
                                break;
                        }
                    }
                }
                else
                {
                    updateSprite();
                }
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (UseFixedUpdate) return;

            if (_isPlaying)
            {
                _time += Time.deltaTime;
                _f += Time.deltaTime;
                LoopCore();
            }

        }

        private void FixedUpdate()
        {
            if (!UseFixedUpdate) return;
            if (_isPlaying)
            {
                _time += Time.fixedDeltaTime;
                _f += Time.fixedDeltaTime;
                LoopCore();
            }
        }

    }
}
