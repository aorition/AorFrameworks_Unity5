using System;
using UnityEngine;
using UnityEngine.UI;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.Debug;
using ExoticUnity.Resource;
using Object = System.Object;

namespace ExoticUnity.GUI.AorUI.Components{

    public enum DialoguesSystemType {
        NotStarted, //没有初始化
        Init, //初始化
        Playing, //正在播放
        NextWaiting, //Next等待
        Pause, //暂停
        Stop //停止
    }

    public class DialoguesSystem : AorUIComponent {

        /// <summary>
        /// 背景容器
        /// </summary>
        public RectTransform background;

        /// <summary>
        /// 肖像A容器
        /// </summary>
        public RectTransform PortraitContentA;

        /// <summary>
        /// 肖像B容器
        /// </summary>
        public RectTransform PortraitContentB;

        /// <summary>
        /// 肖像标题显示容器
        /// </summary>
        public RectTransform TitleBox;

        private Text TitleBoxText;

        /// <summary>
        /// 对话内容显示容器
        /// </summary>
        public RectTransform DialogBox;

        private Text DialogBoxText;

        /// <summary>
        /// Skip按钮
        /// </summary>
        public Button SkipButton;

        public bool AutoDoNext = false;
        /// <summary>
        /// 自动切换对话间隔时间(秒)
        /// </summary>
        public float DoNextInterval = 1f;

        /// <summary>
        /// Skip延迟切换时间(秒)
        /// </summary>
        public float SkipInterval = 1f;

        /// <summary>
        /// 打字机效果时间间隔(秒)
        /// </summary>
        public float TypingTextInterval = 0.1f;

        private bool _isSkip;

        /// <summary>
        /// 对话内容
        /// </summary>
        private DialogData _data;

        //private RectTransform _PortraitsCachePool;

        private TextTyper _textTyper;

        private float _timer;

        public void setDialoguesData(DialogData data) {
            _data = data;

            if (_data != null) {
                _isDirty = true;
            }

        }

        /// <summary>
        /// 执行进度
        /// </summary>
        private int _index;
        public int index {
            get { return _index; }
        }

        //Awake
        public override void OnAwake() {
            base.OnAwake();

            if (background == null) {
                background = transform.FindChild("backround#").GetComponent<RectTransform>();
            }

            if (PortraitContentA == null) {
                PortraitContentA = transform.FindChild("PortraitContentA#").GetComponent<RectTransform>();
            }

            if (PortraitContentB == null) {
                PortraitContentB = transform.FindChild("PortraitContentB#").GetComponent<RectTransform>();
            }

            if (TitleBox == null) {
                TitleBox = transform.FindChild("TitleBox#").GetComponent<RectTransform>();
            }
            if (TitleBox != null) {
                TitleBoxText = TitleBox.GetComponent<Text>();
                if (TitleBoxText == null) {
                    TitleBoxText = TitleBox.gameObject.AddComponent<Text>();
                    //TODO 这里需要配置Text的系列参数
                }
            }
            else {
                UiLog.Error("DialoguesSystem.OnAwake Error :: 缺少重要模块 : TitleBox#");
                return;
            }

            if (DialogBox == null) {
                DialogBox = transform.FindChild("DialogBox#").GetComponent<RectTransform>();
            }
            if (DialogBox != null) {

                DialogBoxText = DialogBox.GetComponent<Text>();
                if (DialogBoxText == null) {
                    DialogBoxText = DialogBox.gameObject.AddComponent<Text>();
                    //TODO 这里需要配置Text的系列参数
                }

                _textTyper = DialogBox.gameObject.AddComponent<TextTyper>();
              //  _textTyper.FinishCall(_textTyper.GetType().ToString());
                _textTyper.Interval = TypingTextInterval;
            }

            else {
                UiLog.Error("DialoguesSystem.OnAwake Error :: 缺少重要模块 : DialogBox#");
                return;
            }

            if (SkipButton == null) {
                SkipButton = transform.FindChild("SkipButton#").GetComponent<Button>();
            }

            SkipButton.onClick.AddListener(() => {
                Skip();
            });

            /*
            _PortraitsCachePool = new GameObject("PortraitsCachePool").AddComponent<RectTransform>();
            _PortraitsCachePool.SetParent(transform, false);
            _PortraitsCachePool.gameObject.SetActive(false);
            */

            //hide
            background.localScale = Vector3.zero;
           // PortraitContentA.localScale = Vector3.zero;
           // PortraitContentB.localScale = Vector3.zero;
            TitleBox.transform.localScale = Vector3.zero;
            DialogBox.transform.localScale = Vector3.zero;
            SkipButton.transform.localScale = Vector3.zero;
        }

        //Start
        protected override void Initialization() {
            base.Initialization();
        }

        public Action OnDrawUI;

