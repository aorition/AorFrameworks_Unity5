using UnityEngine;
using YoukiaUnity.Graphics;


public class DistortionSupport : GraphicsLauncher
{
    private DemoDrawCard card;
    private bool hasRt;

    protected override void Launcher()
    {
        base.Launcher();
        card = GraphicsManager.GetInstance().DrawCard as DemoDrawCard;
        card.AddNeedBlitMainRtCopyObj(gameObject.GetComponent<Renderer>().material);


    }


    void Update()
    {

        if (!hasRt && GraphicsManager.isInited)
        {
            gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", card.MainRtCopy);
            hasRt = true;
        }
    }

}

