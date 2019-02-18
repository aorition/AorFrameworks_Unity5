﻿#pragma warning disable
using System.Deployment.Internal;
using Framework.Graphic;
using UnityEngine;

[AddComponentMenu("")] //禁止此脚本放入ComponentMenu中
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class FLEffectBase : MonoBehaviour, IFLPostEffectComponent
{
    

    public virtual string ScriptName
    {
        get { return string.Empty; }
    }

    public bool IsActive
    {
        get { return this.enabled && gameObject.activeInHierarchy; }
    }
    
    [SerializeField, HideInInspector]
    protected int m_RenderLevel = 0;
    private bool m_RenderLevelDirty = false;
    public int Level {
        get { return m_RenderLevel; }
        set {
            if (m_RenderLevel != value)
            {
                m_RenderLevel = value;
                m_RenderLevelDirty = true;
            }
        }
    }

    protected Material renderMat;

    /// <summary>
    /// 独立运作模式 : 当脚本(包含子类)挂载到Camera节点上即开启独立运作模式, 该模式下脚本将直接对Camera本身起作用而不使用GraphicsManager渲染流.
    /// </summary>
    protected bool m_isStandaloneMode = false;

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

        if (!Application.isPlaying) return;

        Camera cam = GetComponent<Camera>();
        m_isStandaloneMode = cam && cam.enabled && !GetComponent<VisualCamera>();
        
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
        RenderImage(source, destination);
    }

    public void RenderImage(RenderTexture src, RenderTexture dst)
    {
        if (renderMat == null || renderMat.shader == null)
            return;
        render(src, dst);
    }

    protected virtual void render(RenderTexture src, RenderTexture dst)
    {
        if (m_RenderLevelDirty)
        {
            if(GraphicsManager.IsInit()) GraphicsManager.Instance.SortEffectComponents();
            m_RenderLevelDirty = false;
        }
    }


}
