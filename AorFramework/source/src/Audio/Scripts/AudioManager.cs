using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Audio
{

    /// <summary>
    /// 基础多通道AudioManger
    /// </summary>
    public class AudioManager : ManagerBase
    {

        private static string _NameDefine = "AudioManager";

        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get { return _instance; }
        }

        public static bool HasInstance
        {
            get { return _instance != null; }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static AudioManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<AudioManager>(ref _instance, _NameDefine, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static AudioManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<AudioManager>(ref _instance, target);
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance();
            ManagerBase.Request(ref _instance, GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return ManagerBase.VerifyIsInit(ref _instance);
        }

        //-----------------------------------------------------------------

        public class AudioClipKeeper
        {

            public AudioClipKeeper(string loadPath, AudioClip clip, float Survivalseconds)
            {
                this.LoadPath = loadPath;
                this.clip = clip;
                this.SurvivalSeconds = Survivalseconds;
            }

            public string LoadPath;
            public AudioClip clip;
            public float SurvivalSeconds;

            private float _liveSeconds;

            public bool IsPlaying = false;

            private bool m_IsDead = false;
            public bool IsDead
            {
                get { return m_IsDead; }
            }

            public void Update(float deltaTime)
            {
                if (SurvivalSeconds == 0) return;

                if (IsPlaying)
                {
                    _liveSeconds = 0;
                    return;
                }

                _liveSeconds += deltaTime;
                if (_liveSeconds >= SurvivalSeconds)
                {
                    m_IsDead = true;
                }
            }

        }

        /// <summary>
        /// 音效(非循环声音)通道数限制
        /// </summary>
        public int ACChannelLimit = 16;
        /// <summary>
        /// 背景音乐(循环音效)通道限制
        /// </summary>
        public int BGMChannelLimit = 2;

        /// <summary>
        /// 缓存Clip最大条数;
        /// </summary>
        public int AudioClipCacheLimit = 12;

        /// <summary>
        /// 缓存Clip 未被使用时将在多少秒后被清出缓存池?
        /// </summary>
        public float AudioClipCacheSurvivalSeconds = 30f;

        /// <summary>
        /// 是否静音
        /// </summary>
        private bool m_mute = false;
        public bool Mute
        {
            get { return m_muteAC && m_muteBGM; }
            set
            {
                MuteAC = m_muteBGM = value;
            }
        }

        private bool m_muteAC = false;
        public bool MuteAC
        {
            get { return m_muteAC; }
            set
            {
                m_muteAC = value;
                _setMuteAC();
            }
        }

        private bool m_muteBGM = false;
        public bool MuteBGM
        {
            get { return m_muteBGM; }
            set
            {
                m_muteBGM = value;
                _setMuteBGM();
            }
        }

        private List<AudioSource> _ACChannels = new List<AudioSource>();
        private List<AudioSource> _BGMChannels = new List<AudioSource>();

        private List<AudioClipKeeper> _audioClipList = new List<AudioClipKeeper>();
        //private Dictionary<string, AudioClipKeeper> _audioClipDic = new Dictionary<string, AudioClipKeeper>();

        protected override void Awake()
        {

            AudioBridge.PlayHook = Play;
            AudioBridge.PlayClipHook = PlayClip;

            ManagerBase.VerifyUniqueOnInit<AudioManager>(ref _instance, this, null);

            //此Manager不需要Setup数据或者Init初始化
            _isSetuped = true;
            _isInit = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        /// <summary>
        /// 使用一个AudioSource来处理所有音效的方式
        /// </summary>
        private AudioSource _audioSingleSource;
        public AudioSource AudioSingleSource {
            get {
                if (!_audioSingleSource) {
                    _audioSingleSource = gameObject.AddComponent<AudioSource>();
                }
                return _audioSingleSource;
            }
        }

        public void PlayOneShot(string path)
        {

            AudioClipKeeper kepper = _audioClipList.Find(k => k.LoadPath == path);
            if (kepper != null)
            {
                kepper.IsPlaying = true;
                PlayOneShot(kepper.clip);
            }
            else
            {
                if (_audioClipList.Count < AudioClipCacheLimit)
                {
                    ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
                    {
                        if (clip)
                        {
                            kepper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
                            _audioClipList.Add(kepper);
                            PlayOneShot(kepper.clip);
                        }

                    });
                }
            }

        }

        public void PlayOneShot(AudioClip clip)
        {
            AudioSingleSource.PlayOneShot(clip);
        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        public void Play(string path, params object[] parms)
        {

            AudioClipKeeper kepper = _audioClipList.Find(k => k.LoadPath == path);
            if (kepper != null)
            {
                kepper.IsPlaying = true;
                PlayClip(kepper.clip, parms);
            }
            else
            {
                if (_audioClipList.Count < AudioClipCacheLimit)
                {
                    ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
                    {
                        if (clip)
                        {
                            kepper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
                            _audioClipList.Add(kepper);
                            PlayClip(clip, parms);
                        }

                    });
                }
            }

        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        public void PlayClip(AudioClip clip, params object[] parms)
        {
            AudioSource audioSource = _getEmptyAudioSource();
            if (audioSource)
            {
                audioSource.clip = clip;
                _applyParmsForAudioSource(audioSource, parms);
                audioSource.Play();
            }
        }


        public void Stop(string path, params object[] parms)
        {
            AudioSource audioSource = GetAudioSourceByPath(path);
            if (audioSource && audioSource.clip)
            {
                AudioClipKeeper keeper = _audioClipList.Find(k => k.clip == audioSource.clip);
                if (keeper != null)
                {
                    keeper.IsPlaying = false;
                }

                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        public void StopAllAC()
        {

            for (int i = 0; i < _ACChannels.Count; i++)
            {
                if (_ACChannels[i].clip && _ACChannels[i].isPlaying)
                {
                    AudioClipKeeper keeper = _audioClipList.Find(k => k.clip == _ACChannels[i].clip);
                    if (keeper != null)
                    {
                        keeper.IsPlaying = false;
                    }
                    _ACChannels[i].Stop();
                    _ACChannels[i].clip = null;
                }
            }

        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        public void PlayBGM(string path, params object[] parms)
        {
            AudioClipKeeper keeper = _audioClipList.Find(k => k.LoadPath == path);
            if (keeper != null)
            {
                AudioSource audioSource = _getEmptyAudioSource(true);
                if (audioSource)
                {
                    keeper.IsPlaying = true;
                    audioSource.clip = keeper.clip;
                    audioSource.loop = true;
                    _applyParmsForAudioSource(audioSource, parms);
                    audioSource.Play();
                }
            }
            else
            {
                ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
                {

                    if (clip)
                    {
                        keeper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
                        _audioClipList.Add(keeper);
                        AudioSource audioSource = _getEmptyAudioSource(true);
                        if (audioSource)
                        {
                            keeper.IsPlaying = true;
                            audioSource.clip = clip;
                            audioSource.loop = true;
                            _applyParmsForAudioSource(audioSource, parms);
                            audioSource.Play();
                        }
                    }

                });
            }
        }


        public void StopBGM(string path, params object[] parms)
        {
            AudioSource audioSource = GetAudioSourceByPath(path, true);
            if (audioSource && audioSource.clip)
            {
                AudioClipKeeper keeper = _audioClipList.Find(k => k.clip == audioSource.clip);
                if (keeper != null)
                {
                    keeper.IsPlaying = false;
                }
                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        public void StopAllBGM()
        {
            for (int i = 0; i < _BGMChannels.Count; i++)
            {
                if (_BGMChannels[i].clip && _BGMChannels[i].isPlaying)
                {
                    AudioClipKeeper keeper = _audioClipList.Find(k => k.clip == _BGMChannels[i].clip);
                    if (keeper != null)
                    {
                        keeper.IsPlaying = false;
                    }
                    _BGMChannels[i].Stop();
                    _BGMChannels[i].clip = null;
                }
            }
        }

        public void StopAll()
        {
            StopAllAC();
            StopAllBGM();
        }

        public bool IsBGMPlaying(string path)
        {
            return GetAudioSourceByPath(path);
        }

        public bool IsAcPlaying(string path)
        {
            return GetAudioSourceByPath(path, true);
        }

        public AudioSource GetAudioSourceByPath(string path, bool isLoop = false)
        {
            AudioClipKeeper keeper = _audioClipList.Find(k => k.LoadPath == path);
            if (keeper != null)
            {
                return isLoop ? _BGMChannels.Find(c => c.clip == keeper.clip) : _ACChannels.Find(c => c.clip == keeper.clip);
            }
            return null;
        }

        //-------------------------------------------------

        private void _setMuteAC()
        {
            for (int i = 0; i < _ACChannels.Count; i++)
            {
                _ACChannels[i].mute = m_muteAC;
            }
        }

        private void _setMuteBGM()
        {
            for (int i = 0; i < _BGMChannels.Count; i++)
            {
                _BGMChannels[i].mute = m_muteBGM;
            }
        }

        private float m_lastRTime;
        private int u, uLen;
        private List<AudioClipKeeper> _dels = new List<AudioClipKeeper>();
        private void Update()
        {
            uLen = _ACChannels.Count;
            for (u = 0; u < uLen; u++)
            {
                if (_ACChannels[u].clip && _ACChannels[u].isActiveAndEnabled && !_ACChannels[u].isPlaying)
                {
                    AudioClipKeeper keeper = _audioClipList.Find(k => k.clip == _ACChannels[u].clip);
                    if (keeper != null)
                    {
                        keeper.IsPlaying = false;
                    }
                    _ACChannels[u].clip = null;
                }
            }

            uLen = _audioClipList.Count;
            for (u = 0; u < uLen; u++)
            {
                _audioClipList[u].Update(Time.realtimeSinceStartup - m_lastRTime);
                if (_audioClipList[u].IsDead)
                {
                    _dels.Add(_audioClipList[u]);
                }
            }

            uLen = _dels.Count;
            if (uLen > 0)
            {
                for (u = 0; u < uLen; u++)
                {
                    _audioClipList.Remove(_dels[u]);
                }
                _dels.Clear();
            }

            m_lastRTime = Time.realtimeSinceStartup;
        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        private void _applyParmsForAudioSource(AudioSource source, params object[] parms)
        {

            //reset values
            source.volume = 1;
            source.panStereo = 0;
            source.pitch = 1;
            source.spatialBlend = 0;
            source.reverbZoneMix = 1;

            if (parms == null || parms.Length == 0)
            {
                return;
            }

            float volume = (float)parms[0];
            source.volume = volume;

            if (parms.Length > 1)
            {
                float StereoPan = (float)parms[1];
                source.panStereo = StereoPan;
            }
            if (parms.Length > 2)
            {
                float pitch = (float)parms[2];
                source.pitch = pitch;
            }
            if (parms.Length > 3)
            {
                float SpatialBlend = (float)parms[2];
                source.spatialBlend = SpatialBlend;
            }
            if (parms.Length > 4)
            {
                float ReverbZoneMix = (float)parms[3];
                source.reverbZoneMix = ReverbZoneMix;
            }
        }

        private List<AudioSource> _tmpChannels;
        private int _tmpChannelLimit;
        private AudioSource _getEmptyAudioSource(bool isBGM = false)
        {

            _tmpChannels = isBGM ? _BGMChannels : _ACChannels;
            _tmpChannelLimit = isBGM ? BGMChannelLimit : ACChannelLimit;
            AudioSource aos = null;
            for (int i = 0; i < _tmpChannels.Count; i++)
            {
                if (_tmpChannels[i].clip == null)
                {
                    aos = _tmpChannels[i];
                }
            }
            if (!aos && _tmpChannels.Count < _tmpChannelLimit)
            {
                aos = gameObject.AddComponent<AudioSource>();
                aos.playOnAwake = false;
                aos.mute = (isBGM ? m_muteBGM : m_muteAC);
                _tmpChannels.Add(aos);
            }

            return aos;
        }



    }
}