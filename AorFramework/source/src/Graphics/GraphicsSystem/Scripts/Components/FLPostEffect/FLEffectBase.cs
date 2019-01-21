#pragma warning disable
using System.Deployment.Internal;
using Framework.Graphic;
using UnityEngine;

[AddComponentMenu("")] //禁止此脚本放入ComponentMenu中
public class FLEffectBase : MonoBehaviour, IFLPostEffectComponent
{

    public int RenderLevel = 0;
    protected int _RenderLevel;
    protected Material renderMat;

    public virtual string ScriptName
    {
        get { return ""; }
    }

    public bool IsActive
    {
        get { return this.enabled && gameObject.activeInHierarchy; }
    }


    //public int Level => RenderLevel;

    public int Level {
        get { return RenderLevel; }
        set {
            RenderLevel = value;
        }
    }


    /// <summary>
    /// 独立运作模式 : 当脚本(包含子类)挂载到Camera节点上即开启独立运作模式, 该模式下脚本将直接对Camera本身起作用而不使用GraphicsManager渲染流.
    /// </summary>
    protected bool m_isStandaloneMode = false;
    private void Awake()
    {
        //检测是否独立运作模式
        m_isStandaloneMode = GetComponent<Camera>();
        init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void init()
    {
        //
    }

    protected virtual void OnEnable()
    {

        init();

        Camera cam = GetComponent<Camera>();
        m_isStandaloneMode = cam && cam.enabled;
        
        if (!m_isStandaloneMode)
        {
            GraphicsManager.Request(() =>
            {
                GraphicsManager.Instance.AddPostEffectComponent(this);
            });
        }
    }

    protected virtual void OnDisable()
    {
        if (!m_isStandaloneMode && GraphicsManager.IsInit())
            GraphicsManager.Instance.RemovePostEffectComponent(this);
    }

    //仅当立运作模式时才会调用该函数
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderMat == null || renderMat.shader == null)
            return;
        //        Graphics.Blit(src, dst, renderMat);
        render(source, destination);
    }

    public void RenderImage(RenderTexture src, RenderTexture dst)
    {
        if (_RenderLevel != RenderLevel && GraphicsManager.Instance != null)
        {
            GraphicsManager.Instance.SortEffectComponents();
        }

        if (renderMat == null || renderMat.shader == null)
            return;

        //        Graphics.Blit(src, dst, renderMat);
        render(src, dst);

    }

    protected virtual void render(RenderTexture src, RenderTexture dst)
    {

    }


}
