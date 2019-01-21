using System;
using Framework.Extends;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Utility
{
    /// <summary>
    /// 帧率统计器
    /// </summary>
    public class FPS : MonoBehaviour
    {

        public bool GUIDisplay = true;

        /// <summary>
        /// 每次刷新计算的时间      帧/秒
        /// </summary>
        public float updateInterval = 0.5f;
        /// <summary>
        /// 最后间隔结束时间
        /// </summary>
        private double _lastInterval;
        private int _frames = 0;

        public bool AutoGUIFontSize = false;
        private bool _AutoGUIFontSizeCache;

        public int FontSize = 18;
        private int _FontSizeCache;

        public Color FontColor = Color.red;
        private Color _FontColorCache;

        private GUIStyle _labelStyle;
        private GUIStyle labelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = GUI.skin.GetStyle("Label").Clone();
                    _labelStyle.fontSize = AutoGUIFontSize ? (int)((Screen.width < Screen.height ? Screen.width : Screen.height) / 48) : FontSize;
                    _labelStyle.alignment = TextAnchor.UpperLeft;
                    _labelStyle.normal.textColor = Color.red;

                    _AutoGUIFontSizeCache = AutoGUIFontSize;
                    _FontSizeCache = FontSize;
                    _FontColorCache = FontColor;
                }
                return _labelStyle;
            }
        }

        private float _currFPS;
        public float CurrentFPS
        {
            get { return _currFPS; }
        }


        void OnEnable()
        {
            _lastInterval = Time.realtimeSinceStartup;
            _frames = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            ++_frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > _lastInterval + updateInterval)
            {
                _currFPS = (float)(_frames / (timeNow - _lastInterval));
                _frames = 0;
                _lastInterval = timeNow;
            }

            Scene s = SceneManager.GetActiveScene();

        }

        private void OnGUI()
        {
            if (!GUIDisplay) return;

            if (_checkDirty()) _labelStyle = null;
            GUILayout.Label("FPS:" + _currFPS.ToString("f2"), labelStyle, GUILayout.Height(Screen.height * 0.2f));
        }

        private bool _checkDirty()
        {
            return !( _AutoGUIFontSizeCache.Equals(AutoGUIFontSize)
                      && _FontSizeCache.Equals(FontSize)  
                      && _FontColorCache.Equals(FontColor)
            );
        }

    }
}
