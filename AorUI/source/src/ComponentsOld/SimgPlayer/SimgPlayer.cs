using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YoukiaCore;
using YoukiaUnity.App;
using YoukiaUnity.GUI.AorUI.Core;
using YoukiaUnity.GUI.AorUI.Debug;
using YoukiaUnity.Resource;

namespace YoukiaUnity.GUI.AorUI.Components {
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
    public class SimgPlayer : MonoSwitch {

        WrapMode wp = WrapMode.PingPong;

        public float FPS = 30f;

        [SerializeField][HideInInspector]
        private string _AtlasPath;

        [SerializeField]
        private Texture2D _AtlasTexture2D;

        public bool UseFixedUpdate; 

        private List<Sprite> _PimgList;
        private ResourcesManager.ResourceRef _atlasRef;

        private Image _image;
        private bool _hasImage;

        public override void OnScriptCoverFinish() {
            base.OnScriptCoverFinish();

            _PimgList = new List<Sprite>();

        }

        protected override void Initialization() {
            base.Initialization();

            if (_image == null) {
                _image = GetComponent<Image>();
                if (_image == null) {
                    _image = gameObject.AddComponent<Image>();
                    _hasImage = false;
                    imageHide(true);
                }
                else {
                    _hasImage = true;
                }
            }

        }

        protected override void OnDestroy() {
            
            if (_PimgList != null) {
                _PimgList.Clear();
                _PimgList = null;
            }
            _atlasRef = null;

            base.OnDestroy();
        }

        public void setAtlasPath(string atlasPath) {
            _AtlasPath = atlasPath;
        }

        public void Stop() {
            _isPlaying = false;
            _frameNum = 0;
            _time = 0;
            _f = 0;
            if (!_hasImage) {
                imageHide(true);
            }
            if (onStop != null) {
                onStop();
            }
        }

        public CallBack onStop;

        public void Play(string clipName, WrapMode wrapMode, bool reverse = false) {

            if (_PimgList.Count > 0) {
                _PimgList.Clear();
            }

            _currentWrapMode = wrapMode;
            _currentReverse = reverse;

            if (!string.IsNullOrEmpty(_AtlasPath)) {
                AorUIAssetLoader.LoadAllSprites(_AtlasPath, (sps, objs) => {

                    if (objs != null && objs.Length > 0) {
                        _atlasRef = objs[0] as ResourcesManager.ResourceRef;
                    }

                    int i, len = sps.Length;
                    for (i = 0; i < len; i++) {
                        Sprite s = sps[i];
                        string[] cks = s.name.Split('_');
                        if (cks[0] == clipName) {
                            _PimgList.Add(s);
                        }
                    }

                    if (_PimgList.Count == 0) {
                        UiLog.Error("SimgPlayer.Play Error :: 找不到clipName用于播放, clipName == " + clipName);
                        return;
                    }

                    _currentClipLength = _PimgList.Count*(1/FPS);

                    //排序
                    _PimgList.Sort((x, y) => {
                        int xNum = int.Parse(x.name.Split('_')[1]);
                        int yNum = int.Parse(y.name.Split('_')[1]);
                        if (xNum > yNum) {
                            return 1;
                        }else {
                            return -1;
                        }
                    });

                    if (_currentReverse) {
                        _frameNum = _PimgList.Count - 1;
                    }
                    else {
                        _frameNum = 0;
                    }

                    _time = 0;
                    _f = 0;

                    updateSprite();
                    if (!_hasImage) {
                        imageHide(false);
                    }

                    _isPlaying = true;

                });
            }
        }
        
        private void imageHide(bool isHide) {
            if (isHide) {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
            }
            else {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
            }
        }

        private void updateSprite() {
            _image.sprite = _PimgList[_frameNum];
        }

        private WrapMode _currentWrapMode;
        private bool _currentReverse;
        private bool _isPlaying;
        public bool isPlaying {
            get { return _isPlaying; }
        }

        private float _currentClipLength;
        public float currentClipLength {
            get { return _currentClipLength; }
        }

        private float _f;
        private int _frameNum;

        private float _time;
        public float time {
            get { return _time; }
        }

        private void LoopCore() {
            if (_f >= 1 / FPS) {
                _f = 0;

                _frameNum = (_currentReverse == true ? _frameNum - 1 : _frameNum + 1);

                if (_frameNum >= _PimgList.Count || _frameNum < 0) {
                    //单次结束
                    if (_currentReverse) {
                        //反向
                        switch (_currentWrapMode) {
                            case WrapMode.Loop:
                                if (_frameNum < 0) {
                                    _frameNum = _PimgList.Count - 1;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.PingPong:
                                if (_frameNum < 0) {
                                    _currentReverse = false;
                                    _frameNum = 0;
                                    updateSprite();
                                } else if (_frameNum > _PimgList.Count - 1) {
                                    _currentReverse = true;
                                    _frameNum = _PimgList.Count - 1;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.ClampForever:
                                if (_frameNum < 0) {
                                    _frameNum = 0;
                                }
                                break;
                            default:
                                if (_frameNum < 0) {
                                    Stop();
                                }
                                break;
                        }
                    } else {
                        //正向
                        switch (_currentWrapMode) {
                            case WrapMode.Loop:
                                if (_frameNum > _PimgList.Count - 1) {
                                    _frameNum = 0;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.PingPong:
                                if (_frameNum < 0) {
                                    _currentReverse = false;
                                    _frameNum = 0;
                                    updateSprite();

                                } else if (_frameNum > _PimgList.Count - 1) {
                                    _currentReverse = true;
                                    _frameNum = _PimgList.Count - 1;
                                    updateSprite();
                                }
                                break;
                            case WrapMode.ClampForever:
                                if (_frameNum > _PimgList.Count - 1) {
                                    _frameNum = _PimgList.Count - 1;
                                }
                                break;
                            default:
                                if (_frameNum > _PimgList.Count - 1) {
                                    Stop();
                                }
                                break;
                        }
                    }
                } else {
                    updateSprite();
                }
            }
        }

        protected override void OnUpdate() {
            base.OnUpdate();

            if (UseFixedUpdate) return;

            if (_isPlaying) {
                _time += Time.deltaTime;
                _f += Time.deltaTime;
                LoopCore();
            }
            
        }

        private void FixedUpdate() {
            if (!UseFixedUpdate) return;
            if (_isPlaying) {
                _time += Time.fixedDeltaTime;
                _f += Time.fixedDeltaTime;
                LoopCore();
            }
        }

    }
}
