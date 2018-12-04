using UnityEngine;
using System.Collections;
using System;

public class DelayToNextFrame : DelayActionBase
{

    public DelayToNextFrame(Action action) 
        : base("DelayToNextFrame", action)
    {
        //
    }
    private bool _init;
    public override void Init()
    {
        _init = true;
    }

    public override void Update()
    {
        if (_init) {
            _init = false;
            return;
        }

        Action tmp = action;
        tmp();
        action = null;
        dead = true;
    }

}