        //UI初始化
        protected override void DrawUI() {
            //缓存所有要使用的头像和标题
            foreach (DialogPortraitInfo info in _data.PortraitDic.Values) {

                if (info.PortraitType == DialogPortraitType.prefab) {
                    //ExoticApplication.Instance.GetManager<ResourcesManager>().PoolMg.CacheObjToPool(info.PortraitPath, (obj) => { });
                    AorUIAssetLoader.CachePrefab(info.PortraitPath, (obj) => { });

                } else {
                    //ExoticApplication.Instance.GetManager<ResourcesManager>().CacheObject(info.PortraitPath, (obj) => { });
                    AorUIAssetLoader.CacheAsset(info.PortraitPath, (obj) => { });

                    if (info.PortraitMaterialPath != "") {
                        AorUIAssetLoader.CacheAsset(info.PortraitMaterialPath, (obj) => { });
                    }

                }

                if (info.PortraitTitleType == DialogPortraitTitleType.Image) {
                    //ExoticApplication.Instance.GetManager<ResourcesManager>().CacheObject(info.PortraitTitlePath, (obj) => { });
                    AorUIAssetLoader.CachePrefab(info.PortraitTitleValue, (obj) => { });

                    if (info.PortraitTitleMaterialPath != "") {
                        AorUIAssetLoader.CacheAsset(info.PortraitTitleMaterialPath, (obj) => { });
                    }

                }



            }
            //
            background.localScale = Vector3.one;
            //PortraitContentA.localScale = Vector3.one;
            //PortraitContentB.localScale = Vector3.one;
            TitleBox.transform.localScale = Vector3.one;
            DialogBox.transform.localScale = Vector3.one;
            SkipButton.transform.localScale = Vector3.one;
            //
            _dialoguesType = DialoguesSystemType.Init;

            if (OnDrawUI != null)
            {
                OnDrawUI();
            }

            base.DrawUI();
        }

        [SerializeField] //仅作编辑器查看
        private DialoguesSystemType _dialoguesType = DialoguesSystemType.NotStarted;

        protected override void OnUpdate() {
            base.OnUpdate();
            //

            if (!_isStarted) return;

            if (DelaySetParent != null) {
                DelaySetParent();
            }

            if (_dialoguesType == DialoguesSystemType.Init && InitAfterPlay != null) {
                Action DoInitAfterPlay = InitAfterPlay;
                DoInitAfterPlay();
                InitAfterPlay = null;
            }

            if (_isSkip && _dialoguesType == DialoguesSystemType.NextWaiting) {
                updateNextLogic(Time.deltaTime, SkipInterval);
                return;
            }

            if (AutoDoNext && _dialoguesType == DialoguesSystemType.NextWaiting) {
                updateNextLogic(Time.deltaTime, DoNextInterval);
            }

        }

        private void updateNextLogic(float deltaTime, float limtTime) {
            if (_dialoguesType == DialoguesSystemType.NextWaiting) {

                _timer += deltaTime;
                if (_timer >= limtTime) {
                    
                    int n = _index + 1;
                    Play(n);

                    _timer = 0;
                    _isSkip = false;
                }

            }
        }

        /// <summary>
        /// (接口) 所有内容播放完成后调用该事件委托
        /// </summary>
        public Action OnPlayFinished;


        /// <summary>
        /// (接口) 开始播放时调用该事件委托
        /// 说明： 该事件委托包含上一个对话完成后，开启下一个对话播放时，也会调用该事件委托
        /// </summary>
        public Action<int> OnPlayStart;


        private Action InitAfterPlay;

        private Action DelaySetParent;

