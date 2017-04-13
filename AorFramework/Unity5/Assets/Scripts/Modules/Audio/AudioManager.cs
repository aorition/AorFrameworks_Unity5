using UnityEngine;
using System.Collections.Generic;
using YoukiaUnity.Misc;
using YoukiaUnity.Resource;

namespace AorFramework.module
{
    public class AudioManager : SingletonManager<AudioManager>
    {
        /// <summary>
        /// 最大混音数量
        /// </summary>
        public static int MixSoundNum = 5;

        /// <summary>
        /// 背景音乐播放源
        /// </summary>
        private AudioSource MusicSource;
        /// <summary>
        /// 背景音乐播放源_2
        /// </summary>
        private AudioSource MusicSource_2;

        /// <summary>
        /// 其他声源
        /// </summary>
        private Queue<AudioSource> SoundSourceQueue;


        /// <summary>
        /// 数据缓存保持
        /// </summary>
        private List<ResourcesManager.ResourceRef> CacheSound = new List<ResourcesManager.ResourceRef>();

        /// <summary>
        /// 当前的音乐数据保持
        /// </summary>
        private ResourcesManager.ResourceRef musicNow;
        private ResourcesManager.ResourceRef musicNow_2;

        private float _AudioVolume = 1;
        private float _LastAudioVolume = 1;

        /// <summary>
        /// 音效音量
        /// </summary>
        public float AudioVolume {

            get { return _AudioVolume; }
            set {
                _LastAudioVolume = _AudioVolume;
                _AudioVolume = value;
                AudioSource[] scs = SoundSourceQueue.ToArray();
                for (int i = 0; i < scs.Length; i++)
                {
                    scs[i].volume = _AudioVolume;
                }
            }
        }

        private bool _AudioMute;
        /// <summary>
        /// 音效静音
        /// </summary>
        public bool AudioMute {

            get { return _AudioMute; }
            set {
                _AudioMute = value;
                AudioSource[] scs = SoundSourceQueue.ToArray();
                for (int i = 0; i < scs.Length; i++)
                {
                    scs[i].mute = value;
                }
            }
        }

        private float _MusicVolume = 1;


        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float MusicVolume {

            get { return _MusicVolume; }
            set {

                _MusicVolume = value;
                MusicSource.volume = _MusicVolume;
                MusicSource_2.volume = _MusicVolume;
            }

        }

        //停止播放所有音效
        public void StopAllSound()
        {

            foreach (var sound in SoundSourceQueue)
            {
                if (sound.isPlaying)
                    sound.Stop();
            }
        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            SoundSourceQueue = new Queue<AudioSource>();
            gameObject.AddComponent<AudioListener>();
            //混音器数量
            for (int i = 0; i < MixSoundNum; i++)
            {
                AudioSource sc = gameObject.AddComponent<AudioSource>();
                sc.volume = _AudioVolume;
                sc.playOnAwake = false;
                SoundSourceQueue.Enqueue(sc);
            }

            MusicSource = gameObject.AddComponent<AudioSource>();
            MusicSource.volume = _MusicVolume;
            MusicSource.loop = true;
            MusicSource.playOnAwake = false;

            MusicSource_2 = gameObject.AddComponent<AudioSource>();
            MusicSource_2.volume = _MusicVolume;
            MusicSource_2.loop = true;
            MusicSource_2.playOnAwake = false;
        }

        /// <summary>
        /// audio是否已经缓冲
        /// </summary>
        private AudioClip isCached(string path)
        {
            for (int i = 0; i < CacheSound.Count; i++)
            {
                if (CacheSound[i].resUnit.mPath == path)
                    return CacheSound[i].Asset as AudioClip;
            }

            return null;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="clip">指定的剪辑</param>
        public void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                AudioSourcePlay(clip);
            }

        }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path">指定的路径</param>
        public void PlaySound(string path)
        {
            AudioClip clip = isCached(path);
            if (clip != null)
            {
                AudioSourcePlay(clip);
            }
            else
            {
                //YKApplication 缺失
                //
                //                YKApplication.Instance.GetManager<ResourcesManager>().LoadObject(path, (obj) =>
                //                {
                //                    ResourcesManager.ResourceRef refObj = obj as ResourcesManager.ResourceRef;
                //
                //                    clip = refObj.Asset as AudioClip;
                //                    AudioSourcePlay(clip);
                //
                //                    if (!CacheSound.Contains(refObj))
                //                    {
                //
                //                        if (CacheSound.Count > 20)
                //                        {
                //                            //移除最老的
                //                            CacheSound.RemoveAt(0);
                //                        }
                //                        CacheSound.Add(refObj);
                //                    }
                //                });
            }
        }

        private void AudioSourcePlay(AudioClip clip)
        {
            AudioSource sc = SoundSourceQueue.Dequeue();
            sc.clip = clip;
            sc.mute = _AudioMute;
            sc.volume = _AudioVolume;
            sc.Play();
            SoundSourceQueue.Enqueue(sc);
        }


        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="path">指定路径</param>
        public void PlayMusic(string path)
        {
            if (musicNow != null && musicNow.resUnit.mPath == path)
            {
                if (MusicSource.isPlaying == false)
                {
                    MusicSource.Play();
                }
                return;
            }
            //YKApplication 缺失
            //            YKApplication.Instance.GetManager<ResourcesManager>().LoadObject(path, (obj) =>
            //            {
            //                musicNow = obj as ResourcesManager.ResourceRef;
            //                AudioClip clip = musicNow.Asset as AudioClip;
            //                MusicSource.clip = clip;
            //                MusicSource.Play();
            //            });

        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopMusic()
        {
            if (MusicSource.isPlaying)
                MusicSource.Stop();
        }
        public void PauseMusic()
        {
            if (MusicSource.isPlaying)
                MusicSource.Pause();
        }
        public string GetCurrentMusicPath()
        {
            if (musicNow != null)
            {
                return musicNow.resUnit.mPath;
            }
            return "";
        }
        #region 背景音乐2
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="path">指定路径</param>
        public void PlayMusic_2(string path)
        {
            if (musicNow_2 != null && musicNow_2.resUnit.mPath == path)
            {
                if (MusicSource_2.isPlaying == false)
                {
                    MusicSource_2.Play();
                }
                return;
            }

            //YKApplication 缺失
            //            YKApplication.Instance.GetManager<ResourcesManager>().LoadObject(path, (obj) =>
            //            {
            //                musicNow_2 = obj as ResourcesManager.ResourceRef;
            //                AudioClip clip = musicNow_2.Asset as AudioClip;
            //                MusicSource_2.clip = clip;
            //                MusicSource_2.Play();
            //            });

        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopMusic_2()
        {
            if (MusicSource_2.isPlaying)
                MusicSource_2.Stop();
        }
        public void PauseMusic_2()
        {
            if (MusicSource_2.isPlaying)
                MusicSource_2.Pause();
        }
        public string GetCurrentMusic_2Path()
        {
            if (musicNow_2 != null)
            {
                return musicNow_2.resUnit.mPath;
            }
            return "";
        }
        #endregion
    }
}
