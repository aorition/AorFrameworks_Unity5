using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;


public class SceneSkyChangeController : MonoBehaviour
{
    public GameObject daySky;    //白天  12
    public GameObject duckSky;    //黄昏  17,18,19
    public GameObject settingSunSky;    //夕阳  20
    public GameObject rainSky;    //雨天    4

    //public float settingSunRockLight;
    //public Color settingSunRockColor;
    //public Material[] rockMaterials;

    private DayLightSetting daynightSetting;
    private float timeValue;

    void Awake()
    {
        daynightSetting = GetComponent<DayLightSetting>();

        //if (rockMaterials.Length > 0)
        //{
        //    for (int i = 0; i < rockMaterials.Length; i++)
        //    {
        //        rockMaterials[i].shader = Shader.Find("Custom/NoLight/Unlit - Object##");
        //        rockMaterials[i].SetColor("_Color", Color.white);
        //        rockMaterials[i].SetFloat("_Lighting", 1);
        //    }
        //}
    }


    /// <summary>
    /// 改变战斗场景天空
    /// </summary>
    public void ChangeBattleSceneSky()
    {
        if (daynightSetting)
            timeValue = daynightSetting.CurrentTimeForDay;

        if (daySky && duckSky && settingSunSky && rainSky)
        {
            if (timeValue == 20)
            {
                //change sky : settingSun
                settingSunSky.SetActive(true);
                duckSky.SetActive(false);
                daySky.SetActive(false);
                rainSky.SetActive(false);

                //set material value : point light
                //if (rockMaterials.Length > 0)
                //{
                //    for (int i = 0; i < rockMaterials.Length; i++)
                //    {
                //        rockMaterials[i].shader = Shader.Find("Custom/Light/Diffuse - Toon - SimpleObject");
                //        rockMaterials[i].SetColor("_Color", settingSunRockColor);
                //        rockMaterials[i].SetFloat("_Lighting", settingSunRockLight);
                //        rockMaterials[i].SetFloat("_SpPower", 0);
                //    }
                //}
            } 
            else
            {
                settingSunSky.SetActive(false);

                //set material value : no light
                //if (rockMaterials.Length > 0)
                //{
                //    for (int i = 0; i < rockMaterials.Length; i++)
                //    {
                //        rockMaterials[i].shader = Shader.Find("Custom/NoLight/Unlit - Object##");
                //        rockMaterials[i].SetColor("_Color", Color.white);
                //        rockMaterials[i].SetFloat("_Lighting", 1);
                //    }
                //}

                //change sky : rain
                if (timeValue == 4)
                {
                    rainSky.SetActive(true);
                    duckSky.SetActive(false);
                    daySky.SetActive(false);
                }
                else
                {
                    //change sky : day
                    if (timeValue >= 1 && timeValue <= 16)
                    {
                        daySky.SetActive(true);
                        duckSky.SetActive(false);
                        rainSky.SetActive(false);
                    }
                    //change sky : duck
                    else
                    {
                        duckSky.SetActive(true);
                        rainSky.SetActive(false);
                        daySky.SetActive(false);
                    }
                }    
            }
        }

    }


}
