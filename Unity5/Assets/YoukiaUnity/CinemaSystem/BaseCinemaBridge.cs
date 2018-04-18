using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using UnityEngine;
//using YoukiaUnity.App;

namespace YoukiaUnity.CinemaSystem
{

    public struct HidingEffectItem
    {
        GameObject gameObject;
        int orginalLayer;
    }

    /// <summary>
    /// 电影剪辑系统的桥接类
    /// </summary>
    public class BaseCinemaBridge : MonoBehaviour, ICinemaBridge
    {

        public virtual void setupValues(string valuesStr)
        {
            //no nothing;
        }

        public virtual string ExportValuesString()
        {
            return null;
        }

        private Action onDestoryClip;

        private List<GameObject> _hidePrefabList;
        public void addHidePrefab(GameObject prefab)
        {
            if (_hidePrefabList == null) _hidePrefabList = new List<GameObject>();

            if (!_hidePrefabList.Contains(prefab))
            {
                _hidePrefabList.Add(prefab);
                prefab.SetActive(false);
            }
        }

        /// <summary>
        /// 动画中事件驱动 - 摄像机抖动
        /// </summary>
        /// <param name="parma">时间|力度[|频率 = 100]</param>
//        [CinemaBrigdgeInterface("摄像机抖动", "时间(秒)|力度(float)[|频率(float) = 100]")]
//        public void ShakeCamera(string parma)
//        {
//
//            string[] parmas = parma.Split('|');
//
//            float time = float.Parse(parmas[0]);
//            float strength = float.Parse(parmas[1]);
//
//            int vibrato = 100;
//            if (parmas.Length > 2)
//            {
//                vibrato = int.Parse(parmas[2]);
//            }
//
//            //            YKApplication.Instance.GetManager<GraphicsManager>().CameraShake(float.Parse(parmas[0]), float.Parse(parmas[0]));
//            if (GraphicsManager.GetInstance() != null)
//                GraphicsManager.GetInstance().CameraShake(time, strength, vibrato);
//
//        }

        /// <summary>
        /// 切换到临时场景
        /// 
        /// 注意：：该功能必须配合多State的Clip使用，且该事件必须在某个State的最后一帧；
        /// 
        /// </summary>
        [CinemaBrigdgeInterface("切换到临时场景", "场景名(string)")]
        public void LoadAndIntoTempScene(string parma)
        {
            CinemaClip.ChangeToTempScene(parma, _ToSetNextStatePlay);
        }

        private void _ToSetNextStatePlay()
        {
            Animator animtor = gameObject.GetComponent<Animator>();
            if (animtor)
            {
                int step = animtor.GetInteger("step") + 1;
                animtor.SetInteger("step", step);
            }
        }


