using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;


/// <summary>
/// 选关场景天气变化效果
/// </summary>
public class SceneSkyChangeEffectAnim : MonoBehaviour
{
    public float timeValue = 12;

    private Animator anim;
    private DayLightSetting daynightSetting;


    void Start()
    {
        daynightSetting = GetComponent<DayLightSetting>();
        anim = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        if (daynightSetting && daynightSetting.CurrentTimeForDay != timeValue)
        {
            daynightSetting.CurrentTimeForDay = timeValue;
            daynightSetting.ApplyTimeInEnvironmentSetting();
        }

    }



    /// <summary>
    /// 切换到白天
    /// </summary>
    public void ChangeToDay()
    {
        if (anim)
        {
            if (HasAnimState("day"))
            {
                RainToDay();
                DuckToDay();
            }
        }
    }
    /// <summary>
    /// 切换到雨天
    /// </summary>
    public void ChangeToRain()
    {
        if (anim)
        {
            if (HasAnimState("rain"))
            {
                DayToRain();
                DuckToRain();
            }
        }
    }
    /// <summary>
    /// 切换到黄昏
    /// </summary>
    public void ChangeToDuck()
    {
        if (anim)
        {
            if (HasAnimState("duck"))
            {
                DayToDuck();
                RainToDuck();
            }
        }
    }


    //白天变雨天
    void DayToRain()
    {
        if (HasAnimState("day") && anim.GetInteger("dayChange") == 0)
        {
            anim.SetInteger("dayChange", 1);
        }
    }
    //雨天变白天
    void RainToDay()
    {
        if (HasAnimState("rain") && anim.GetInteger("dayChange") == 1)
        {
            anim.SetInteger("dayChange", 0);
        }
    }
    //白天变黄昏
    void DayToDuck()
    {
        if (HasAnimState("day") && anim.GetInteger("dayChange") == 0)
        {
            anim.SetInteger("dayChange", 2);
        }
    }
    //黄昏变白天
    void DuckToDay()
    {
        if (HasAnimState("duck") && anim.GetInteger("dayChange") == 2)
        {
            anim.SetInteger("dayChange", 0);
        }
    }
    //黄昏变雨天
    void DuckToRain()
    {
        if (HasAnimState("duck") && anim.GetInteger("dayChange") == 2)
        {
            anim.SetInteger("dayChange", 1);
        }
    }
    //雨天变黄昏
    void RainToDuck()
    {
        if (HasAnimState("rain") && anim.GetInteger("dayChange") == 1)
        {
            anim.SetInteger("dayChange", 2);
        }
    }

    //判断是否有指定的状态机
    bool HasAnimState(string stateName)
    {
        bool returnValue = false;
        if (anim)
        {
            for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
            {
                if (anim.runtimeAnimatorController.animationClips[i].name.Contains(stateName))
                    returnValue = true;
            }
        }
        return returnValue;
    }


}
