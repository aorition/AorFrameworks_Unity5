using UnityEngine;
using UnityEngine.UI;
using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.Misc;

namespace ExoticUnity.GUI.AorUI.Components
{
    public class BloodBarHandler : AorUIComponent {
        /*
        [HideInInspector]
        [SerializeField]
        float _Width;
        [HideInInspector]
        [SerializeField]
        float _Height;
        */
        // [HideInInspector]
        //  [SerializeField]

        //  [HideInInspector]
        //  [SerializeField]




        private int _animTimer = 0;

        private float _currentValue;

        //private Image _bgIMG;
        private Image _MbgIMG;
        private Image _TbgIMG;
        private Text _echo;
        /*
        public float BloodBarWidth{
            get { return _Width; }
            set {
                if (_Width != value) {
                    _Width = value;
                    changeValueFunc();
                }
            }
        }

        public float BloodBarHeight {
            get { return _Height; }
            set {
                if (_Height != value) {
                    _Height = value;
                    changeValueFunc();
                }
            }
        }
        */
        [SerializeField, SetProperty("showValue")]
        private bool _showValue = false;
        public bool showValue {
            get { return _showValue; }
            set {
                _showValue = value;
                if (_echo != null) {
                    if (_showValue) {
                        _echo.gameObject.SetActive(true);
                    } else {
                        _echo.gameObject.SetActive(false);
                    }
                }
            }
        }
        [SerializeField, SetProperty("Value")]
        private float _Value;
        public float Value {
            get { return _Value; }
            set {
                //if(_Value != value) {
                _Value = Mathf.Clamp(value, 0, _MaxValue);
                _isDirty = true;
                //}
            }
        }
        [SerializeField, SetProperty("MaxValue")]
        private float _MaxValue = 100;
        public float MaxValue {
            get { return _MaxValue; }
            set {
                //if(_MaxValue != value) {
                _MaxValue = (value <= 0 ? 0 : value);
                _isDirty = true;
                //}
            }
        }

        public int subAnimDelay = 2;

        // Use this for initialization
        protected override void Initialization() {
            
            // _bgIMG = GetComponent<Image>();
            _MbgIMG = transform.FindChild("MColorL#").GetComponent<Image>();
            _TbgIMG = transform.FindChild("TColorL#").GetComponent<Image>();
            _echo = transform.FindChild("ValueEcho#").GetComponent<Text>();
            if (!_showValue) {
                _echo.gameObject.SetActive(false);
            }
            _currentValue = _Value;
            /*
            _MbgIMG.rectTransform.localPosition = Vector3.zero;
            _TbgIMG.rectTransform.localPosition = Vector3.zero;
            */
            base.Initialization();
            _isDirty = true;
        }

        // Update is called once per frame
        protected override void OnUpdate() {
            base.OnUpdate();
            if (_isAnim) {
                _animTimer++;

                if (_animTimer >= subAnimDelay) {
                    _animTimer = 0;
                    if (_currentValue > _Value) {
                        _currentValue = _currentValue * 0.98f - MaxValue * 0.005f;
                        _MbgIMG.rectTransform.anchorMax = new Vector2(_currentValue / MaxValue, 1f);
                    } else {
                        _currentValue = _Value;
                        _MbgIMG.rectTransform.anchorMax = new Vector2(_TbgIMG.rectTransform.anchorMax.x, 1f);
                        _isAnim = false;
                        _animTimer = 0;
                    }
                }
            }
        }

        protected override void OnDestroy() {
            //  _bgIMG = null;
            _MbgIMG = null;
            _TbgIMG = null;
        }

        private bool _isAnim = false;

        protected override void DrawUI() {
            //Debug.UiLog("*************");
            if (!_isStarted) return;
            //set value
            //  Debug.UiLog("**** Value = " + Value);
            //string es = int.Parse(Value.ToString()).ToString() + " / " + int.Parse(MaxValue.ToString()).ToString();
            string es = Value.ToString("F0") + " / " + MaxValue.ToString("F0");
            _echo.text = es;

            if (Value > 0) {
                _TbgIMG.rectTransform.anchorMax = new Vector2(Value / MaxValue, 1f);
            } else {
                _TbgIMG.rectTransform.anchorMax = new Vector2(0, 1f);
            }

            if (_currentValue > _Value) {
                _isAnim = true;
            } else {
                _currentValue = _Value;
                _MbgIMG.rectTransform.anchorMax = new Vector2(_TbgIMG.rectTransform.anchorMax.x, 1f);
                _isAnim = false;
                _animTimer = 0;
            }

            base.DrawUI();
        }

    }
}