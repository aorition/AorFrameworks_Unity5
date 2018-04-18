using UnityEngine;
using UnityEngine.UI;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.Misc;

namespace ExoticUnity.GUI.AorUI.Components
{

    public class ProgressBarHandler : AorUIComponent {

        [SerializeField]
        private Text _echo;
        [SerializeField]
        private RectTransform _fillrect;
        [SerializeField]
        private RectTransform _bg;
        [SerializeField]
        private RectTransform _fill;

        [SerializeField]
        private Image _fillImage;
        public Image fillImage {
            get { return _fillImage; }
        }

        [SerializeField]
        private RectTransform _point;
        public RectTransform point {
            get { return _point; }
        }

        public override void OnAwake() {
            base.OnAwake();



        }

        // Use this for initialization
        protected override void Initialization() {
            if (_bg == null) {
                _bg = transform.FindChild("pgb_bg#").GetComponent<RectTransform>();
            }
            if (_fillrect == null) {
                _fillrect = _bg.FindChild("pgb_fillArea#").GetComponent<RectTransform>();
            }
            if (_fill == null) {
                _fill = _fillrect.FindChild("pgb_fill#").GetComponent<RectTransform>();
            }
            if (_fillImage == null) {
                _fillImage = _fill.GetComponent<Image>();
            }
            if (_echo == null) {
                _echo = _bg.FindChild("ValueEcho#").GetComponent<Text>();
            }
            if (_showValue) {
                _echo.gameObject.SetActive(true);
            } else {
                _echo.gameObject.SetActive(false);
            }
            _point = _bg.FindChild("pgb_point#").GetComponent<RectTransform>();
            if (_showPointer) {
                _point.gameObject.SetActive(true);
            } else {
                _point.gameObject.SetActive(false);
            }
            base.Initialization();
            _isDirty = true;
        }

        protected override void OnDestroy() {
            _bg = null;
            _fill = null;
            _fillImage = null;
            _point = null;
            _echo = null;
            _fillrect = null;
        }

        [SerializeField, SetProperty("LoadedBytes")]
        private float _LoadedBytes;
        public float LoadedBytes {
            get { return _LoadedBytes; }
            set {
                //if(_LoadedBytes != value) {
                _LoadedBytes = Mathf.Clamp(value, 0, _TotalBytes);
                _isDirty = true;
                //}
            }
        }
        [SerializeField, SetProperty("TotalBytes")]
        private float _TotalBytes = 100;
        public float TotalBytes {
            get { return _TotalBytes; }
            set {
                //if(_TotalBytes != value) {
                _TotalBytes = (value <= 0 ? 0 : value);
                _isDirty = true;
                //}
            }
        }

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

        [SerializeField, SetProperty("showPointer")]
        private bool _showPointer = false;
        public bool showPointer {
            get { return _showPointer; }
            set {
                _showPointer = value;

                if (_point != null) {
                    if (_showPointer) {
                        _point.gameObject.SetActive(true);
                    } else {
                        _point.gameObject.SetActive(false);
                    }
                }

            }
        }
        /*
        // Update is called once per frame
        void Update () {
	
        }*/

        protected override void DrawUI() {
            if (!_isStarted) return;

            float nv = Mathf.Clamp01(LoadedBytes/TotalBytes);

            if (_fillImage != null && _fillImage.type == Image.Type.Filled) {
                _fillImage.fillAmount = nv;
            }
            else {
                _fill.anchorMax = new Vector2(nv, 1f);                
            }

            _echo.text = Mathf.Clamp(LoadedBytes/TotalBytes*100, 0, 100f).ToString("F2") + " %";

            _point.anchoredPosition = new Vector3(_bg.rect.width * nv, 0f, 0f);

            
            base.DrawUI();
        }

    }
}