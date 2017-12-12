using System;
using UnityEngine;
//using YoukiaUnity.App;

namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 基于2DSprites的布告板系统
    /// </summary>
    public class PanoramicSprite : MonoBehaviour
    {

        /// <summary>
        /// Sprites
        /// </summary>
        public Sprite[] Sprites;
        /// <summary>
        /// InitialAngle
        /// </summary>
        public float InitialAngle = 0;

        private Camera _viewCam;

        /// <summary>
        /// ViewCam
        /// </summary>
        public Camera ViewCam
        {
            set { _viewCam = value; }
            get { return _viewCam ?? Camera.main; }
        }

        private SpriteRenderer _sprite;

        private GraphicsManager _graphicsManager;

        void Awake() {
            if (_graphicsManager == null) {
                _graphicsManager = GameObject.Find("GraphicsManager").GetComponent<GraphicsManager>();
            }
        }

        // Use this for initialization
        void Start() {
//            _viewCam = YKApplication.Instance.GetManager<GraphicsManager>().MainCamera;
            _viewCam = _graphicsManager.MainCamera;
            //            YKApplication.Instance.GetManager<GraphicsManager>().CameraFarClip = 50;
            _graphicsManager.CameraFarClip = 50;
            _sprite = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            int count = Sprites.Length;

            Vector2 worldDir = new Vector2(transform.forward.x, transform.forward.z).normalized;
            Vector3 camVec = transform.position - ViewCam.transform.position;
            Vector2 camDir = new Vector2(camVec.x, camVec.z).normalized;
            float degCam = Mathf.Acos(camDir.x);
            degCam = ((degCam * Mathf.Sign(camDir.y) / Mathf.PI) + 1) / 2f;
            float degObj = Mathf.PI - (float)Math.Acos(worldDir.x);
            degObj = ((degObj * Mathf.Sign(worldDir.y) / Mathf.PI) + 1) / 2f;
            degCam += degObj;
            degCam -= (InitialAngle / 180f);
            degCam += 0.5f / count;
            degCam -= (float)Math.Floor(degCam);
            //        UiLog.Error(degree.ToString());

            _sprite.sprite = Sprites[(int)Mathf.Floor(degCam * count)];
            //        _mat.SetTexture("_MainTex", TexArray[(int)Mathf.Floor(degCam * count)]);
        }
    }
}


