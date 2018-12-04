using UnityEngine;
using System;

public class DelayBySeconds : DelayActionBase
{

    public DelayBySeconds(float seconds, Action action)
        : base("DelayBySeconds", action)
    {
        _start = Time.realtimeSinceStartup;
        _max = seconds < 0.1f ? 0.1f : seconds;
    }

    private float _start;
    private float _max;

    public override void Update()
    {
        if (Time.realtimeSinceStartup - _start >= _max)
        {
            Action tmp = action;
            tmp();
            action = null;
            dead = true;
        }
    }
}
