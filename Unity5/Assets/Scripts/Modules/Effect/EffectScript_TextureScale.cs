using UnityEngine;
using System.Collections.Generic;
public class EffectScript_TextureScale : MonoBehaviour, ISimulateAble
{

    public AnimationCurve curveX;
    public AnimationCurve curveY;

    private List<Vector2> offsetArray;
    private float secPerFrame;
    private float mTime = 0;
    public Material mat;

    public void OnEnable()
    {
        mTime = 0;
        if(mat==null)
        mat = gameObject.GetComponent<Renderer>().material;
    }

    public void Process(float time)
    {
        if (mat==null)
            return;


            float x = curveX.Evaluate(mTime);

        float y = curveY.Evaluate(mTime);

        mat.mainTextureScale=new Vector2(x,y);
        mat.mainTextureOffset = new Vector2(-(x/2-0.5f), -(y / 2 - 0.5f));
    }



    void Update()
    {
        mTime += Time.deltaTime;
        Process(mTime);

    }

 
}

