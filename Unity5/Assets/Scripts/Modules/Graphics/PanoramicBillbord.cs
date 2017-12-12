using System;
using UnityEngine;
//using YoukiaUnity.App;


namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 布告板系统
    /// </summary>
    public class PanoramicBillbord : MonoBehaviour {
        /// <summary>
        /// TexArray
        /// </summary>
        public Texture[] TexArray;
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
            get { return _viewCam; }
        }

        private GraphicsManager _graphicsManager;

        private Material _mat;

//        public override void OnScriptCoverFinish()
        void Awake()
        {
//            base.OnScriptCoverFinish();
            if (_graphicsManager == null) {
                _graphicsManager = GameObject.Find("GraphicsManager").GetComponent<GraphicsManager>();
            }


            if (_graphicsManager.MainCamera != null) {
                _viewCam = _graphicsManager.MainCamera;
                _graphicsManager.CameraFarClip = 50;
            }
            else
            {

                if (Camera.main == null)
                {
                    _viewCam = new GameObject("MainCamera").AddComponent<Camera>();
                    _viewCam.tag = "MainCamera";
                }
            }

            _mat = GetComponent<MeshRenderer>().material;
        }


//        protected override void OnUpdate()
        void Update()
        {
//            base.OnUpdate();
            int count = TexArray.Length;

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
            _mat.SetTexture("_MainTex", TexArray[(int)Mathf.Floor(degCam * count)]);
        }
    }

}


