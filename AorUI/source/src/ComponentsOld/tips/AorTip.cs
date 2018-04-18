using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using ExoticUnity.GUI.AorUI.Animtion;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.Misc;

namespace ExoticUnity.GUI.AorUI.Components
{
    /// <summary>
    /// ToolTip组件
    /// 注意: 该组件支持的AorUIAnimator预处理动画仅仅支持Zoom,Fade和custom这3种预处理动画.如果需要
    /// </summary>
    public class AorTip : AorUIComponent {
        /*
        protected override string _getComponentType() {
            return 
        }*/
        /// <summary>
        /// isStatic属性为true时,在tip被关闭后不会被移除,而是被隐藏.
        /// </summary>
        public bool isStatic = false;
        /// <summary>
        /// autoOpenOnEnable属性为true时,在OnEnable事件被触发时,自动执行open方法;
        /// </summary>
        public bool autoOpenOnEnable = false;

        //单位为毫秒设置为1000以上才会有效
        /// <summary>
        /// 等待多少时间后(秒),tip对象会自动执行close方法; 默认值为0,则不启用自动延时关闭功能;
        /// ps: 运行时请使用delayClose方法;
        /// </summary>
        [SerializeField]
        protected float autoCloseOnDelayTime = 0;

        private RectTransform _bg;
        private Text _echo;

        [TextArea(5, 10)]
        [SerializeField, SetProperty("message")]
        private string _message = "tip massage";
        public string message {
            get { return _message; }
            set {
                _message = value;
                if (_echo != null) {
                    setTipMessage();
                }
            }
        }

        private float _autoCloseValue = 0;

        protected override void Initialization() {

            _uiAnimator = this.GetComponent<AorUIAnimator>();
            _bg = transform.FindChild("bg#").GetComponent<RectTransform>();
            _echo = transform.FindChild("bg#/echo#").GetComponent<Text>();
            this.gameObject.SetActive(false);
            base.Initialization();
            if (autoOpenOnEnable) {
                open();
            }
            setTipMessage();
        }
        protected override void OnDestroy() {
            _uiAnimator = null;
            _bg = null;
            _echo = null;
        }

        protected override void OnEnable() {
            base.OnEnable();
            if (!_isStarted) return;
            if (autoOpenOnEnable) {
                open();
            }
        }

        private void setTipMessage() {
            _message = _message.Replace("\\n", "\n");
            _echo.text = _message;
            StartCoroutine(setBGSize());
        }

        IEnumerator setBGSize() {
            yield return 0;
            if (_bg != null && _echo != null) {
                _bg.sizeDelta = new Vector2(_echo.preferredWidth + 10, _echo.preferredHeight + 10);
            }
        }

        IEnumerator delayCloseFunc() {
            while (true) {
                yield return 0;
                _autoCloseValue += Time.deltaTime;
                if (_autoCloseValue > autoCloseOnDelayTime) {
                    close();
                    break;
                }
            }
        }

        private void checkAutoClose() {
            if (!this.gameObject.activeInHierarchy) return;
            if (autoCloseOnDelayTime > 0) {
                _autoCloseValue = 0;
                StartCoroutine(delayCloseFunc());
            }
        }

        //-------------------------------------------------- 
        //----------------------------- Event interface
        /// <summary>
        /// 当tip打开时触发此事件委托
        /// </summary>
        public Action onOpen;
        /// <summary>
        /// 当tip关闭时触发此事件委托
        /// </summary>
        public Action onClose;
        /// <summary>
        /// 此事件委托用于重写tip的open行为;
        /// 注意实现此委托后,tip默认行为将不会执行;
        /// </summary>
        public Action<Action> customOpen;
        /// <summary>
        /// 此事件委托用于重写tip的close行为;
        /// 注意实现此委托后,tip默认行为将不会执行;
        /// </summary>
        public Action<Action> customClose;

        private AorUIAnimator _uiAnimator;
        /// <summary>
        /// 打开tip
        /// </summary>
        /// <param name="openedDoFunc">打开完成后触发回调</param>
        public void open(Action openedDoFunc = null) {
           // Debug.UiLog("open  ****************************** ");

            if (onOpen != null) {
                onOpen();
            }

            if (customOpen != null) {
                customOpen(openedDoFunc);
                return;
            }

            if (!gameObject.activeInHierarchy) {
                gameObject.SetActive(true);
            }
            if (_uiAnimator != null) {
                _uiAnimator.fadeIN(() => {
                    if (openedDoFunc != null) {
                        openedDoFunc();
                    }
               });
            } else {
                if (openedDoFunc != null) {
                    openedDoFunc();
                }
            }
            checkAutoClose();
        }

        public void delayClose(int delayTime) {
            autoCloseOnDelayTime = delayTime;
            checkAutoClose();
        }

        /// <summary>
        /// 关闭tip
        /// </summary>
        /// <param name="closedDoFunc">关闭完成执行回调</param>
        public void close(Action closedDoFunc = null) {

            if (onClose != null) {
                onClose();
            }

            if (customClose != null) {
                customClose(closedDoFunc);
                return;
            }

            if (_uiAnimator != null) {
                _uiAnimator.fadeOUT(() => {
                    fadeOutCompleteDo(closedDoFunc);
                });
            } else {
                fadeOutCompleteDo(closedDoFunc);
            }
        }
        private void fadeOutCompleteDo(Action closedDoFunc = null) {
            if (closedDoFunc != null) {
                closedDoFunc();
            }
            if (isStatic) {
                if (gameObject.activeInHierarchy) {
                    gameObject.SetActive(false);
                }
            } else {
                GameObject.Destroy(gameObject);
            }
        }
        
    }

}
