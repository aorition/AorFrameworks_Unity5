using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Framework.Graphic.Utility;

namespace Framework.UI
{

    public class RenderTextureView : AorUIComponent
    {

        [SerializeField]
        protected Camera _renderCamera;
        public Camera renderCamera
        {
            set
            {
                _renderCamera = value;
            }
        }
        
        protected RawImage _rawImage;
        public RawImage rawImage
        {
            set
            {
                _rawImage = value;
            }
        }

        private RenderTexture _renderTex;
        public RenderTexture renderTex
        {
            get
            {
                return _renderTex;
            }
        }

        [SerializeField]
        protected Vector2 _tx_size = new Vector2(512, 512);

        [SerializeField]
        [HideInInspector]
        protected int _tx_antiAliasing = 1;
        [SerializeField]
        [HideInInspector]
        protected int _tx_depth = 24;
        [SerializeField]
        [HideInInspector]
        protected TextureWrapMode _tx_wrapMode = TextureWrapMode.Clamp;
        [SerializeField]
        [HideInInspector]
        protected FilterMode _tx_filterMode= FilterMode.Bilinear;

        [SerializeField]
        [HideInInspector]
        protected int _tx_anisoLevel = 8;
        
        public void Setup(Camera camera, Vector2 size)
        {
            _renderCamera = camera;
            _tx_size = size;

            _isInit = Initialization();
        }

        public void Setup (Camera camera, Vector2 size, int antiAliasing, int depth, TextureWrapMode twm, FilterMode fm, int anisoLevel )
        {
            _renderCamera = camera;
            _tx_size = size;
            _tx_antiAliasing = antiAliasing;
            _tx_depth = depth;
            _tx_wrapMode = twm;
            _tx_filterMode = fm;
            _tx_anisoLevel = anisoLevel;

            _isInit = Initialization();
        }

        protected override bool Initialization ()
        {
            if (_renderCamera == null)
                return false;

            if (_renderTex != null)
            {
                _renderTex.Release();
                _renderTex = null;
            }
            _renderTex = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo((int)_tx_size.x, (int)_tx_size.y));
            _renderTex.hideFlags = HideFlags.HideAndDontSave;
            _renderTex.anisoLevel = _tx_anisoLevel;
            _renderTex.wrapMode = _tx_wrapMode;
            _renderTex.filterMode = _tx_filterMode;
            _renderTex.antiAliasing = (_tx_antiAliasing <= 0) ? 1 : _tx_antiAliasing;

            _renderCamera.targetTexture = _renderTex;

            if (_rawImage == null)
            {
                _rawImage = GetComponent<RawImage>();
                if (_rawImage == null)
                {
                    _rawImage = gameObject.AddComponent<RawImage>();
                    _rawImage.hideFlags = HideFlags.DontSave;
                }
            }
            _rawImage.texture = _renderTex;
            return true;
        }

        protected override void OnDestroy ()
        {
            if (_renderCamera)
            {
                _renderCamera = null;
            }

            if (_renderTex != null)
            {
                RenderTextureUtility.Release(_renderTex);
                _renderTex = null;
            }
        }

    }
}
