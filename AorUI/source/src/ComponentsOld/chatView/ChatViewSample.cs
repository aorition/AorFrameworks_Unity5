using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.Debug;

namespace ExoticUnity.GUI.AorUI.Components
{
    [Serializable]
    public class ChatDataSample
    {

        public ChatDataSample(int id, string value)
        {
            this.id = id;
            this.value = value;
        }

        public int id;
        public string value;
    }

    /// <summary>
    /// 聊天显示容器 
    /// 
    /// author : aorition
    /// 
    /// ToDo 1.对头像框的支持 2.显示级别的缓存
    /// 
    /// </summary>
    public class ChatViewSample : AorUIComponent
    {

        [SerializeField]
        private List<ChatDataSample> _chatDatas;


        private List<AorTextIncSprites> _chatItemList;
        //private Dictionary<int, float> _chatDataPosCache;

        [SerializeField]
        private RectTransform _scrollRectRT;

        private ScrollRect _scrollRect;

        [SerializeField]
        private RectTransform _panel;

        //awake
        public override void OnAwake()
        {
            base.OnAwake();

            _chatItemList = new List<AorTextIncSprites>();
            //_chatDataPosCache = new Dictionary<int, float>();

            if (_chatDatas == null)
            {
                _chatDatas = new List<ChatDataSample>();
            }

            if (_scrollRectRT == null)
            {
                _scrollRect = transform.GetComponent<ScrollRect>();
                if (_scrollRect == null)
                {
                    _scrollRect = AorUiRuntimeUtility.CreatePrefab_ScrollRect(transform);
                    _scrollRect.gameObject.name = "ScrollRect#";

                    GameObject panelGo = AorUiRuntimeUtility.CreatePrefab_UIBase(_scrollRect.transform, 0, 0, 0, 100f, 0, 1f, 1f, 1f, .5f, 1f);
                    panelGo.name = "Panel#";

                    _panel = panelGo.GetComponent<RectTransform>();

                    _scrollRect.content = _panel;
                    _scrollRect.horizontal = false;
                    _scrollRect.vertical = true;

                }
                _scrollRectRT = _scrollRect.GetComponent<RectTransform>();
            }

            if (_panel == null)
            {
                if (_scrollRect.content != null)
                {
                    _panel = _scrollRect.content;
                }
                else
                {
                    GameObject panelGo = AorUiRuntimeUtility.CreatePrefab_UIBase(_scrollRect.transform, 0, 0, 0, 100f, 0, 1f, 1f, 1f, 0f, 1f);
                    panelGo.name = "Panel#";
                    _panel = panelGo.GetComponent<RectTransform>();
                    _scrollRect.content = _panel;
                }
            }

        }

        //start
        protected override void Initialization()
        {

            if (_chatDatas.Count > 0)
            {
                _createPos = 0;
                _isDirty = true;
            }

            //
            base.Initialization();
        }

        //-----------------------------------------------------------------------

        /// <summary>
        /// ChatItem 最大显示条数(Max值),超过该数值限制,ChatView会从最早的ChatItem开始删除,并只保留ChatItemLimitMin个ChatItem;
        /// </summary>
        public int ChatItemLimitMax = 256;
        /// <summary>
        /// ChatItem 最大显示条数(Min值),超过该数值限制,ChatView会从最早的ChatItem开始删除,并只保留ChatItemLimitMin个ChatItem;
        /// </summary>
        public int ChatItemLimitMin = 128;

        /// <summary>
        /// 添加一条Chat数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void addChatData(int id, string value)
        {
            _chatDatas.Add(new ChatDataSample(id, value));
            _isDirty = true;
        }

