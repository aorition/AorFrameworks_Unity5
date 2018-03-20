using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;


public class DemoDrawCard : BaseDrawCard
{
    public enum Resolution
    {
        Low = 0,
        High = 1,
    }

    public bool _UseBloom;
    public void BloomCheck()
    {

        if (!GraphicsManager.GetInstance().EnableBloom)
        {
            _UseBloom = false;
            setPostEffectDraw(postEffectDraw.enabled);
            //  GraphicsManager.GetInstance().FinalCamera.GetComponent<PostEffectDraw>().enabled = false;
            mat.SetTexture("_CurveTex", Texture2D.blackTexture);

            if (smallRt != null)
            {
                smallRt.Release();
                smallRt = null;
            }


            if (rt2 != null)
            {
                rt2.Release();
                rt2 = null;
            }

            return;
        }
        else
        {
            _UseBloom = true;
            setPostEffectDraw(true);
        }


    }


    //辉光阀
    [Range(0.0f, 5f)]
    public float BlurThreshhold = 2.8f;

    //辉光强度
    [Range(0.0f, 2.5f)]
    public float BlurIntensity = 0.1f;

    //发光范围
    [Range(0.25f, 5.5f)]
    public float blurSize = 3f;
    public Resolution resolution = Resolution.Low;

    //blur级别
    [Range(1, 4)]
    public int blurIterations = 2;
    public Shader fastBloomShader;

    Material fastBloomMat;
    private RenderTexture smallRt;
    private RenderTexture rt2;
    private RenderTexture frt;

    //主Rt等大
    private RenderTexture _MainRtCopy;
    public RenderTexture MainRtCopy
    {

        get { return _MainRtCopy; }
    }

    /// <summary>
    /// 需要主rt的拷贝的物体
    /// </summary>
    private HashSet<Material> NeedBlitMainRtCopyObjs = new HashSet<Material>();
    private bool isBlackTex;
    
    /// <summary>
    /// 清理部分渲染贴图,下次再用会重新创建
    /// </summary>
    public void Clear()
    {
        //frt = null;
        RenderTexture.ReleaseTemporary(frt);
        //smallRt = null;
        RenderTexture.ReleaseTemporary(smallRt);
        //        rt2 = null;
        RenderTexture.ReleaseTemporary(rt2);
        _MainRtCopy = null;
        NeedBlitMainRtCopyObjs.Clear();
    }

    /// <summary>
    /// 添加一个需求主RT拷贝的物体
    /// </summary>
    /// <param name="obj">物体</param>
    public void AddNeedBlitMainRtCopyObj(Material obj)
    {
        if (!NeedBlitMainRtCopyObjs.Contains(obj))
        {
            NeedBlitMainRtCopyObjs.Add(obj);

            if (MainRtCopy != null)
                obj.mainTexture = MainRtCopy;

            setEffectDraw(true);
        }

    }

    /// <summary>
    /// 移除一个需求主RT拷贝的物体
    /// </summary>
    /// <param name="obj">物体</param>
    public void RemoveNeedBlitMainRtCopyObj(Material obj)
    {
        NeedBlitMainRtCopyObjs.Remove(obj);

        if (NeedBlitMainRtCopyObjs.Count == 0)
        {
            setEffectDraw(false);
        }
    }


    public DemoDrawCard()
    {
        fastBloomShader = Shader.Find("Hidden/PostEffect/FastBloom");
        fastBloomMat = new Material(fastBloomShader);
        NeedBlitMainRtCopyObjs.Clear();

    }

    //半透明物体绘制完成后,拷贝一份 mainRt 做空气扭曲
    public override void OnEffectRenderAfter(ref RenderTexture mainRt)
    {
        _MainRtCopy = mainRt;
    }
    
    public override void OnPostEffectRenderAfter(ref RenderTexture mainRt)
    {
        if (mainRt == null)
        {
            return;
        }

        if (!_UseBloom)
        {
            if (!isBlackTex)
            {
                GetMaterial().SetTexture("_CurveTex", Texture2D.blackTexture);
                isBlackTex = true;
            }

            base.OnPostEffectRenderAfter(ref mainRt);

            return;
        }

        float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

        fastBloomMat.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, BlurThreshhold, BlurIntensity));

        if (!smallRt)
        {
            int divider = resolution == Resolution.Low ? 4 : 2;
            var rtW = mainRt.width / divider;
            var rtH = mainRt.height / divider;

            // downsample
            if (smallRt == null)
            {
//                smallRt = new RenderTexture(rtW, rtH, 0, mainRt.format);
                smallRt = RenderTexture.GetTemporary(rtW, rtH, 0, mainRt.format);
                smallRt.filterMode = FilterMode.Bilinear;
            }
        }

        Graphics.Blit(mainRt, smallRt, fastBloomMat, 0);

        for (int i = 0; i < blurIterations; i++)
        {
            fastBloomMat.SetVector("_Parameter", new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, BlurThreshhold, BlurIntensity));
            
            if (rt2 == null)
            {
//                rt2 = new RenderTexture(smallRt.width, smallRt.height, 0, mainRt.format);
                rt2 = RenderTexture.GetTemporary(smallRt.width, smallRt.height, 0, mainRt.format);
                rt2.filterMode = FilterMode.Bilinear;
            }

            // vertical blur
            Graphics.Blit(smallRt, rt2, fastBloomMat, 1);

            // horizontal blur
            Graphics.Blit(rt2, smallRt, fastBloomMat, 2);
            
        }

        //IOS 辉光颠倒问题

        //5.6 版本后，Android 也存在此问题

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {

            if (frt == null)
            {
                frt = RenderTexture.GetTemporary(smallRt.width, smallRt.height, 0);
            }

            Graphics.Blit(smallRt, frt, reverseMat);
            Graphics.Blit(frt, smallRt);
        }

        if (mat != null)
        {
            mat.SetTexture("_CurveTex", smallRt);
            isBlackTex = false;
        }

        base.OnPostEffectRenderAfter(ref mainRt);
    }

    public override void OnBlitEnd()
    {

        //  RenderTexture.ReleaseTemporary(smallRt);
    }

    public override void OnSettingUpdate()
    {
        BloomCheck();
    }
}