        /// <summary>
        /// 切换到临时场景
        /// 
        /// 注意：：该功能必须配合多State的Clip使用，且该事件必须在某个State的最后一帧；
        /// 
        /// </summary>
        [CinemaBrigdgeInterface("退出临时场景", "无参数")]
        public void RemoveTempScene(string parma)
        {
            CinemaClip.RestoreCurrentScene(_ToSetNextStatePlay);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="parma"></param>
        [CinemaBrigdgeInterface("播放声音", "声音文件名(无后缀名，声音文件须放在指定文件夹内)[|是否循环播放(true/false)]")]
        public void PlaySound(string parma)
        {

            if (parma.Contains("|"))
            {

                string[] p = parma.Split('|');

                bool b = false;
                string pp = p[1].ToLower();
                if (pp == "true" || pp == "1" || pp == "t")
                {
                    b = true;
                }
                CinemaClip.PlayAudio(p[0], b);
            }
            else
            {
                CinemaClip.PlayAudio(parma, false);
            }
        }

        [CinemaBrigdgeInterface("停止播放声音", "声音文件名(无后缀名，声音文件须放在指定文件夹内）")]
        public void StopSound(string parma)
        {
            CinemaClip.StopAudio(parma);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="parma"></param>
        [CinemaBrigdgeInterface("播放背景音乐", "声音文件名(无后缀名，声音文件须放在指定文件夹内)")]
        public void PlayBGM(string parma)
        {
            CinemaClip.PlayBGM(parma);
        }

        /// <summary>
        /// 淡入当前摄像机(从暗淡入)
        /// </summary>
        /// <param name="parma"></param>
        public virtual void FadeInClip(string parma)
        {
            //
        }

        /// <summary>
        /// 淡入当前摄像机(从亮淡入)
        /// </summary>
        /// <param name="parma"></param>
        public virtual void FadeInClipLight(string parma)
        {
            //
        }

        /// <summary>
        /// 淡出当前摄像机(淡出变暗)
        /// </summary>
        /// <param name="parma"></param>
        [CinemaBrigdgeInterface("淡出当前摄像机(淡出变暗)", "淡出时间(秒)")]
        public virtual void FadeOutClip(string parma)
        {
            //
        }

        /// <summary>
        /// 淡出当前摄像机(变亮)
        /// </summary>
        /// <param name="parma"></param>
        [CinemaBrigdgeInterface("淡出当前摄像机(淡出变亮)", "淡出时间(秒)")]
        public virtual void FadeOutClipLight(string parma)
        {
            //
        }

        /// <summary>
        /// 马上销毁电影剪辑
        /// </summary>
//        [CinemaBrigdgeInterface("销毁剪辑", "")]
        public virtual void DestoryClip(string parma)
        {
            ICinemaBridge icb = gameObject.GetInterface<ICinemaBridge>();
            if (icb != null)
            {
                icb.OnClipEnd();
            }

            //重置Clip
            CinemaClip.resetClip(gameObject.GetComponent<CinemaClip>());

            CinemaClip.UnloadPrefab(gameObject);
        }

        /// <summary>
        /// 动画中事件驱动 - 创建特效 
        /// </summary>
        /// <param name="parma"> 效果ID|演员名字|身体位置 </param>
        [CinemaBrigdgeInterface("创建特效", "效果ID(string)|演员名字(string)|身体位置")]
        public void CreateEffect(string parma)
        {

            string[] parmas = parma.Split('|');
            CinemaCharacter actor = GetComponent<CinemaClip>().GetActorByName(parmas[1]);

            if (actor == null)
                return;

            //            YKApplication.Instance.GetManager<ResourcesManager>().PoolMg.LoadObjectFromPool(parmas[0], (obj) =>
            //            {
            //
            //                (obj as ResourceRefKeeper).transform.parent = actor.transform;
            //                (obj as ResourceRefKeeper).transform.localPosition = Vector3.zero;
            //                (obj as ResourceRefKeeper).transform.localEulerAngles = Vector3.zero;
            //            });

            //loadObj
            //            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(parmas[0])) as GameObject;
//            CinemaClip.LoadPrefab(parmas[0], (obj) =>
//            {
//                obj.transform.SetLayer(1 << LayerMask.NameToLayer(GraphicsManager.ClipPostEffectLayerDefine));
//            }, 1);
        }

        [CinemaBrigdgeInterface("隐藏角色", "角色名(string)|...")]
        public virtual void HideActor(string parma)
        {
            CinemaClip clip = transform.GetComponent<CinemaClip>();
            if (clip == null) return;

            string[] parmas = parma.Split('|');
            if (parmas.Length == 0) return;

            int i, len = parmas.Length;
            for (i = 0; i < len; i ++)
            {
                string actorName = parmas[i];

                CinemaCharacter character = clip.GetActorByName(actorName);
                if (character == null) continue;

                character.transform.SetLayer(LayerMask.NameToLayer("hide"));

            }

        }

        [CinemaBrigdgeInterface("恢复隐藏角色", "角色名(string)|...")]
        public virtual void ShowActor(string parma)
        {
            CinemaClip clip = transform.GetComponent<CinemaClip>();
            if (clip == null) return;

            string[] parmas = parma.Split('|');
            if (parmas.Length == 0) return;

            int i, len = parmas.Length;
            for (i = 0; i < len; i ++)
            {
                string actorName = parmas[i];

                CinemaCharacter character = clip.GetActorByName(actorName);
                if (character == null) continue;

                character.transform.SetLayer(LayerMask.NameToLayer("Role"));

            }

        }

//        [CinemaBrigdgeInterface("石化效果", "角色名(string)|开关(bool)|...")]
//        public virtual void BeFossilised(string parma)
//        {
//
//            CinemaClip clip = transform.GetComponent<CinemaClip>();
//            if (clip == null) return;
//
//            string[] parmas = parma.Split('|');
//            if (parmas.Length == 0) return;
//
//            int i, len = parmas.Length;
//            for (i = 0; i < len; i += 2)
//            {
//                string actorName = parmas[i];
//                bool isEnabled = parmas[i + 1].ToLower() == "true" ? true : false;
//
//                CinemaCharacter character = clip.GetActorByName(actorName);
//                if (character == null) continue;
//
//               // PivotPointData ppd = character.GetComponentInChildren<PivotPointData>();
//                PivotPointData ppd = character.CinemaActor.View.gameObject.GetComponentInChildren<PivotPointData>();
//                if (ppd == null) continue;
//
//                RoleShaderHandler rsh = ppd.gameObject.GetComponent<RoleShaderHandler>();
//				if (rsh == null) {
//					rsh = ppd.gameObject.AddComponent<RoleShaderHandler> ();
//				}
//
//				if (rsh.RockTexture == null) {
//                    
//                    CinemaClip.LoadTexture("Texture/FightImg/RockStateTex", (tex) =>
//                    {
//                        rsh.SetRockTexture(tex as Texture2D);
//                        rsh.enabled = isEnabled;
//                    });
//                } else {
//					rsh.enabled = isEnabled;
//				}
//            }
//        }

        /// <summary>
        /// 改变角色表情
        /// </summary>
        /// <param name="parma">角色名|表情名|...</param>
//        [CinemaBrigdgeInterface("改变角色表情", "角色名(string)|表情名(string)|...")]
//        public virtual void ChangeActorEmoji(string parma)
//        {
//
//            CinemaClip clip = transform.GetComponent<CinemaClip>();
//            if (clip == null) return;
//
//            string[] parmas = parma.Split('|');
//            if (parmas.Length == 0) return;
//
//            int i, len = parmas.Length;
//            for (i = 0; i < len; i += 2)
//            {
//                string actorName = parmas[i];
//                string faceName = parmas[i + 1];
//
//                CinemaCharacter character = clip.GetActorByName(actorName);
//                if (character == null) continue;
//
//                RoleFaceCollection data = character.transform.GetComponentInChildren<RoleFaceCollection>();
//                if (data == null) continue;
//
//                data.SetRoleFace(faceName);
//
//            }
//
//        }

        /// <summary>
        /// 恢复角色默认表情
        /// </summary>
        /// <param name="parma">角色名|...</param>
//        [CinemaBrigdgeInterface("恢复角色默认表情", "角色名(string)|...")]
//        public virtual void ResetActorEmoji(string parma)
//        {
//            CinemaClip clip = transform.GetComponent<CinemaClip>();
//            if (clip == null) return;
//
//            string[] parmas = parma.Split('|');
//            if (parmas.Length == 0) return;
//
//            int i, len = parmas.Length;
//            for (i = 0; i < len; i++)
//            {
//                string actorName = parmas[i];
//
//                CinemaCharacter character = clip.GetActorByName(actorName);
//                if (character == null) continue;
//
//                RoleFaceCollection data = character.transform.GetComponentInChildren<RoleFaceCollection>();
//                if (data == null) continue;
//
//                data.ResetRoleFace();
//
//            }
//
//        }

        /// <summary>
        /// 小黑屋模式
        /// 格式 强度|时间[|渐变时间]
        /// (有BUG,后续可能移除该方法)
        /// </summary>
        /// <param name="str"></param>
//        [Obsolete]
//        public void DarkOcclusion(string str)
//        {
//            if (string.IsNullOrEmpty(str)) return;
//            if (GraphicsManager.isInited)
//            {
//
//                string[] parms = str.Split('|');
//
//                float lum = float.Parse(parms[0]);
//                float time = float.Parse(parms[1]);
//                float fade = 0.2f;
//                if (parms.Length > 2)
//                {
//                    fade = float.Parse(parms[2]);
//                }
//
//                GraphicsManager.GetInstance().DarkOcclusion(lum, time,1, "Role", fade);
//            }
//        }

        public virtual void OnClipStart()
        {
            //  throw new System.NotImplementedException();
        }

        public virtual void OnClipPlay()
        {
            //   Debug.UnityLoggerUtilityLog("xxxxx");
            //  throw new System.NotImplementedException();
        }



        /// <summary>
        /// 剪辑播放完成后的回调
        /// </summary>
        /// <param name="onDestoryClip"></param>
        public void SetFinishCallBack(Action onDestoryClip = null)
        {
            this.onDestoryClip = onDestoryClip;
        }

        public virtual void OnClipEnd()
        {

            if (CinemaClip.UsedTempScene)
            {
                CinemaClip.RestoreCurrentScene(null);
            }

            if (_hidePrefabList != null && _hidePrefabList.Count > 0)
            {
                int i, len = _hidePrefabList.Count;
                for (i = 0; i < len; i++)
                {
                    _hidePrefabList[i].SetActive(true);
                }
                _hidePrefabList.Clear();
            }

            if (onDestoryClip != null)
            {
                Action onDestoryClipDo = onDestoryClip;
                onDestoryClip = null;
                onDestoryClipDo();
            }

        }

        public virtual IGameAnimObject LoadPlayer(string playerID)
        {
            return null;
        }

        /// <summary>
        /// 初始化角色
        /// </summary>
        /// <param name="character">角色脚本</param>
        /// <param name="finish">初始化后的回调</param>
        public virtual void InitActor(CinemaCharacter character, Action finish)
        {
            BaseCinemaActor view = new BaseCinemaActor();
            character.CinemaActor = view;
            view.Initialization(character, this, finish);
        }

        [CinemaBrigdgeInterface("自定义Cinema事件", "自定义事件格式")]
        public virtual void CallCinemaEvent(string param)
        {

        }

        /// <summary>
        /// 检查龙套
        /// 
        /// 该函数提供 在CinemaCharacter启用运行时关联如果其节点下挂有龙套角色时，CinemaCharacter的挂载角色逻辑。
        /// 
        /// </summary>
        public virtual IGameAnimObject CheckLongTao(CinemaCharacter character)
        {

            if (character.transform.childCount == 0 || character.LoadedGameObject) return null;
            //

            int i, len = character.transform.childCount;
            for (i = 0; i < len; i++)
            {
                CinemaLongtao longtao = character.transform.GetChild(i).GetComponent<CinemaLongtao>();
                if (longtao)
                {

                    if (longtao.IsDestroyed) return null;

                    character.CreateBySelf = true;
                    SimpleCinemaView view = SimpleCinemaView.Create(longtao.gameObject);
                    view.gameObject.SetActive(true);
                    view.gameObject.transform.parent = character.transform;
                    view.gameObject.transform.localPosition = Vector3.zero;
                    view.gameObject.transform.localEulerAngles = Vector3.zero;

                    character.LoadedGameObject = view.gameObject;

                    return view;
                }
            }

            return null;

        }


    }

    public class CinemaBrigdgeInterfaceAttribute : Attribute
    {
        public readonly string info;
        public readonly string parmInfo;

        public CinemaBrigdgeInterfaceAttribute(string info, string parmInfo)
        {
            this.info = info;
            this.parmInfo = parmInfo;
        }
    }

}



