using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Graphic
{
    public class GMEffect
    {

        private static GMEffect _instance;
        public static GMEffect Instance
        {
            get { return _instance; }
        }

        public static GMEffect GetInstance(GraphicsManager manager)
        {
            if (_instance == null)
            {
                _instance = new GMEffect(manager);
            }
            return _instance;
        }

        //--------------------------------

        private Image _fadeUIImage;

        private Tweener _tweener_fadeOut;
        private Tweener _tweener_fadeIn;
        private Tweener _tweener_CameraShake;

        private Action _fadeInAction;
        private Action _fadeOutAction;

        private GraphicsManager _manager;
        private GMEffect(GraphicsManager manager)
        {
            _manager = manager;
        }

        private Vector3 _camShakeOffset;
        public Vector3 CamShakeOffset
        {
            get { return _camShakeOffset; }
        }

        //-----------------------------------

        private Vector3 __cameraShakePosCache;

        /// <summary>
        /// 抖动当前摄像机
        /// </summary>
        /// <param name="time">时间(秒)</param>
        /// <param name="power">力度(XY方向)</param>
        /// <param name="vibrato">振幅</param>
        public void CameraShake(float time, float power, int vibrato = 100)
        {
            CameraShake(time, power, power, 0, vibrato);
        }

        /// <summary>
        /// 抖动当前摄像机
        /// </summary>
        /// <param name="time">时间(秒)</param>
        /// <param name="powerX">力度(X方向)</param>
        /// <param name="powerY">力度(Y方向)</param>
        /// <param name="powerZ">力度(Z方向)</param>
        /// <param name="vibrato">振幅</param>
        public void CameraShake(float time, float powerX, float powerY, float powerZ, int vibrato = 100)
        {
            if (_tweener_CameraShake != null)
            {
                DOTween.Kill(_tweener_CameraShake);
                _tweener_CameraShake = null;
            }

            __cameraShakePosCache = _manager.transform.position;
            _camShakeOffset = Vector3.zero;

            _tweener_CameraShake = _manager.transform.DOShakePosition(time, new Vector3(powerX, powerY, powerZ), vibrato).SetEase(Ease.Linear).OnUpdate(
                () =>
                {
                    _camShakeOffset = _manager.transform.position - __cameraShakePosCache;
                }).OnComplete(() =>
                {
                    _manager.transform.position = __cameraShakePosCache;
                    _camShakeOffset = Vector3.zero;
                    _tweener_CameraShake = null;
                });
        }

        private Image _createFadeUIImage()
        {
            GameObject fadeUIGo = new GameObject("GMEffectFadeUI");
            fadeUIGo.layer = _manager.UIEffRoot.gameObject.layer;
            fadeUIGo.transform.SetParent(_manager.UIEffRoot, false);

            RectTransform rt = fadeUIGo.GetComponent<RectTransform>();
            if (!rt) rt = fadeUIGo.AddComponent<RectTransform>();

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;

            Image img = fadeUIGo.AddComponent<Image>();
            return img;
        }

        public void FadeIn(Color color, float time, bool interActive = false,Action finishCallback = null)
        {
            if (!_manager.UIEffRoot) return;
            //

            _clearFade(false);

            if (!_fadeUIImage)
            {
                _fadeUIImage = _createFadeUIImage();
            }

            if (!_fadeUIImage.gameObject.activeSelf)
            {
                _fadeUIImage.gameObject.SetActive(true);
            }

            _fadeUIImage.raycastTarget = interActive;
            _fadeUIImage.color = new Color(color.r, color.g, color.b, 1);

            _fadeInAction = finishCallback;

            _tweener_fadeIn = _fadeUIImage.DOFade(0, time).OnComplete(() =>
            {
                if (_fadeInAction != null)
                {
                    Action tmp = _fadeInAction;
                    tmp();
                    _fadeInAction = null;
                }
                _fadeUIImage.gameObject.SetActive(false);
                _tweener_fadeIn = null;
            });

        }
        public void FadeOut(Color color, float time, bool interActive = false, Action finishCallback = null)
        {
            if (!_manager.UIEffRoot) return;
            //

            _clearFade(false);

            if (!_fadeUIImage)
            {
                _fadeUIImage = _createFadeUIImage();
            }

            _fadeUIImage.raycastTarget = interActive;
            _fadeUIImage.color = new Color(color.r, color.g, color.b, 0);

            _fadeOutAction = finishCallback;

            if (!_fadeUIImage.gameObject.activeSelf)
            {
                _fadeUIImage.gameObject.SetActive(true);
            }

            _tweener_fadeOut = _fadeUIImage.DOFade(1, time).OnComplete(() =>
            {
                if (_fadeOutAction != null)
                {
                    Action tmp = _fadeOutAction;
                    tmp();
                    _fadeOutAction = null;
                }
                _tweener_fadeOut = null;
            });

        }
        private void _clearFade(bool deep)
        {
            if (!_manager.UIEffRoot || !_fadeUIImage) return;
            //
            if (_tweener_fadeOut != null || _tweener_fadeIn != null)
            {
                DOTween.Kill(_fadeUIImage);
                _tweener_fadeOut = null;
                _tweener_fadeIn = null;

                if (_fadeInAction != null)
                {
                    Action tmp = _fadeInAction;
                    tmp();
                    _fadeInAction = null;
                }

                if (_fadeOutAction != null)
                {
                    Action tmp = _fadeOutAction;
                    tmp();
                    _fadeOutAction = null;
                }

            }

            if (deep)
            {
                _fadeUIImage.gameObject.SetActive(false);
            }
        }
        public void ClearFade()
        {
            _clearFade(true);
        }
        

    }
}
