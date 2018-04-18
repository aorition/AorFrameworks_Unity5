using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class EffectScript_EffectDelayRun : MonoBehaviour, ICustomTimeScale
{
    public float DelayTime = 0;

    private float _timePassed;
    private bool _isEnable = false;

    protected void Awake()
    {
        OnEnable();
    }

    protected void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        _timePassed = 0;
        _isEnable = false;
    }

    protected void Update()
    {
        if (!Pause)
        {
            Update(Time.deltaTime);
        }
    }

    public bool Pause { get; set; }

    public void Update(float tpf)
    {
        if (!_isEnable)
        {
            _timePassed += tpf;
            if (_timePassed > DelayTime)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                _isEnable = true;
            }
        }
    }
}