        public void Play(int index = 0) {

            if (_dialoguesType == DialoguesSystemType.NotStarted) {
                int i = index;
                InitAfterPlay = () => {
                    Play(i);
                };
                return;
            }

            if (index < 0) {
                index = 0;
            }
            else if (index >= _data.DialogList.Count) {
                _dialoguesType = DialoguesSystemType.Stop;
                if (OnPlayFinished != null) {
                    OnPlayFinished();
                }
                if (OnStop != null) {
                    OnStop(_index);
                }
                return;
            }

            _index = index;
            _dialoguesType = DialoguesSystemType.Playing;
            //

            if (OnPlayStart != null)
            {
                OnPlayStart(_index);
            }

            DialogPortraitInfo currentInfo = _data.PortraitDic[_data.PortraitList[_index]];

            //归置头像框
            resetPortrait(_index, currentInfo, _data.ProtraitPosList[_index], _data.DialogEventList[_index]);
            //归置Text
            DialogBoxText.text = "";

            //加载头像:
            if (currentInfo.PortraitType == DialogPortraitType.prefab) {
                //GameObject
                AorUIAssetLoader.LoadPrefabFromPool(currentInfo.PortraitPath, (go) => {

                    if (_data.ProtraitPosList[_index] == DialogPortraitPosition.PositionA) {
                        //PositionA
                        go.transform.SetParent(PortraitContentA, false);
                    } else {
                        //PositionB
                        go.transform.SetParent(PortraitContentB, false);
                    }

                    if (!go.activeInHierarchy) {
                        go.SetActive(true);
                    }

                });

            }else if(currentInfo.PortraitType == DialogPortraitType.Image) {
                //Sprite

                string[] imgPaths = currentInfo.PortraitPath.Split('@');
                if (imgPaths.Length < 2) {
                    UiLog.Error("DialoguesSystem.Play Error : 肖像数据有错误, 数据中不包含正确的Sprite名称: >> 错误的DialogPortraitInfo[" + currentInfo.PortraitID + "]");
                    return;
                }

                AorUIAssetLoader.LoadSprite(imgPaths[0], imgPaths[1], (sp,objRef) => {
                    if (currentInfo.PortraitMaterialPath != "") {

                        AorUIAssetLoader.LoadUIAssetAsync(currentInfo.PortraitMaterialPath, (mt,obj) => {
                            Material material = (Material)mt;
                            setupPortraitImage(sp, _data.ProtraitPosList[_index], material);
                        });

                    }
                    else {
                        setupPortraitImage(sp, _data.ProtraitPosList[_index], null);
                    }
                });
            }else {
                //Textrue2D
                AorUIAssetLoader.LoadUIAssetAsync(currentInfo.PortraitPath, (texAsset,obj) => {

                    ResourcesManager.ResourceRef resRef = obj[0] as ResourcesManager.ResourceRef;
                    Texture2D tex = texAsset as Texture2D;

                    if (currentInfo.PortraitMaterialPath != "") {

                        AorUIAssetLoader.LoadUIAssetAsync(currentInfo.PortraitMaterialPath, (mt,obj2) => {
                            Material material = (Material)mt;
                            setupPortraitRawImage(tex, _data.ProtraitPosList[_index], material);
                        });

                    } else {
                        setupPortraitRawImage(tex, _data.ProtraitPosList[_index], null);
                    }
                    
                    
                });

            }

            //标题
            if (currentInfo.PortraitTitleType == DialogPortraitTitleType.Text) {
                TitleBoxText.text = currentInfo.PortraitTitleValue;
            }
            else {
                string[] imgPaths = currentInfo.PortraitPath.Split('@');
                if (imgPaths.Length < 2) {
                    UiLog.Error("DialoguesSystem.Play Error : 肖像标题数据有错误, 数据中不包含正确的TitleSprite名称: >> 错误的DialogPortraitInfo[" + currentInfo.PortraitID + "]");
                    return;
                }
                //Image
                AorUIAssetLoader.LoadSprite(imgPaths[0], imgPaths[1], (sp,objRef) => {

                    if (currentInfo.PortraitTitleMaterialPath != "") {
                        AorUIAssetLoader.LoadUIAssetAsync(currentInfo.PortraitTitleMaterialPath, (mt,obj) => {
                            Material material = (Material) mt;
                            setupPortaitImgTitle(sp, material);
                        });

                    }
                    else {
                        setupPortaitImgTitle(sp, null);
                    }
                });
            }

            //对话播放
            if (_textTyper.onTypingFinish == null) {
                _textTyper.onTypingFinish = () => {
                    _dialoguesType = DialoguesSystemType.NextWaiting;
                };
            }
            _textTyper.AutoTypingOnEnable = true;
            _textTyper.setTypeData(_data.DialogList[_index]);
            _timer = 0;
        }

        /// <summary>
        /// (接口) 自定义 肖像清理方法,
        /// 说明: 当上一段话进入下一段对话时回调用此委托, 重写此委托实现肖像容器的归置, 如保留上一个肖像的显示,或者同时显示肖像容器A和B ...
        /// 
        /// 传入参数:
        /// int : 当前执行序号.
        /// DialogPortraitInfo : 当前肖像的信息
        /// DialogPortraitType : 当前肖像的位置( 在A/B ?)
        /// string : 事件内容 (事件内容由配置表提供,默认情况为"");
        /// 
        /// </summary>
        public Action<int, DialogPortraitInfo, DialogPortraitPosition, string> CustomResetPortraitFunc;

