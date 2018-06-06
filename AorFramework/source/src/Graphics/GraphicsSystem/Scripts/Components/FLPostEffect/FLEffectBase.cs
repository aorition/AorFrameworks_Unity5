using Framework.Graphic;
using UnityEngine;

[AddComponentMenu("")] //禁止此脚本放入ComponentMenu中
public class FLEffectBase : MonoBehaviour, IFLPostEffectComponent
{

    public int RenderLevel = 0;
    protected int _RenderLevel;
    protected Material renderMat;
    protected bool _isInit;

    public virtual string ScriptName
    {
        get { return ""; }
    }

    public bool IsActive
    {
        get { return this.enabled && gameObject.activeInHierarchy; }
    }


    public int Level => RenderLevel;

    protected virtual void init()
    {

    }

    protected virtual void OnEnable()
    {
        GraphicsManager.Request(() =>
        {
            init();
            GraphicsManager.Instance.AddPostEffectComponent(this);
            _isInit = true;
        });
    }

    protected virtual void OnDisable()
    {
        if(GraphicsManager.IsInit())
            GraphicsManager.Instance.RemovePostEffectComponent(this);
    }


    public void RenderImage(RenderTexture src, RenderTexture dst)
    {
        if (_RenderLevel != RenderLevel && GraphicsManager.Instance)
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
