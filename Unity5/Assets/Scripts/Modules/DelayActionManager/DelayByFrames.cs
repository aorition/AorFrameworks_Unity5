using UnityEngine;
using System;

public class DelayByFrames : DelayActionBase
{

    public DelayByFrames(int frames, Action action)
        : base("DelayToNextFrame", action)
    {
        _max = frames < 1 ? 1 : frames;
    }

    private int _max;
    private int _ct;

    public override void Init()
    {
        _ct = 0;
    }

    public override void Update()
    {
        _ct++;
        if (_ct >= _max)
        {
            Action tmp = action;
            tmp();
            action = null;
            dead = true;
        }
    }

}