        /// <summary>
        /// 字体
        /// </summary>
        public Font chat_font;
        /// <summary>
        /// 默认字体大小
        /// </summary>
        public int chat_fontSize;
        /// <summary>
        /// 默认字体颜色
        /// </summary>
        public Color chat_fontColor;
        /// <summary>
        /// 插入图片的材质
        /// </summary>
        public Material chat_spriteMaterial;
        /// <summary>
        /// 插入图片资源描述类实例
        /// </summary>
        public SpriteAsset chat_spriteAsset;
        /// <summary>
        /// ChatItem之间的间隔距离
        /// </summary>
        public int chatItem_interval = 15;
        /// <summary>
        /// ChatItem头部的留空距离(类似CSS的padding-left)
        /// </summary>
        public int chatItem_offset = 10;
        /// <summary>
        /// 是否启用文字渐入动画,默认为不启用
        /// </summary>
        public bool useFadeAnimation = false;
        /// <summary>
        /// 文字渐入动画的滑行距离偏移.(值>0为从上向下滑入,值<0为从下向上滑入)
        /// </summary>
        public float FadeAnimation_start_offset = -100f;
        /// <summary>
        /// 文字渐入动画长度,单位为秒
        /// </summary>
        public float FadeAnimation_duration = 1f;
        /// <summary>
        /// 是否启用Panel偏移动画,默认是启用
        /// 调用movePanelToBottom方法时,如启用此项则会从当前Scroll位置动画滚动到最底,否则直接显示Scroll区域最底
        /// </summary>
        public bool usePanelMoveAnimation = true;
        /// <summary>
        /// Panel偏移动画长度,单位为秒
        /// </summary>
        public float PanelMoveAnimation_duration = 0.8f;
        /// <summary>
        /// 是否自动执行Panel偏移
        /// 如启用:每加入一条ChatItem时,ChatView会自动调用movePanelToBottom方法,将Scroll区域滚动到最底.
        /// </summary>
        public bool AutoPanelMove = true;

        /// <summary>
        /// <接口> 当ChatView中的超链接被点击时调用该委托.
        /// </summary>
        public Action<string> onLinksClicked;

        /// <summary>
        /// 清空已绘制的chatitem对象
        /// </summary>
        public void clearChatData()
        {

            _chatItemList.Clear();

            while (_panel.childCount > 0)
            {
                GameObject.DestroyImmediate(_panel.GetChild(0));
            }

            _createPos = 0;
        }

        /// <summary>
        /// 清空已绘制过的chatitem对象的位置缓存
        /// 注意** Chatview在生存期内不会自行清理该缓存
        /// </summary>
        /*
        public void clearPosCache() {
            _chatDataPosCache.Clear();
        }*/

