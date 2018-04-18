using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;
using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.Misc;

namespace ExoticUnity.GUI.AorUI.Components
{

    public class NumFollow : AorUIComponent {


        public float _animationSpeed = 0.3f;

        [SerializeField, SetProperty("Value")]
        private int _Value = 0;
        public int Value {
            get { return _Value; }
            set {
                _Value = value;
            }
        }
        private Text _text;
        // Use this for initialization

        protected override void Initialization() {
            _provalue = _current = _Value;
            _text = GetComponent<Text>();
            _text.text = _Value.ToString();
            base.Initialization();
        }
        private int _provalue;
        private int _current;
        public int currentValue {
            get { return _current; }
        }

        private void DoAnimation() {
            Tweener tr = DOTween.To(() => _current, x => _current = x, _Value, _animationSpeed).SetEase(Ease.Linear).OnUpdate(() => {
                _text.text = _current.ToString();
            });
        }

        protected override void OnDestroy() {
            _text = null;
        }

        // Update is called once per frame
        protected override void OnUpdate() {
            if (!_isStarted) return;
            if (_provalue != _Value) {
                DoAnimation();
                _provalue = _Value;
            }
        }
    }
}