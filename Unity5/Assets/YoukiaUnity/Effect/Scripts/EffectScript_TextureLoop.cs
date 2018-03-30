using UnityEngine;
using System.Collections.Generic;
public class EffectScript_TextureLoop : MonoBehaviour, ISimulateAble
{

    public int tileX = 1;
    public int tileY = 1;
    public int framerate = 16;
    public int Count = 0;
    private int _count = 0;
    private List<Vector2> offsetArray;
    private float secPerFrame;
    private float cd = 0;
    private Renderer _render;
    private float _time;
    private int _currentIndex = 0;


    void Awake()
    {
        _render = GetComponent<Renderer>();
    }

    private bool _isStarted = false;

    void OnEnable()
    {
        _count = 0;
        _currentIndex = 0;
        _time = 0;
        oldindex = -1;
    }

    void Start()
    {
        
        int i;
        int j;
        offsetArray = new List<Vector2>();
        secPerFrame = 1 / (float)framerate;
        if (Application.isPlaying)
            _render.material.SetTextureScale("_MainTex", new Vector2(1 / (float)tileX, 1 / (float)tileY));
        else
            _render.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1 / (float)tileX, 1 / (float)tileY));


        for (j = 0; j < tileY; j++)
        {
            for (i = 0; i < tileX; i++)
            {

                offsetArray.Add(new Vector2(i / (float)tileX, 1 - (j + 1) / (float)tileY));
            }
        }

        _isStarted = true;
    }


    void Update()
    {
        if (_render == null)
            return;

        Process(_time);
        _time += Time.deltaTime;
    }

    private int oldindex = -1;
    public void Process(float time)
    {

        if (!_isStarted)
        {
            Awake();
            Start();
        }

        if (_count > Count)
        {
            return;
        }


        _currentIndex = (int)(time / secPerFrame) % offsetArray.Count;

        if (oldindex != _currentIndex)
        {
            oldindex = _currentIndex;



            if (_currentIndex == 0 && Count > 0)
            {
                _count++;
                return;
            }



            if (Application.isPlaying)
                _render.material.SetTextureOffset("_MainTex", offsetArray[_currentIndex]);
            else
                _render.sharedMaterial.SetTextureOffset("_MainTex", offsetArray[_currentIndex]);
        }






    }

}