        private void resetPortrait(int index, DialogPortraitInfo PortraitInfo, DialogPortraitPosition PortraitPos, string Event) {

            if (CustomResetPortraitFunc != null) {
                CustomResetPortraitFunc(index, PortraitInfo, PortraitPos, Event);
                return;
            }

            //默认肖像清理方式: 
            //prefab
            while (PortraitContentA.transform.childCount > 0) {
                Transform t = PortraitContentA.transform.GetChild(0);
                AorUIAssetLoader.PutPrefabInPool(t.gameObject);
            }
            while (PortraitContentB.transform.childCount > 0) {
                Transform t = PortraitContentB.transform.GetChild(0);
                AorUIAssetLoader.PutPrefabInPool(t.gameObject);
            }
            //Image
            Image imA = PortraitContentA.GetComponent<Image>();
            if (imA != null) {
                GameObject.DestroyImmediate(imA);
            }
            Image imB = PortraitContentB.GetComponent<Image>();
            if (imB != null) {
                GameObject.DestroyImmediate(imB);
            }
            //RawImage
            RawImage rA = PortraitContentA.GetComponent<RawImage>();
            if (rA != null) {
                GameObject.DestroyImmediate(rA);
            }
            RawImage rB = PortraitContentB.GetComponent<RawImage>();
            if (rB != null) {
                GameObject.DestroyImmediate(rB);
            }
        }

        private void setupPortaitImgTitle(Sprite sp, Material mat) {
            GameObject titleImg;
            Transform titleImgRt = TitleBox.FindChild("TitleImg#");
            if (titleImgRt == null) {
                titleImg = AorUiRuntimeUtility.CreatePrefab_UIBase(TitleBox);
                titleImg.name = "TitleImg#";
            } else {
                titleImg = titleImgRt.gameObject;
            }

            Image im = titleImg.GetComponent<Image>();
            if (im == null) {
                im = titleImg.gameObject.AddComponent<Image>();
            }

            if (mat != null) {
                im.material = mat;
            }

            im.sprite = sp;
            im.preserveAspect = true;
            im.color = new Color(1f, 1f, 1f, 1f);
        }

        private void setupPortraitRawImage(Texture2D tex, DialogPortraitPosition pos, Material mat) {
            RawImage ri;
            if (pos == DialogPortraitPosition.PositionA) {
                //PositionA
                ri = PortraitContentA.GetComponent<RawImage>();
                if (ri == null) {
                    ri = PortraitContentA.gameObject.AddComponent<RawImage>();
                }
            } else {
                //PositionB
                ri = PortraitContentB.GetComponent<RawImage>();
                if (ri == null) {
                    ri = PortraitContentB.gameObject.AddComponent<RawImage>();
                }
            }

            if (mat != null) {
                ri.material = mat;
            }

            ri.color = new Color(1f, 1f, 1f, 1f);
            ri.texture = tex;

        }
        
        /// <summary>
        /// 装载Image
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="pos"></param>
        private void setupPortraitImage(Sprite sp, DialogPortraitPosition pos, Material mat) {
            Image im;
            if (pos == DialogPortraitPosition.PositionA) {
                //PositionA
                im = PortraitContentA.GetComponent<Image>();
                if (im == null) {
                    im = PortraitContentA.gameObject.AddComponent<Image>();
                }
            }
            else {
                //PositionB
                im = PortraitContentB.GetComponent<Image>();
                if (im == null) {
                    im = PortraitContentB.gameObject.AddComponent<Image>();
                }
            }

            if (mat != null) {
                im.material = mat;
            }

            im.preserveAspect = true;
            im.color = new Color(1f, 1f, 1f, 1f);
            im.sprite = sp;
        }

        /// <summary>
        /// (接口) 在发生Skip行为是调用该事件委托
        /// 
        /// 参数传入
        /// 
        /// int : skip时的执行序号
        /// 
        /// </summary>
        public Action<int> OnSkip;
        public void Skip() {
            if (_dialoguesType == DialoguesSystemType.NotStarted || _dialoguesType == DialoguesSystemType.Stop) {
                return;
            }

            if (_dialoguesType == DialoguesSystemType.Playing) {
                _textTyper.SkipTyping();
            }
            if (OnSkip != null) {
                OnSkip(_index);
            }
            _isSkip = true;
        }

        public Action<int> OnPauseContinue;

        public void PauseContinue() {
            if (_dialoguesType == DialoguesSystemType.Pause) {
                _dialoguesType = DialoguesSystemType.Playing;
                _textTyper.Play();

                if (OnPauseContinue != null) {
                    OnPauseContinue(_index);
                }

            }
        }

        public Action<int> OnPause;

        public void Pause() {

            if (_dialoguesType == DialoguesSystemType.Playing) {

                _dialoguesType = DialoguesSystemType.Pause;

                _textTyper.Stop();

                if (OnPause != null) {
                    OnPause(_index);
                }
            }
        }

        public Action<int> OnStop;
        public void Stop() {

            resetPortrait(_index, _data.PortraitDic[_data.PortraitList[_index]], _data.ProtraitPosList[_index], _data.DialogEventList[_index]);

            _textTyper.Stop();
            DialogBoxText.text = "";

            if (OnStop != null) {
                OnStop(_index);
            }
        }

    }
}
