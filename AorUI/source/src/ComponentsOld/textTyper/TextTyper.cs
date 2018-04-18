using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{

    /// <summary>
    /// 打字机组件
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class TextTyper : AorUIComponent {

        /// <summary>
        /// 打印每字的时间间隔(秒)
        /// </summary>
        public float Interval = 0.5f;


        private bool _isPlaying = false;
        public bool isPlaying {
            get { return _isPlaying; }
        }

        public bool AutoTypingOnEnable = false;

        private Text _text;
        private List<char> _typingList; 
        private string _typeData;
        private float _timer;
        private int _index;


        public Action onTypingFinish;

        public void setTypeData(string data) {
            _typeData = data;
            if (!string.IsNullOrEmpty(_typeData)) {
                _isDirty = true;
            }
        }

        public void SkipTyping() {
            if (!string.IsNullOrEmpty(_typeData) && _text != null) {
                _isPlaying = false;
                //
                _text.text = _typeData;

                if (onTypingFinish != null) {
                    Action doo = onTypingFinish;
                    doo();
                    onTypingFinish = null;
                }
            }
        }

        protected override void Initialization() {

            _text = transform.GetComponent<Text>();
            if (!_text)
            {
                _text = gameObject.AddComponent<Text>();
            }

            if (!string.IsNullOrEmpty(_typeData)) {
                _isDirty = true;
            }
            else {
                if (!string.IsNullOrEmpty(_text.text)) {
                    _typeData = _text.text;
                    _isDirty = true;
                }
            }
            
            base.Initialization();
        }

        protected override void DrawUI() {

            if (_typingList == null) {
                _typingList = new List<char>();
            }
            else {
                _typingList.Clear();
            }

            _typingList.AddRange(_typeData.ToCharArray());

            _timer = 0;
            _text.text = "";
            _index = 0;
            //
            if (AutoTypingOnEnable) {
                _isPlaying = true;
            }

            base.DrawUI();
        }

        public void Play() {
            if (_typingList.Count > 0) {
                _isPlaying = true;
            }
        }

        public void Stop() {
            _isPlaying = false;
        }

        public void ShowAll ()
        {
            _text.text = string.Empty;
            StringBuilder _sb = new StringBuilder ();
            for (int i = 0; i<_typingList.Count;++i )
            {
                _sb.Append (_typingList[i]);
            }
            _text.text = _sb.ToString ();
            _index = _typingList.Count-1;
            _isPlaying = false;
        }

        protected override void OnUpdate() {
            base.OnUpdate();

            if (_isPlaying) {
                _timer += Time.deltaTime;

                if (_timer >= Interval) {
                    _timer = 0;
                    //
                    _text.text = _text.text + _typingList[_index];
                    _index ++;

                    if (_index >= _typingList.Count) {
                        _isPlaying = false;

                        if (onTypingFinish != null) {
                            Action doo = onTypingFinish;
                            doo();
                            onTypingFinish = null;
                        }
                    }

                }

            }

        }
    }
}
