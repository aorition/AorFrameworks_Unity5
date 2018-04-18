using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;

namespace Framework.UI
{

    [RequireComponent(typeof(Text))]
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

        protected override void OnAwake()
        {
            base.OnAwake();
            _text = GetComponent<Text>();
        }

        protected override bool Initialization() {
            _provalue = _current = _Value;
            _text.text = _Value.ToString();
            return base.Initialization();
        }
        private int _provalue;
        private int _current;
        public int currentValue {
            get { return _current; }
        }

        private Tweener _tweener;
        private void DoAnimation() {
            _tweener = DOTween.To(() => _current, x => _current = x, _Value, _animationSpeed).SetEase(Ease.Linear).OnUpdate(() => {
                _text.text = _current.ToString();
            });
        }

        protected override void OnDestroy() {
            _text = null;
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
        }

        // Update is called once per frame
        protected override void OnUpdate() {
            if (!_isInit) return;
            if (_provalue != _Value) {
                DoAnimation();
                _provalue = _Value;
            }
        }
    }
}