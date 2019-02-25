using System;
using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;

namespace Framework.UI
{

    /// <summary>
    /// AorUI舞台缩放方式定义
    /// </summary>
    public enum ScaleModeType
    {
        /// <summary>
        /// Unity缩放方式
        /// </summary>
        normal,
        /// <summary>
        /// 不缩放,已设计尺寸显示舞台大小
        /// </summary>
        noScale,
        /// <summary>
        /// 基于宽度缩放,再匹配高度
        /// </summary>
        widthBase,
        /// <summary>
        /// 基于高度缩放,再匹配宽度
        /// </summary>
        heightBase,
        /// <summary>
        /// 等比缩放到屏幕内部.
        /// </summary>
        fitScreen,
        /// <summary>
        /// 等比缩放裁切超出部分
        /// </summary>
        envelopeScreen
    }

    [RequireComponent(typeof(RectTransform))]
    public class ScaleLayout : AorUIComponent
    {

        private Vector2 _stageSize;
        public Vector2 DesignLayoutSize
        {
            get { return _stageSize; }
        }

        [SerializeField]//此序列化只用于编辑器模式下查看数据使用
        private ScaleModeType _scaleMode = ScaleModeType.noScale;
        /// <summary>
        /// 舞台缩放方式
        /// </summary>
        public ScaleModeType ScaleMode
        {
            get { return _scaleMode; }
            set
            {
                _scaleMode = value;
                //if (_isStartd) {
                changeScaleMode();
                //}
            }
        }

        private RectTransform _stage;
        public RectTransform Stage
        {
            get
            {
                if (!_stage)
                {
                    _stage = GetComponent<RectTransform>();
                }
                return _stage;
            }
        }


        protected override void OnAwake()
        {
            _stage = GetComponent<RectTransform>();
        }

        protected override bool Initialization()
        {

            return base.Initialization();
        }

        private void changeScaleMode()
        {
            if (_scaleMode == ScaleModeType.noScale)
            {
                _isDirty = false;
                Stage.localScale = new Vector3(1f, 1f, 1f);
                set2DefaultMode();
            }
            else if (_scaleMode == ScaleModeType.normal)
            {
                _isDirty = false;
                Stage.anchorMin = Vector2.zero;
                Stage.anchorMax = new Vector2(1f, 1f);
                Stage.sizeDelta = new Vector2(0, 0);
                Stage.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                set2DefaultMode();
                updateStageSize();
                _isDirty = true;
            }
        }

        private void updateStageSize()
        {

            if (Application.isEditor && !Application.isPlaying)
            {
                if (!_isInit)
                {
                    return;
                }
            }

            switch (_scaleMode)
            {
                case ScaleModeType.widthBase:
                    set2WidthBaseMode();
                    break;
                case ScaleModeType.heightBase:
                    set2HeightBaseMode();
                    break;
                case ScaleModeType.fitScreen:
                    set2FitScreenMode();
                    break;
                case ScaleModeType.envelopeScreen:
                    set2EnvelopeScreenMode();
                    break;
                default:
                    break;
            }

        }

        private Canvas _rootCanvas;
        public Canvas RootCanvas
        {
            get
            {
                if (!_rootCanvas)
                {
                    _rootCanvas = _stage.FindRootCanvas();
                }
                return _rootCanvas;
            }
        }

        private float _getWidthPernum()
        {
            
            return RootCanvas.pixelRect.width / _stageSize.x;
        }

        private float _getHeightPernum()
        {
            return RootCanvas.pixelRect.height / _stageSize.y;
        }

        private void set2WidthBaseMode()
        {
            float b = _getWidthPernum();
            Stage.localScale = new Vector3(b, b, b);
            float c = RootCanvas.pixelRect.height - _stageSize.y * b;
            Stage.sizeDelta = new Vector2(Stage.sizeDelta.x, _stageSize.y + c / b);
        }
        
        public Vector3 GetHeightBaseMode()
        {
            float b = RootCanvas.pixelRect.height / _stageSize.y;
            float aspect = (float)Screen.width / Screen.height;
            //编辑模式测试用
            if (aspect > 2.1f && (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                b *= (1 - 0.035f);
            }
            return new Vector3(b, b, b);
        }

        //0.035
        private void set2HeightBaseMode()
        {
            float aspect = (float)Screen.width / Screen.height;
            //编辑模式测试用
            if (aspect > 2.1f && (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                float b = _getWidthPernum();
                b *= (1 - 0.035f);
                Stage.localScale = new Vector3(b, b, b);
                Stage.anchoredPosition = new Vector2(0, RootCanvas.pixelRect.height * 0.0175f);
                float c = RootCanvas.pixelRect.width - (_stageSize.x + 120.0f) * b;
                Stage.sizeDelta = new Vector2(_stageSize.x + c / b, Stage.sizeDelta.y);
            }
            else
            {
                float b = _getWidthPernum();
                Stage.localScale = new Vector3(b, b, b);
                float c = RootCanvas.pixelRect.width - _stageSize.x * b;
                Stage.sizeDelta = new Vector2(_stageSize.x + c / b, Stage.sizeDelta.y);
            }
        }
        private void set2EnvelopeScreenMode()
        {
            //获取Screen宽高比
            float c_whb = RootCanvas.pixelRect.width / RootCanvas.pixelRect.height;
            float s_whb = _stageSize.x / _stageSize.y;
            if (c_whb < s_whb)
            {
                //竖
                float b = RootCanvas.pixelRect.height / _stageSize.y;
                Stage.localScale = new Vector3(b, b, b);
            }
            else
            {
                //横(正)
                float b = RootCanvas.pixelRect.width / _stageSize.x;
                Stage.localScale = new Vector3(b, b, b);
            }
        }
        private void set2FitScreenMode()
        {
            //获取Screen宽高比
            float c_whb = RootCanvas.pixelRect.width / RootCanvas.pixelRect.height;
            float s_whb = _stageSize.x / _stageSize.y;
            if (c_whb > s_whb)
            {
                //竖
                float b = RootCanvas.pixelRect.height / _stageSize.y;
                Stage.localScale = new Vector3(b, b, b);
            }
            else
            {
                //横(正)
                float b = RootCanvas.pixelRect.width / _stageSize.x;
                Stage.localScale = new Vector3(b, b, b);
            }
        }
        private void set2DefaultMode()
        {
            Stage.pivot = new Vector2(.5f, .5f);
            Stage.anchorMin = new Vector2(.5f, .5f);
            Stage.anchorMax = new Vector2(.5f, .5f);
            Stage.sizeDelta = new Vector2(_stageSize.x, _stageSize.y);
        }

    }

}

