using UnityEngine;
using System.Collections;
using System;

public class SimpleLoopAction : DelayActionBase
{

    public SimpleLoopAction(Action action, float inverval)
        : base("SimpleLoopAction", action)
    {
        this.inverval = inverval;
    }
    private float lastRunTime = 0;
    private float inverval = 0;
    public override void Init()
    {
        lastRunTime = Time.realtimeSinceStartup;
    }

    public override void Update()
    {

        //float curTime = Time.realtimeSinceStartup;
        //while (curTime > lastRunTime + inverval)
        //{
        //    action();
        //    lastRunTime += inverval;
        //}

        float curTime2 = Time.realtimeSinceStartup - lastRunTime;
        if (curTime2 >= inverval) {
            lastRunTime = Time.realtimeSinceStartup;
            action();
        }

    }

}