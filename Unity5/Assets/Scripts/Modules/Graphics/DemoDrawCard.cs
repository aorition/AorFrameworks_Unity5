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


    public bool UseBloom
    {
        set
        {
            if (_UseBloom == value)
                return;

            _UseBloom = value;

            if (!value)
            {
                mat.SetTexture("_CurveTex", Texture2D.blackTexture);

                if (smallRt != null)
                    smallRt.Release();
                if (rt2 != null)
                    rt2.Release();


            }


        }

        get { return _UseBloom; }

    }

    private bool _UseBloom;


    [Range(0.0f, 1f)]
    public float whiteBalance = 0f;

    [Range(0.0f, 1.5f)]
    public float threshhold = 0.2f;


    [Range(0.0f, 2.5f)]
    public float intensity = 0.75f;

    [Range(0.25f, 5.5f)]
    public float blurSize = 3f;
    public Resolution resolution = Resolution.Low;

    [Range(1, 4)]
    public int blurIterations = 1;
    public Shader fastBloomShader;

    Material fastBloomMat;
    private RenderTexture smallRt;
    private RenderTexture rt2;

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
        smallRt = null;
        rt2 = null;
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
                obj.SetTexture("_MainTex", MainRtCopy);
        }

    }

/// <summary>
/// 移除一个需求主RT拷贝的物体
/// </summary>
/// <param name="obj">物体</param>
public void RemoveNeedBlitMainRtCopyObj(Material obj)
{
    NeedBlitMainRtCopyObjs.Remove(obj);
}


public DemoDrawCard()
{
    fastBloomShader = Shader.Find("Hidden/PostEffect/FastBloom");
    fastBloomMat = new Material(fastBloomShader);
    NeedBlitMainRtCopyObjs.Clear();
}

//半透明物体绘制完成后,拷贝一份 mainRt 做空气扭曲
public override void OnPreEffectAfter(RenderTexture mainRt)
{



    if (NeedBlitMainRtCopyObjs.Count > 0)
    {

        if (_MainRtCopy == null)
        {
            _MainRtCopy = new RenderTexture(mainRt.width, mainRt.height, 0, mainRt.format);

            foreach (Material each in NeedBlitMainRtCopyObjs)
            {
                each.SetTexture("_MainTex", MainRtCopy);
            }
        }

        Graphics.Blit(mainRt, _MainRtCopy);
    }


}

private RenderTextureFormat rtFormat;
public float middleGrey = 0.4f;
public float adaptionSpeed = 4f;
private Material tonemapMaterial;


private RenderTexture[] lumRts;
private RenderTexture avglumRt;


public bool _Hdr = false;
public bool Hdr
{
    get { return _Hdr; }
    set
    {
        _Hdr = value;


        if (!GraphicsManager.GetInstance().isSupportHdr())
        {
            _Hdr = false;
        }

        if (_Hdr == false)
        {
                GraphicsManager.GetInstance().SetDrawPassCount(0);
            clearHdrRt();
        }
        else
        {

                GraphicsManager.GetInstance().SetDrawPassCount(1);

        }

    }


}


void clearHdrRt()
{
    if (lumRts != null && lumRts.Length > 0)
    {
        for (int i = 0; i < lumRts.Length; i++)
        {
            if (lumRts[i] != null)
            {
                lumRts[i].Release();
                lumRts[i] = null;
            }
        }

    }

    lumRts = null;


}
bool CreateInternalRenderTexture()
{
    if (avglumRt != null)
    {
        return false;
    }
    rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf) ? RenderTextureFormat.RGHalf : RenderTextureFormat.ARGBHalf;
    avglumRt = new RenderTexture(1, 1, 0, rtFormat);
    avglumRt.hideFlags = HideFlags.DontSave;
    return true;
}

//求画面平均亮度
void avgLuminance(RenderTexture destination)
{

    if (!tonemapMaterial)
        tonemapMaterial = new Material(Shader.Find("Hidden/Tonemapper"));


    bool freshlyBrewedInternalRt = CreateInternalRenderTexture();

    if (lumRts == null)
    {
        lumRts = new RenderTexture[2]{
                new RenderTexture(2, 2, 0, rtFormat),
            //     new RenderTexture(4, 4, 0,rtFormat),
            //      new RenderTexture(8,8,0, rtFormat),
                 new RenderTexture(16,16,0, rtFormat)};
    }

    Graphics.Blit(destination, lumRts[1]);
    RenderTexture lumRt = lumRts[0];
    //第一个sample的平均亮度
    Graphics.Blit(lumRts[1], lumRts[0], tonemapMaterial, 1);

    //downSample
//    for (int i = 2; i > 0; i--)
//    {
//        Graphics.Blit(lumRts[i], lumRts[i - 1]);
//        lumRt = lumRts[i - 1];
//    }


    adaptionSpeed = adaptionSpeed < 0.001f ? 0.001f : adaptionSpeed;
    tonemapMaterial.SetFloat("_AdaptionSpeed", adaptionSpeed);

    //第一次写亮度平均值用3:fragExp 2:fragDownsample
    //  Graphics.Blit(lumRt, avglumRt, tonemapMaterial, freshlyBrewedInternalRt ? 3 : 2);
    Graphics.Blit(lumRt, avglumRt, tonemapMaterial, freshlyBrewedInternalRt ? 3 : 2);

    middleGrey = middleGrey < 0.001f ? 0.001f : middleGrey;
    GraphicsManager.GetInstance().RenderMaterial.SetVector("_HdrParams", new Vector4(middleGrey, whiteBalance, middleGrey,1));
    GraphicsManager.GetInstance().RenderMaterial.SetTexture("_SmallTex", avglumRt);

    //   Graphics.Blit(LuminanceSource, destination, tonemapMaterial, 0);


}





public override void OnPostEffectRenderAfter(RenderTexture mainRt)
{
    base.OnPostEffectRenderAfter(mainRt);

    if (Hdr)
    {

        avgLuminance(mainRt);
    }


    if (!_UseBloom)
    {
        if (!isBlackTex)
        {
            mat.SetTexture("_CurveTex", Texture2D.blackTexture);
            isBlackTex = true;
        }

        return;
    }




    float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

    fastBloomMat.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshhold, intensity));

    if (!smallRt)
    {
        int divider = resolution == Resolution.Low ? 4 : 2;
        var rtW = mainRt.width / divider;
        var rtH = mainRt.height / divider;

        // downsample
        if (smallRt == null)
        {
            smallRt = new RenderTexture(rtW, rtH, 0, mainRt.format);
            smallRt.filterMode = FilterMode.Bilinear;
        }
    }

    Graphics.Blit(mainRt, smallRt, fastBloomMat, 0);



    for (int i = 0; i < blurIterations; i++)
    {
        fastBloomMat.SetVector("_Parameter", new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, threshhold, intensity));


        // vertical blur
        if (rt2 == null)
        {
            rt2 = new RenderTexture(smallRt.width, smallRt.height, 0, mainRt.format);
            rt2.filterMode = FilterMode.Bilinear;
        }

        Graphics.Blit(smallRt, rt2, fastBloomMat, 1);


        // horizontal blur
        Graphics.Blit(rt2, smallRt, fastBloomMat, 2);

    }

    if (mat != null)
    {
        mat.SetTexture("_CurveTex", smallRt);
        isBlackTex = false;
    }


}

public override void OnBlitEnd()
{

    //  RenderTexture.ReleaseTemporary(smallRt);
}

}
