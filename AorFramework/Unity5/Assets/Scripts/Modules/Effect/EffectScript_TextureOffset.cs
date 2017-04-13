using UnityEngine;
using System;

public class EffectScript_TextureOffset : MonoBehaviour, ISimulateAble
{

    public float SpeedX = 0.25f;
    public float SpeedY = 0.25f;
    private Vector2 offset;
    private float _time;

    private Renderer _Render;
    public Renderer Render
    {
        get
        {
            if (_Render == null)
            {
                _Render = GetComponent<Renderer>();
            }
            return _Render;


        }

    }


    void OnEnable()
    {
        _time = 0;
    }

    public void Process(float time)
    {
        offset.x = time * (-SpeedX);
        offset.y = time * (-SpeedY);

        if (Application.isPlaying)
        {

            Render.material.mainTextureOffset = offset;
        }
        else
        {
            Render.sharedMaterial.mainTextureOffset = offset;
        }

    }

    void Update()
    {
        _time += Time.deltaTime;
        Process(_time);

    }
}