        /// <summary>
        /// 移动Panel到最底部
        /// </summary>
        public void movePanelToBottom()
        {
            if (_panel.rect.height > _scrollRectRT.rect.height)
            {
                float tarY = _panel.rect.height - _scrollRectRT.rect.height;
                if (usePanelMoveAnimation)
                {
                    _panel.DOAnchorPosY(tarY, PanelMoveAnimation_duration, true).OnComplete(() => {
                        checkItemNumLimt();
                    });
                }
                else
                {
                    _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, tarY);
                    checkItemNumLimt();
                }
            }
            else
            {
                _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, 0);
            }
        }

        //-----------------------------------------------------------------------

        private float _createPos;

        protected override void DrawUI()
        {

            int i, length = _chatDatas.Count;
            for (i = 0; i < length; i++)
            {

                //                if (_chatDataPosCache.ContainsKey(_chatDatas[i].id)) {
                //                    createTextIncSprites(_chatDatas[i], new Vector2(chatItem_offset, -_chatDataPosCache[_chatDatas[i].id]));
                //                }
                //                else {
                _createPos += createTextIncSprites(_chatDatas[i], new Vector2(chatItem_offset, -_createPos)) + chatItem_interval;
                float pos = _createPos;
                //                    _chatDataPosCache.Add(_chatDatas[i].id, pos);
                //                }

            }

            _panel.sizeDelta = new Vector2(0, _createPos);
            if (AutoPanelMove)
            {
                movePanelToBottom();
            }
            else
            {
                //不启用AutoPanelMove时需要手动判定一次是否需要checkItemNumLimt
                if (_panel.anchoredPosition.y.FloatEquel(_panel.rect.height - _scrollRectRT.rect.height))
                {
                    checkItemNumLimt();
                }
            }

            _chatDatas.Clear();

            //
            base.DrawUI();
        }

        private void checkItemNumLimt()
        {
            if (_chatItemList.Count > ChatItemLimitMax)
            {
                while (_chatItemList.Count > ChatItemLimitMin)
                {

                    AorTextIncSprites atis = _chatItemList[0];
                    _chatItemList.RemoveAt(0);

                    GameObject.DestroyImmediate(atis.gameObject);

                }
                resetChatItemPos();
            }
        }

        private void resetChatItemPos()
        {

            _createPos = 0;

            int i, length = _chatItemList.Count;

            for (i = 0; i < length; i++)
            {
                _chatItemList[i].rectTransform.anchoredPosition = new Vector2(chatItem_offset, -_createPos);
                _createPos += _chatItemList[i].rectTransform.rect.height + chatItem_interval;
            }

            _panel.sizeDelta = new Vector2(0, _createPos);

            if (_panel.rect.height > _scrollRectRT.rect.height)
            {
                float tarY = _panel.rect.height - _scrollRectRT.rect.height;
                _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, tarY);
            }
            else
            {
                _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, 0);
            }

        }

        private float createTextIncSprites(ChatDataSample value, Vector2 pos)
        {

            GameObject tisGo = AorUiRuntimeUtility.CreatePrefab_UIBase(_panel, pos.x, pos.y, 0, 0, 0, 1f, 1f, 1f, 0, 1f);
            tisGo.name = "chatListItem#";

            AorTextIncSprites atts = tisGo.AddComponent<AorTextIncSprites>();
            atts.font = chat_font;
            atts.fontSize = chat_fontSize;
            atts.spriteGraphic_Color = chat_fontColor;
            atts.spriteGraphic_Material = chat_spriteMaterial;
            atts.spriteGraphic_SpriteAsset = chat_spriteAsset;
            atts.id = value.id;
            atts.text = value.value;
            atts.onHrefClick.AddListener(link => {
                if (onLinksClicked != null)
                {
                    onLinksClicked(link);
                    return;
                }

                UiLog.Info("ChatView > LinkClicked => " + link + " |");

            });

            float h = atts.preferredHeight;

            if (useFadeAnimation)
            {
                CanvasGroup cgp = atts.gameObject.AddComponent<CanvasGroup>();
                cgp.alpha = 0;

                RectTransform rt = atts.gameObject.GetComponent<RectTransform>();

                rt.localPosition = new Vector2(pos.x, pos.y + FadeAnimation_start_offset);
                atts.rectTransform.sizeDelta = new Vector2(-pos.x, h);

                rt.DOAnchorPosY(pos.y, FadeAnimation_duration, true);
                cgp.DOFade(1f, FadeAnimation_duration);

            }
            else
            {
                //                atts.rectTransform.anchoredPosition = new Vector2(pos.x, pos.y);
                atts.rectTransform.sizeDelta = new Vector2(-pos.x, h);
            }

            _chatItemList.Add(atts);

            return h;
        }

        protected override void OnDestroy()
        {

            if (_chatDatas != null)
            {
                _chatDatas.Clear();
                _chatDatas = null;
            }

            if (_chatItemList != null)
            {
                _chatItemList.Clear();
                _chatItemList = null;
            }

            /*
            if (_chatDataPosCache != null) {
                _chatDataPosCache.Clear();
                _chatDataPosCache = null;
            }*/

            _scrollRectRT = null;
            _scrollRect = null;
            _panel = null;

            base.OnDestroy();
        }
    }
}
