using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡溶解效果脚本
/// 
/// 使用方式将脚本挂载到对应角色上(挂有MeshRender的GameObject)即可使用.
///
/// 注意事项: AddComponent此脚本之前, 记得先填充 DeathDissolveEffectHandler.NoiseTexLink (静态属性). 否则初始化失败.
/// 
/// 友情提示:: 预置Noise贴图加载路径: Resources/Effect/Common/Textures/noise001
/// 
/// </summary>
public class DeathDissolveEffectHandler : MonoBehaviour
{

    private const string DissoleEffectShaderName = "Spine/Skeleton-DissolveEffect##";
    public static Texture2D NoiseTexLink;

    public Texture2D DissolveMap;

    public Color Color = new Color(1, 1, 1, 1);
    public Color DissolveColor = new Color(0, 0, 0, 0);
    public Color DissolveEdgeColor = new Color(0.23529f, 0.0003921f, 0.2f, 0);
    public Vector2 DissolveMapTiling = new Vector2(0.9f, 2.1f);

    public float ColorFactor = 0.508f;
    public float DissolveEdge = 0.157f;

    /// <summary>
    /// 溶解动画长度(秒)
    /// </summary>
    public float DissolveDuration = 1;

    /// <summary>
    /// 溶解动画播放完毕后触发的回调.
    /// </summary>
    public Action OnDissoleFinished;

    private Renderer _renderer;
    private Material _orginalMaterial;
    private Material _dissolveMaterial;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer)
        {
            _orginalMaterial = _renderer.material;
        }
    }

    private void OnEnable()
    {
        if(!_orginalMaterial) return;

        if (!DissolveMap)
        {
            if (!NoiseTexLink)
            {
                Debug.LogError(
                    "DeathDissolveEffectHandler.OnEnable Error :: 请优先填充 DeathDissolveEffectHandler.NosieTexLink 字段,再AddComponent.");
            }
            else
            {
                DissolveMap = NoiseTexLink;
            }
        }

        if (!_dissolveMaterial)
        {
            _dissolveMaterial = new Material(_orginalMaterial);
            _dissolveMaterial.shader = Shader.Find(DissoleEffectShaderName);

            _dissolveMaterial.SetColor("_Color", Color);
            _dissolveMaterial.SetColor("_DissolveColor", DissolveColor);
            _dissolveMaterial.SetColor("_DissolveEdgeColor", DissolveEdgeColor);

            _dissolveMaterial.SetTexture("_DissolveMap", DissolveMap);
            _dissolveMaterial.SetTextureScale("_DissolveMap", DissolveMapTiling);
            
            _dissolveMaterial.SetFloat("_DissolveThreshold", 0);
            _dissolveMaterial.SetFloat("_ColorFactor", ColorFactor);
            _dissolveMaterial.SetFloat("_DissolveEdge", DissolveEdge);

        }

        _renderer.material = _dissolveMaterial;
        _dissolveValue = 0;
        _isFinished = false;
    }

    private bool _isFinished = false;
    private float _dissolveValue;
    private void Update()
    {
        if (_isFinished || !_orginalMaterial) return;
        
        
        _dissolveValue += Time.deltaTime;
        if (_dissolveValue > DissolveDuration)
        {
            _dissolveMaterial.SetFloat("_DissolveThreshold", 1f);
            if (OnDissoleFinished != null)
            {
                Action tmp = OnDissoleFinished;
                tmp();
                OnDissoleFinished = null;
            }
            _isFinished = true;
        }
        else
        {
            _dissolveMaterial.SetFloat("_DissolveThreshold", _dissolveValue / DissolveDuration);
        }
    }

    private void OnDisable()
    {
        _renderer.material = _orginalMaterial;
    }

    private void OnDestroy()
    {
        if (_dissolveMaterial) _dissolveMaterial = null;
        if (_orginalMaterial) _orginalMaterial = null;
        if (_renderer) _renderer = null;
        OnDissoleFinished = null;
    }

}
