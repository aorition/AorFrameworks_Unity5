using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.events;
using UnityEngine.EventSystems;

namespace ExoticUnity.GUI.AorUI.Components
{
    [ExecuteInEditMode]
    public class RenderTXView : AorUIComponent
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
        [SerializeField]
        [HideInInspector]
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
        protected Vector2 _tx_size;
        [SerializeField]
        [HideInInspector]
        protected int _tx_antiAliasing;
        [SerializeField]
        [HideInInspector]
        protected int _tx_depth;
        [SerializeField]
        [HideInInspector]
        protected TextureWrapMode _tx_wrapMode;
        [SerializeField]
        [HideInInspector]
        protected FilterMode _tx_filterMode;
        [SerializeField]
        [HideInInspector]
        protected int _tx_anisoLevel;


        [SerializeField]
        protected bool m_RotateModel;
        [SerializeField]
        protected Transform m_Model;
        [SerializeField]
        protected float m_rotateSpeed=10f;
        protected AorUIEventListener m_AorUIEventListener;


        public void RenderTexInit ( Vector2 size, int antiAliasing, int depth, TextureWrapMode twm, FilterMode fm, int anisoLevel )
        {

            _tx_size = new Vector2 (size.x, size.y);
            _tx_antiAliasing = antiAliasing;
            _tx_depth = depth;
            _tx_wrapMode = twm;
            _tx_filterMode = fm;
            _tx_anisoLevel = anisoLevel;

            if (m_RotateModel && null != m_Model)
            {
                m_AorUIEventListener = GetComponent<AorUIEventListener> ();
                if (null == m_AorUIEventListener)
                {
                    m_AorUIEventListener = gameObject.AddComponent<AorUIEventListener> ();
                }
                m_AorUIEventListener.Down = true;
                m_AorUIEventListener.Drag = true;
                m_AorUIEventListener.onEventDrag.RemoveAllListeners ();
                m_AorUIEventListener.onEventDrag.AddListener (onEventDrag);
                m_AorUIEventListener.onEventDown.RemoveAllListeners ();
                m_AorUIEventListener.onEventDown.AddListener (onEventDown);


            }
            RenderInit ();
        }

        private Vector2 m_lastPosition = Vector2.zero;
        private void onEventDown ( GameObject go, PointerEventData pd )
        {
            m_lastPosition = pd.position;
        }

        private void onEventDrag ( GameObject go, PointerEventData pd )
        {
            Vector3 _temp=  m_Model.localRotation.eulerAngles;
            _temp.y -= (pd.position.x - m_lastPosition.x) * Time.deltaTime * m_rotateSpeed;
            m_Model.localRotation = Quaternion.Euler (_temp);
            m_lastPosition = pd.position;
        }

        protected override void Initialization ()
        {
            base.Initialization ();

            RenderInit ();
        }

        protected override void OnEditorStart ()
        {
            base.OnEditorStart ();

            RenderInit ();
        }

        private void RenderInit ()
        {
            if (_renderCamera == null)
                return;

            if (_renderTex != null)
            {
                _renderTex.Release ();
                _renderTex = null;
            }
            _renderTex = new RenderTexture ((int)_tx_size.x, (int)_tx_size.y, _tx_depth, RenderTextureFormat.ARGB32);
            _renderTex.hideFlags = HideFlags.HideAndDontSave;
            _renderTex.anisoLevel = _tx_anisoLevel;
            _renderTex.wrapMode = _tx_wrapMode;
            _renderTex.filterMode = _tx_filterMode;
            _renderTex.antiAliasing = (_tx_antiAliasing <= 0) ? 1 : _tx_antiAliasing;

            _renderCamera.targetTexture = _renderTex;

            if (_rawImage == null)
            {
                _rawImage = GetComponent<RawImage> ();
                if (_rawImage == null)
                {
                    _rawImage = gameObject.AddComponent<RawImage> ();
                }
            }
            _rawImage.texture = _renderTex;

            //resetRT();
        }

        private void resetRT ()
        {
            RectTransform rt = GetComponent<RectTransform> ();

            rt.anchorMin = new Vector2 (0.5f, 0.5f);
            rt.anchorMax = new Vector2 (0.5f, 0.5f);
            rt.pivot = new Vector2 (0.5f, 0.5f);

            rt.localPosition = Vector3.zero;
            rt.localRotation = new Quaternion ();
            rt.localScale = Vector3.one;

            rt.sizeDelta = new Vector2 (_renderTex.width, _renderTex.height);

        }

        protected override void OnDestroy ()
        {
            if (_renderCamera != null)
            {
                if (Application.isEditor)
                {
                    GameObject.DestroyImmediate (_renderCamera.gameObject);

                }
                else
                {
                    GameObject.Destroy (_renderCamera.gameObject);
                }
                _renderCamera = null;
            }

            if (_renderTex != null)
            {
                _renderTex.Release ();
                _renderTex = null;
            }
        }

    }
}
