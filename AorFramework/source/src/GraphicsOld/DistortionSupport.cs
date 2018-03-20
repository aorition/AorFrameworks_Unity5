using UnityEngine;
using YoukiaUnity.Graphics;


public class DistortionSupport : GraphicsLauncher
{
    private DemoDrawCard card;
    private bool hasRt;
    private Material mat;

    protected override void Launcher()
    {
        base.Launcher();
        card = GraphicsManager.GetInstance().DrawCard as DemoDrawCard;
        if (card == null)
        {
            card = new DemoDrawCard();
            GraphicsManager.GetInstance().DrawCard = card;
        }
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (!hasRt)
        {
            if (GraphicsManager.GetInstance().DrawCard!=null && (GraphicsManager.GetInstance().DrawCard as DemoDrawCard).MainRtCopy != null)
            {
                mat.mainTexture = (GraphicsManager.GetInstance().DrawCard as DemoDrawCard).MainRtCopy;
                hasRt = true;
            }
        }

    }

    private void OnEnable()
    {
        card.AddNeedBlitMainRtCopyObj(mat);
    }

    private void OnDisable()
    {
        hasRt = false;
        card.RemoveNeedBlitMainRtCopyObj(mat);
    }



}

