using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.module
{

    public enum SplineWalkerMode
    {
        /// <summary>
        /// Walk the path once from start to end.
        /// 从路径头移动到路径尾,动画结束
        /// </summary>
        Once,

        /// <summary>
        /// Walk the path from start to end and then again from start to end in a loop.
        /// 循环播放从路径头移动到路径尾. *** 提示: 可以设置 SplineAnimation.runAnimTime 属性然后其运行多少秒后停止.
        /// </summary>
        Loop,

        /// <summary>
        /// Cycle from start to end and then go back from end to start.
        /// 播放从路径头移动到路径尾,再反向移动至路径头,如此循环 *** 提示: 可以设置 SplineAnimation.runAnimTime 属性然后其运行多少秒后停止.
        /// </summary>
        PingPong
    }


    /// <summary>
    /// 类说明：曲线轨迹动画
    /// 作者：刘耀鑫
    /// update: Aorition
    /// </summary>

    public class SplineAnimation : MonoBehaviour
    {

        [SerializeField]
        protected Spline _spline;
        public Spline spline
        {
            get { return _spline; }
        }

        /// <summary>
        /// 反向动画
        /// </summary>
        private bool _reverse = true;

        [SerializeField]
        protected float _velocity = 1;
        /// <summary>
        /// 物件移动速率, 注意:当isTimeAnimation为true时其值自动被赋值,手动设置的值会被覆盖.
        /// </summary>
        public float velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        [SerializeField]
        protected bool _ApplyLineDirection = false;
        /// <summary>
        /// 是否启用物件跟随路径改变方向,默认为false
        /// </summary>
        public bool ApplyLineDirection
        {
            get { return _ApplyLineDirection; }
            set { _ApplyLineDirection = value; }
        }

        [SerializeField]
        protected bool _lockUPDirection = false;
        /// <summary>
        /// 是否锁定Up轴方向,仅在ApplyLineDirection属性为true时生效
        /// </summary>
        public bool lockUPDirection
        {
            get { return _lockUPDirection; }
            set { _lockUPDirection = value; }
        }

        /// <summary>
        /// 动画模式
        /// </summary>
        [SerializeField]
        protected SplineWalkerMode _mode = SplineWalkerMode.Once;
        public SplineWalkerMode mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        [SerializeField]
        protected AnimationCurve _easeCurve;
        public AnimationCurve easeCurve
        {
            get { return _easeCurve; }
            set { _easeCurve = value; }
        }

        //
        [SerializeField]
        protected bool _isTimeAnimation = false;
        public bool isTimeAnimation
        {
            get { return _isTimeAnimation; }
            set { _isTimeAnimation = value; }
        }

        protected Action<GameObject> _OnPlayEnd;

        /// <summary>
        /// 表示完成从路径头到路径尾移动需要的时间(秒), 当isTimeAnimation为true时产生作用.
        /// </summary>
        [SerializeField]
        protected float _animationTime;
        public float animationTime
        {
            get { return _animationTime; }
            set { _animationTime = value; }
        }

        [SerializeField]
        protected float _runAnimTime = 0;
        /// <summary>
        /// 按运行时间(秒)方式来结束路径动画, 默认值为0,即不使用此方式. *** 提示Loop/PingPong这两种模式下,如果不指定此值则会永久循环播放动画,并不会触发_OnPlayEnd委托.
        /// </summary>
        public float runAnimTime
        {
            get { return _runAnimTime; }
            set { _runAnimTime = value; }
        }


        private int _keyIndex = -1;

        private bool _isPlaying;
        /// <summary>
        /// 是否正在播放路径动画
        /// </summary>
        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        private bool _isPause;
        /// <summary>
        /// 是否暂停播放动画
        /// </summary>
        public bool isPause
        {
            get { return _isPause; }
        }

        private float _delayTime;

        [SerializeField]
        protected Vector3 _offsetPos;

        private float _T;
        private float _progress;

        /// <summary>
        /// The walk progress (_easeCurve parameter in [0, 1]).
        /// 路径动画的进度值[0,1] (_easeCurve parameter in [0, 1]).
        /// </summary>
        public float Progress
        {
            get
            {
                return _progress;
            }
        }

        private float _usedTime;
        /// <summary>
        /// 单次(端到端)路径动画使用了多少时间
        /// </summary>
        public float usedTime
        {
            get
            {
                return _usedTime;
            }
        }

        private float _totalTime;
        /// <summary>
        /// 本次播放的路径动画使用了多少时间(秒)
        /// </summary>
        public float totalTime
        {
            get
            {
                return _totalTime;
            }
        }

        /// <summary>
        /// 路径动画事件回调委托
        /// </summary>
        public Action<GameObject, SplineAnimation.SplineAnimKeyInfo> onSplineAnimKeyInfo;

        [SerializeField]
        protected List<float> _keyList = new List<float>();
        public List<float> keyList
        {
            get { return _keyList; }
        }

        [SerializeField]
        private List<SplineAnimKeyInfo> _infoList = new List<SplineAnimKeyInfo>();
        public List<SplineAnimKeyInfo> InfoList
        {
            get { return _infoList; }
        }

        void Start()
        {
            if (_easeCurve == null || _easeCurve.length == 0)
            {
                _easeCurve = AnimationCurve.Linear(0, 1, 1, 1);
            }
        }

        void Update()
        {

            if (_spline == null) return;

            if (isPlaying && !isPause)
            {
                animationLoop();
            }
        }

        void OnDestroy()
        {
            _spline = null;
            _easeCurve = null;

            _keyList.Clear();
            _keyList = null;

            _infoList.Clear();
            _infoList = null;

            _OnPlayEnd = null;
        }

        public void AddKey(float key, SplineAnimKeyInfo keyInfo)
        {
            if (!_keyList.Contains(key))
            {
                _keyList.Add(key);
                _infoList.Add(keyInfo);
            }
        }

        public bool ContainsKey(float key)
        {
            return _keyList.Contains(key);
        }

        public void ClearKey()
        {
            _keyList.Clear();
            _infoList.Clear();
        }

        public void DeleteKey(float key)
        {
            int index = _keyList.IndexOf(key);
            if (index >= 0)
            {
                _keyList.RemoveAt(index);
                _infoList.RemoveAt(index);
            }
        }

        /// <summary>
        /// 播放路径动画
        /// </summary>
        /// <param name="spline">路径</param>
        /// <param name="mode">路径动画模式</param>
        /// <param name="offsetPos">路径动画的位置偏移</param>
        /// <param name="applyLineDrrection">是否物件跟随路径的方向</param>
        /// <param name="lockUpDirection">是否锁定物件Up轴旋转方向,仅当applyLineDrrection为true时有效</param>
        /// <param name="value">一个值当valueIsTime为true时,此值表示animationTime属性值,当valueIsTime为false时,此值表示velocity属性值, 默认值为0,即不覆盖原始数据</param>
        /// <param name="valueIsTime">标识value形参的作用.为true时,value表示animationTime属性值,为false时,value表示velocity属性值</param>
        /// <param name="onEventCallback">动画结束后的回调委托</param>
        /// <param name="reverse">反向运行动画</param>
        /// <param name="runAnimTime">动画运行时间,当该值大于0时启用,表示该次动画运行的时间(秒),结束时调用OnPlayEnd回调委托</param>
        /// <param name="EaseCure">缓动曲线</param>
        public void Play(Spline spline, SplineWalkerMode mode, Vector3 offsetPos, bool reverse, bool applyLineDrrection, bool lockUpDirection,
                          float value = 0, bool valueIsTime = false, Action<GameObject> OnPlayEnd = null,
                          float runAnimTime = 0, AnimationCurve EaseCure = null
        )
        {
            _spline = spline;
            _mode = mode;
            _offsetPos = offsetPos;
            _reverse = reverse;
            _ApplyLineDirection = applyLineDrrection;
            _lockUPDirection = lockUpDirection;
            Play(value, valueIsTime, OnPlayEnd, runAnimTime, EaseCure);
        }
        /// <summary>
        /// 播放路径动画
        /// </summary>
        /// <param name="value">一个值当valueIsTime为true时,此值表示animationTime属性值,当valueIsTime为false时,此值表示velocity属性值, 默认值为0,即不覆盖原始数据</param>
        /// <param name="valueIsTime">标识value形参的作用.为true时,value表示animationTime属性值,为false时,value表示velocity属性值</param>
        /// <param name="onEventCallback">动画结束后的回调委托</param>
        /// <param name="reverse">反向运行动画</param>
        /// <param name="runAnimTime">动画运行时间,当该值大于0时启用,表示该次动画运行的时间(秒),结束时调用OnPlayEnd回调委托</param>
        /// <param name="EaseCure">缓动曲线</param>
        public void Play(float value = 0, bool valueIsTime = false, Action<GameObject> OnPlayEnd = null,
                          float runAnimTime = 0, AnimationCurve EaseCure = null
            )
        {

            if (value > 0)
            {
                if (valueIsTime)
                {
                    _isTimeAnimation = true;
                    _animationTime = value;
                }
                else
                {
                    _velocity = value;
                }
            }
            _runAnimTime = runAnimTime;
            _OnPlayEnd = OnPlayEnd;

            if (EaseCure != null)
            {
                _easeCurve = EaseCure;
            }

            if (_spline == null) return;
            if (_reverse)
            {
                _progress = 1;
                _T = 1;
            }
            else
            {
                _progress = 0;
                _T = 0;
            }
            _isPlaying = true;

        }

        /// <summary>
        /// 停止路径动画
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _T = 0;
            _progress = 0;
            _keyIndex = -1;
            _usedTime = 0;
            _totalTime = 0;
            if (_OnPlayEnd != null)
            {
                _OnPlayEnd(gameObject);
                _OnPlayEnd = null;
            }
        }

        /// <summary>
        /// 暂停路径动画
        /// </summary>
        public void Pause()
        {
            _isPause = true;
        }

        /// <summary>
        /// 继续播放路径动画,仅当执行过暂定方法有效
        /// </summary>
        public void Continue()
        {
            _isPause = false;
        }

        /// <summary>
        /// 单帧执行动画,仅当执行过暂定方法有效
        /// </summary>
        public void runAnimationOneFrame()
        {
            if (_isPause)
            {
                animationLoop();
            }
        }

        private void animationLoop()
        {
            if (_delayTime > 0)
            {
                _delayTime -= Time.deltaTime;
                return;
            }

            _totalTime += Time.deltaTime;

            if (isTimeAnimation)
            {
                if (_animationTime <= 0)
                {
                    _animationTime = 0.01f;
                }
                _velocity = _spline.length / _animationTime;
            }

            float lastT = _T;
            if (!_reverse)
            {

                _T = _spline.GetProgressAtSpeedByTime(_T, _easeCurve.Evaluate(_T) * _velocity);

                if (_T >= 1f)
                {
                    if (_mode == SplineWalkerMode.Once)
                    {
                        _T = 1f;
                    }
                    else if (_mode == SplineWalkerMode.Loop)
                    {
                        _T -= 1f;
                    }
                    else
                    {
                        _T = 2f - _T;
                        _reverse = true;
                        _usedTime = 0;
                    }
                }

                if (_T > lastT)
                {
                    _usedTime += Time.deltaTime;
                    _progress = _usedTime * _velocity / _spline.length;
                }

                CheckKey();

            }
            else
            {
                _T = _spline.GetProgressAtSpeedByTime(_T, _easeCurve.Evaluate(_T) * _velocity, -1);

                if (_T <= 0f)
                {
                    if (_mode == SplineWalkerMode.Once)
                    {
                        _T = 0f;
                    }
                    else if (_mode == SplineWalkerMode.Loop)
                    {
                        _T += 1f;
                    }
                    else
                    {
                        _T = -_T;
                        _reverse = false;
                        _usedTime = 0;
                    }
                }

                if (_T < lastT)
                {
                    _usedTime += Time.deltaTime;
                    _progress = 1 - (_usedTime * _velocity / _spline.length);
                }

                CheckKey();
            }

            Vector3 position = _spline.GetPoint(_T);
            if (ApplyLineDirection)
            {
                Vector3 rc = transform.eulerAngles;
                Vector3 n = _spline.GetDirection(_T);
                Quaternion r = (_reverse ? Quaternion.LookRotation(-n) : Quaternion.LookRotation(n));
                //Quaternion r = Quaternion.LookRotation(-n);
                transform.rotation = r;
                if (_lockUPDirection)
                {
                    transform.eulerAngles = new Vector3(rc.x, r.eulerAngles.y, rc.z);
                }

                transform.position = position + (r * _offsetPos);
            }
            else
            {
                transform.position = position + _offsetPos;
            }
            if (!_reverse)
            {
                if (_mode == SplineWalkerMode.Once && _T >= 1f)
                {
                    Stop();
                }
            }
            else
            {
                if (_mode == SplineWalkerMode.Once && _T <= 0f)
                {
                    Stop();
                }
            }
            if (_runAnimTime > 0)
            {
                if (_totalTime >= _runAnimTime)
                {
                    Stop();
                }
            }

        }

        private void CheckKey()
        {
            if (_keyIndex < _keyList.Count)
            {
                int index = -1;
                if (!_reverse)
                {
                    for (int i = _keyIndex + 1; i < _keyList.Count; i++)
                    {
                        //if (_keyList[i] <= _T)
                        if (_keyList[i] <= _progress)
                        {
                            index = i;
                        }
                    }
                }
                else
                {
                    for (int i = _keyIndex - 1; i >= 0; i--)
                    {
                        //if (_keyList[i] >= _T)
                        if (_keyList[i] >= _progress)
                        {
                            index = i;
                        }
                    }
                }
                if (index >= 0)
                {
                    _keyIndex = index;
                    _delayTime = _infoList[_keyIndex].delayTime;
                    Debug.Log("index = " + index + " , _infoList = " + _infoList[_keyIndex]);
                    if (onSplineAnimKeyInfo != null)
                    {
                        onSplineAnimKeyInfo(this.gameObject, _infoList[_keyIndex]);
                    }
                }
            }
        }

        [System.Serializable]
        public class SplineAnimKeyInfo
        {
            /// <summary>
            /// 抛出动画的名称
            /// </summary>
            public string animationName;
            /// <summary>
            /// 目标方向(目前是个参考值)
            /// </summary>
            public Vector3 direction;
            /// <summary>
            /// 暂停动画的时间
            /// </summary>
            public float delayTime;
        }

    }  


}





