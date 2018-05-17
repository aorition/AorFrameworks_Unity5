using System;
using Framework.Graphic.Utility;
using UnityEngine;

namespace Framework.Graphic
{

    public class RoleShadowEffectCamera : MonoBehaviour, IRTPostEffectComponent
    {

        public static void Create()
        {
            //检查SubCamera组件-> RoleGlowEffectCam 是否存在
            GraphicsManager.Request(() =>
            {

                Camera camera = GraphicsManager.Instance.GetSubCamera("RoleShadowEffectCam");
                if (!camera)
                {

                    GameObject gecGo = new GameObject("RoleShadowEffectCam");
                    camera = gecGo.AddComponent<Camera>();
                }

                RoleShadowEffectCamera rgCamera = camera.gameObject.GetComponent<RoleShadowEffectCamera>();
                if (!rgCamera) camera.gameObject.AddComponent<RoleShadowEffectCamera>();
            });
        }

        private static Material _roleShadowEffectMat;

        [SerializeField] private int _size = 8; //2

        [SerializeField] private float _shadowMoffestX = 0.25f;
        [SerializeField] private float _shadowMoffestY = 0.25f;
        [SerializeField] private float _shadowMoffestW = 0;

        [SerializeField] private float _PxThreshold = 0.01f;

        [SerializeField] private float _power = 1f;

        [SerializeField] private bool _BlurEnable = true;

        [SerializeField] private float _BlurRadius = 0.01f; //0.004f

        //    private bool _offsetMatrixDirty = false;
        //    private Matrix4x4 _offsetMatrix;

        private bool _isInit;

        private Camera _target;

        [SerializeField] //debug
        private RenderTexture _rgRt;

        private RenderTexture _temp;

        private RenderTextureCombine _PECombine;

        private int _sizeCache;

        private void Awake()
        {
            ////检查SubCamera组件-> RoleGlowEffectCombine 是否存在
            GraphicsManager.Request(() =>
            {

                _target = GetComponent<Camera>();
                _target.clearFlags = CameraClearFlags.SolidColor;
                _target.backgroundColor = new Color(1, 1, 1, 0);
                _target.cullingMask = 1 << LayerMask.NameToLayer("Role") | 1 << LayerMask.NameToLayer("RoleGlowLayer");
                _target.depth = 23; //因为Role的Depth为25,且RoleGlowEffectCombine的Depth定义为24

                //            _target.SetReplacementShader(Shader.Find("Hidden/FShadowOutput"), "RenderType");

                _target.enabled = false;

                GraphicsManager.Instance.AddSubCamera(_target);

                //            _rgRt = RenderTexture.GetTemporary(Screen.width, Screen.height);
                //            _target.targetTexture = _rgRt;



                Camera combine = GraphicsManager.Instance.GetSubCamera("RoleShadowEffectCombine");
                if (!combine)
                {
                    GameObject combineGo = new GameObject("RoleShadowEffectCombine");
                    combine = combineGo.AddComponent<Camera>();

                    combine.clearFlags = CameraClearFlags.Nothing;
                    combine.cullingMask = 0;
                    combine.depth = 22; //因为Role的Depth为25

                    GraphicsManager.Instance.AddSubCamera(combine);
                }

                _PECombine = RenderTextureCombine.Create(combine.gameObject,
                    RenderTextureCombine.PostEffectCombineType.Multiply);

                RoleShadowSettingAsset setting = GraphicsManager.Instance.getSubSettingData<RoleShadowSettingAsset>();
                if (setting)
                {

                    _size = setting.RTSize;

                    _shadowMoffestX = setting.ShadowOffsetX;
                    _shadowMoffestY = setting.ShadowOffestY;
                    _shadowMoffestW = setting.ShadowOffestW;

                    _PxThreshold = setting.PxThreshold;
                    _power = setting.ShadowPower;

                    _BlurEnable = setting.EnableBlur;
                    _BlurRadius = setting.ShadowBlurRadius;

                }

                if (_BlurEnable)
                {
                    if (!_roleShadowEffectMat)
                    {
                        _roleShadowEffectMat = new Material(Shader.Find("Hidden/BaseBoxBlur"));
                    }
                }

                _isInit = true;
//            _offsetMatrixDirty = true;

            });
        }

        private Vector2 texSize = Vector2.one;

        public void UpdateGMPostEffect()
        {

            Shader.SetGlobalFloat("_FO_offsetX", _shadowMoffestX);
            Shader.SetGlobalFloat("_FO_offsetY", _shadowMoffestY);
            Shader.SetGlobalFloat("_FO_offsetW", _shadowMoffestW);

            //

            //        Shader.SetGlobalFloat("_BlurRadius", _BlurRadius);

            Shader.SetGlobalFloat("_FO_PxThreshold", _PxThreshold);

            Shader.SetGlobalFloat("_FO_Power", _power);


            if (!_isInit || _PECombine == null) return;
            //
            if (_PECombine.renderTexture == null)
            {
//                _PECombine.renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
                _PECombine.renderTexture = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo(Screen.width, Screen.height));
            }

            if (_sizeCache != _size)
            {
                _sizeCache = _size;

                if (_rgRt != null)
                {
                    //                    RenderTexture.ReleaseTemporary(_rgRt);
                    RenderTextureUtility.Release(_rgRt);
                }
//                _rgRt = RenderTexture.GetTemporary(Screen.width, Screen.height);
                _rgRt = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo(Screen.width, Screen.height));
                _target.targetTexture = _rgRt;

                if (_temp != null)
                {
                    //RenderTexture.ReleaseTemporary(_temp);
                    RenderTextureUtility.Release(_temp);
                }

            }

            _target.RenderWithShader(Shader.Find("Hidden/FShadowOutput"), "RenderType");

            if (_BlurEnable)
            {
                if (!_temp)
                {
                    //_temp = RenderTexture.GetTemporary(Screen.width/_sizeCache, Screen.height/_sizeCache);
                    _temp = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo(Screen.width / _sizeCache, Screen.height / _sizeCache));

                }

                _roleShadowEffectMat.SetFloat("_BlurRadius", _BlurRadius);
                Graphics.Blit(_rgRt, _temp, _roleShadowEffectMat);
                Graphics.Blit(_temp, _PECombine.renderTexture);

            }
            else
            {
                Graphics.Blit(_rgRt, _PECombine.renderTexture);
            }
        }

        private void OnEnable()
        {
            GraphicsManager.Request(() =>
            {
                GraphicsManager.Instance.AddPostEffectComponent(this);
            });
        }

        private void OnDisable()
        {
            if (GraphicsManager.IsInit())
            {
                GraphicsManager.Instance.RemovePostEffectComponent(this);
            }
        }


    }
}