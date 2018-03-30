using System;
using System.Collections.Generic;
using System.Xml;
using AorFramework;
using UnityEngine;


namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// 电影系统角色类
    /// 演员:该类运行时加载的人物
    /// 角色:制作时的点位
    /// </summary>
    public class CinemaCharacter : MonoBehaviour ,IAnimLinkageCharacter, ICinemaCharacterGizmo
    {

        [NonSerialized]
        public Animator LoadedAnimator;

        [NonSerialized]
        public GameObject LoadedGameObject;
        

        /// <summary>
        /// 使用运行时预先关联角色模式
        /// 
        /// 此模式作用为角色是指定的但是需要从运行时环境动态绑定时就需要启用此模式
        /// 
        /// 此模式使用指定角色的角色动作表而非公共动作配置表
        /// 
        /// </summary>
        [SerializeField]//[HideInInspector]
        private bool _UseActorIDPreBinding = false;
        public bool UseActorIDPreBinding
        {
            get { return _UseActorIDPreBinding; }
        }

        /// <summary>
        /// 记录预关联角色模式下关联的角色预制体路径（主要用于查错）
        /// </summary>
        [SerializeField]//[HideInInspector]
        private string _ActorLoadPath;
        public string ActorLoadPath
        {
            get { return _ActorLoadPath; }
        }

        [NonSerialized]
        public string name = "NoInstall";

        /// <summary>
        /// 忽略角色动作
        /// </summary>
        public bool IgnoreActorBehavior = false;

        /// <summary>
        /// 隐藏当前角色
        /// </summary>
        public bool HideRole = false;

        /// <summary>
        /// 角色动画播放速度倍率
        /// </summary>
        public float Speed = 1f;

        /// <summary>
        /// 行为id
        /// </summary>
        //       [NonSerialized]
        //        [KeyAbleVariable]
        public float BehaviorID = 0;

        /// <summary>
        /// 运行时关联的演员ID
        /// </summary>
        public string ActorID;

        /// <summary>
        /// 角色缩放值
        /// </summary>
        public float ActorScale = 1f;

        /// <summary>
        /// 隐藏绑定的角色
        /// </summary>
        [SerializeField]//[HideInInspector]
        private bool _HideBindingActorPrefab = false;
        public bool HideBindingActorPrefab
        {
            get { return _HideBindingActorPrefab; }
        }

        public string CurrentbehavName
        {
            get { return BehaviorIDToBehaviorName(); }
        }

        /// <summary>
        /// 自加载的演员会parent到Actor<角色>下面，非自加载的人物保持它本身树节点关系不边，update中更新世界坐标位置信息
        /// </summary>
        [NonSerialized]
        public bool CreateBySelf = false;

        [NonSerialized]
        public ICinemaActor CinemaActor;
        
        /// <summary>
        /// 角色是否自动创建ModelShadowProjector组件
        /// </summary>
        public bool UseModelShadowProjector = false;

        /// <summary>
        /// 关联人物表演前的坐标
        /// </summary>
        private Vector3 orgPos;
        /// <summary>
        /// 关联人物表演前的旋转
        /// </summary>
        private Vector3 orgRot;
        
        /// <summary>
        /// 当前行为
        /// </summary>
        public string Currentbehavior;

        /// <summary>
        /// (actor)角色跟随演员,关闭的则为演员跟随(actor)角色
        /// </summary>
        public bool FollowPrefab = false;

        public void LinkSubAnimator()
        {
            LoadedAnimator = null;
            if (LoadedGameObject)
            {
                LoadedAnimator = LoadedGameObject.GetComponentInChildren<Animator>();
            }
        }

        public void Reset()
        {
            name = gameObject.name;
            CinemaActor = null;
            LoadedAnimator = null;
            LoadedGameObject = null;
            CreateBySelf = false;
        }

        /// <summary>
        /// 输出当前BehaviorID对应的BehaviorName, 如BehaviorID找不到对应的BehaviorName则返回Null
        /// </summary>
        public string BehaviorIDToBehaviorName()
        {

            if (_UseActorIDPreBinding || !string.IsNullOrEmpty(_ActorLoadPath))
            {
                //预绑定角色 =>
                if (_BehaviorNameList != null)
                {
                    int id = (int)BehaviorID;
                    if (id >= 0 && id < _BehaviorNameList.Length)
                    {
                        return _BehaviorNameList[id];
                    }
                }
            }
            else
            {
                //运行时绑定角色 =>
                return CinemaRTBActorBehavior.BehaviorIDToBehaviorName(BehaviorID);
            }
            return null;
        }

        [SerializeField]
//        [HideInInspector]
        private string[] _BehaviorNameList;

        //        protected override void OnUpdate()
        private void Update()
        {
            //            base.OnUpdate();
            if (CinemaActor != null)
            {
                CinemaActor.OnUpdate(this);
            }

            if (LoadedGameObject)
            {
                LoadedGameObject.transform.localScale = new Vector3(ActorScale, ActorScale, ActorScale);
            }

            if (LoadedAnimator)
            {
                LoadedAnimator.speed = Speed;
            }

        }

    }

    /// <summary>
    /// CinemaRuntimeBindingActorBehavior
    /// 
    /// *** 注意： 
    /// 
    /// 运行时需要使用配置表作为公共角色动作定义表，
    /// 则请在运行时设置CinemaRTBActorBehavior.UseAnimationClipConfig = true, 
    /// 并填充CinemaRTBActorBehavior.AnimationClipConfigDic；
    /// 
    /// </summary>
    public class CinemaRTBActorBehavior
    {

        public static bool UseAnimationClipConfig = false;
        
        public static Dictionary<string, int> AnimationClipConfigDic;

        /// <summary>
        /// 公共角色动作定义表
        /// </summary>
        enum RTBActorBehaviorEnum
        {
            idle = 0,
            show = 2, //show1,
            die = 4, //die1,
            hurt = 10, hurt1, //hurt2, hurt3, hurt4,
            hurtfly = 15, hurtfly1, //hurtfly2, hurtfly3, hurtfly4,
            move = 20, move1, move2, //move3, move4,
            rush = 25, //rush1, rush2, rush3, rush4,
            getup = 30, //getup1, getup2, getup3, getup4,
            //---- 预留 ------
            attack = 50, attack1, attack2, attack3, attack4, attack5, //attack6, attack7, attack8, attack9,
            stand = 60, stand1, stand2,// stand3, stand4,
            stun = 65, //stun1, stun2, stun3, stun4
            skill = 70, skill1, skill2, skill3, skill4, skill5, //skill6, skill7, skill8, skill9, 
        }

        public static List<string> ExportAllRTBActorBehaviorNames()
        {
            List<string> list = new List<string>();
            Dictionary<int,string> dic = new Dictionary<int, string>();
            string[] allExs = Enum.GetNames(typeof (RTBActorBehaviorEnum));
            int MaxId = -1;
            int i, len = allExs.Length;
            for (i = 0; i < len; i++)
            {
                string name = allExs[i];
                int idx = (int) Enum.Parse(typeof (RTBActorBehaviorEnum), name);
                dic.Add(idx, name);
                if (idx > MaxId)
                {
                    MaxId = idx;
                }
            }

            len = MaxId + 1;
            for (i = 0; i < len; i++)
            {
                if (dic.ContainsKey(i))
                {
                    list.Add(dic[i]);
                }
                else
                {
                    list.Add(null);
                }
            }

            return list;
        }

        /// <summary>
        /// 根据BehaviorID返回公共角色动作定义表中所对应的动作名
        /// 
        /// 注意：如果该BehaviorID未在公共角色动作定义表中定义，则返回 Null
        /// 
        /// </summary>
        public static string BehaviorIDToBehaviorName(float BehaviorID)
        {
            return BehaviorIDToBehaviorName((int)BehaviorID);
        }
        public static string BehaviorIDToBehaviorName(int BehaviorID)
        {
            if (UseAnimationClipConfig && AnimationClipConfigDic != null)
            {

                foreach (KeyValuePair<string, int> each in AnimationClipConfigDic)
                {
                    if (each.Value == BehaviorID)
                    {
                        return each.Key;
                    }
                }

                return null;

            }
            else
            {
                if (Enum.IsDefined(typeof(RTBActorBehaviorEnum), BehaviorID))
                {
                    RTBActorBehaviorEnum em = (RTBActorBehaviorEnum)BehaviorID;
                    return em.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 返回公共角色动作定义表中所对应的BehaviorID
        /// 
        /// 注意：如果该动作名字未在公共角色动作定义表中定义，则返回 0
        /// 
        /// </summary>
        public static int BehaviorNameToBehaviorID(string BehaviorName)
        {

            if (UseAnimationClipConfig && AnimationClipConfigDic != null)
            {
                foreach (KeyValuePair<string, int> each in AnimationClipConfigDic)
                {
                    if (each.Key == BehaviorName)
                    {
                        return each.Value;
                    }
                }

                return 0;
            }
            else
            {
                if (Enum.IsDefined(typeof (RTBActorBehaviorEnum), BehaviorName))
                {
                    RTBActorBehaviorEnum em =
                        (RTBActorBehaviorEnum) Enum.Parse(typeof (RTBActorBehaviorEnum), BehaviorName);
                    return (int) em;
                }
            }
            return 0;
        }

    }

}