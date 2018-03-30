using System;
using Framework.Graphic;
using Framework.Graphic.Utility;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RoleGlowEffectCamera : MonoBehaviour, IGMPostEffectComponent
{

    private static readonly string RoleGlowLayerDefine = "RoleGlowLayer";

    private static Material _roleGlowEffectMat;

    [SerializeField]
    private int _size = 4; //4

    [SerializeField]
    private float _BlurRadius = 0.018f; //0.018f

    [SerializeField]
    private float _offset = 0.02f; //0.004f

    private RenderTextureCombine _PECombine;

    private RenderTexture _temp;
    [SerializeField] //debug
    private RenderTexture _rgRt;

    private bool _isInit = false;

    private Camera _target;
    private void Awake()
    {

        ////检查SubCamera组件-> RoleGlowEffectCombine 是否存在
        GraphicsManager.RequestGraphicsManager(() =>
        {

            _target = GetComponent<Camera>();
            _target.clearFlags = CameraClearFlags.SolidColor;
            _target.backgroundColor = new Color(0, 0, 0, 0);
            _target.cullingMask = 1 << LayerMask.NameToLayer(RoleGlowLayerDefine);
            _target.depth = 23; //定义为31是因为Role的Depth为25,且RoleGlowEffectCombine的Depth定义为24

//            _target.SetReplacementShader(Shader.Find("Hidden/FColorOutput"), "RenderType");
            _target.enabled = false;

            GraphicsManager.instance.AddSubCamera(_target);

            if (!_roleGlowEffectMat)
            {
                _roleGlowEffectMat = new Material(Shader.Find("Hidden/RppGlow"));
            }

            Camera combine = GraphicsManager.instance.GetSubCamera("RoleGlowEffectCombine");
            if (!combine)
            {
                GameObject combineGo = new GameObject("RoleGlowEffectCombine");
                combine = combineGo.AddComponent<Camera>();

                combine.clearFlags = CameraClearFlags.Nothing;
                combine.cullingMask = 0;
                combine.depth = 24; //定义为24是因为Role的Depth为25

                GraphicsManager.instance.AddSubCamera(combine);
            }

//            _PECombine = combine.gameObject.GetComponent<PostEffectCombine>();
//            if (!_PECombine) _PECombine = combine.gameObject.AddComponent<PostEffectCombine>();

            _PECombine = RenderTextureCombine.Create(combine.gameObject, RenderTextureCombine.PostEffectCombineType.Add);

            RoleGlowSettingAsset setting = GraphicsManager.instance.getSubSettingData<RoleGlowSettingAsset>();
            if (setting)
            {
                _size = setting.RTSize;
                _BlurRadius = setting.BlurRadius;
                _offset = setting.Offset;
                _PECombine.Power = setting.Power;
            }
            _isInit = true;
        });

    }

//    private void OnPreRender()
//    {
//        Shader.SetGlobalFloat("_vColorOutput", 1);
//    }
//
//    private void OnPostRender()
//    {
//        Shader.SetGlobalFloat("_vColorOutput", 0);
//    }

    private int _sizeCache;
    private Vector2 texSize = Vector2.one;
//    private void LateUpdate()
    public void UpdateGMPostEffect()
    {
        if (!_isInit || _PECombine == null) return;
        //
        if (_PECombine.renderTexture == null)
        {
//            _PECombine.renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
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
                //                RenderTexture.ReleaseTemporary(_temp);
                RenderTextureUtility.Release(_temp);
            }

            texSize = new Vector2(Screen.width / _sizeCache, Screen.height / _sizeCache);
            _temp = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo((int)texSize.x, (int)texSize.y)); 
        }

        _target.RenderWithShader(Shader.Find("Hidden/FColorOutput"), "RenderType");

        Graphics.Blit(_rgRt, _temp);

        _roleGlowEffectMat.SetTexture("_GlowTex", _temp);
        _roleGlowEffectMat.SetFloat("_TexSizeX", texSize.x / Screen.width);
        _roleGlowEffectMat.SetFloat("_TexSizeY", texSize.y / Screen.height);
        _roleGlowEffectMat.SetFloat("_BlurRadius", _BlurRadius);
        _roleGlowEffectMat.SetFloat("_Offest", _offset);

        Graphics.Blit(_rgRt, _PECombine.renderTexture, _roleGlowEffectMat);
    }

    private void OnEnable()
    {
        GraphicsManager.instance.AddPostEffectComponent(this);
    }

    private void OnDisable()
    {
        GraphicsManager.instance.RemovePostEffectComponent(this);
    }

}
