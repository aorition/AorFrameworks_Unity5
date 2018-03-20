using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using YoukiaCore;
using YoukiaUnity.View;

//冲击波控制器

public class WaveController : EffectDescript
{

    //结束动画播放时间
    public float EndAnimPlayDelay;
    private float _EndAnimPlayDelay;
 
    //bo头
    public Transform WaveHeader;
    //bo根部
    public Transform WaveBase;
    //  public Transform ScaleBody;

    public Animator EndAnim;

    //模型长度和uv比值,默认1米对应1uv
    private float UvScale = 1;
 
    protected  void OnEnable()
    {
        reset();
    }

    void reset()
    {
        _EndAnimPlayDelay = EndAnimPlayDelay;
        if (WaveHeader != null)
        {
            WaveHeader.transform.localPosition = Vector3.zero;
        }

        if (EndAnim != null)
        {
            EndAnim.Play("normal", 0);
        }


    //    ScaleBody.transform.localScale = Vector3.one;
    }

 
    protected  void LateUpdate()
    {
 
    //    ScaleBody.transform.LookAt(WaveHeader);
        _EndAnimPlayDelay -= Time.deltaTime;

        if (_EndAnimPlayDelay <= 0 && EndAnim != null && EndAnim.GetCurrentAnimatorStateInfo(0).IsName("normal"))
        {
            EndAnim.Play("end");
        }
        if (WaveBase != null && WaveHeader != null)
        {
            WaveBase.forward = WaveHeader.forward;
        }
    }
}
