using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YoukiaUnity.Graphics;

public class DayNightEffect : MonoBehaviour
{
    public bool isInitalLightness = false;
    public float lightness = 12;
    private UnityEngine.SceneManagement.Scene _currentScene;
    private DayLightSetting daynightSetting;
    private float initLightness;


    void OnEnable()
    {

        Transform sceneObjectRoot = Utils.GetSceneObjectRoot();
        if (sceneObjectRoot)
        {
            daynightSetting = findComponentLoop(sceneObjectRoot);
            GetInitialLightness();    //get lightness
        }
        else
        {
            StartCoroutine(waitForLoadScene(SceneManager.GetActiveScene()));
        }
    }

    void OnDisable()
    {
        if (isInitalLightness)
        {
            daynightSetting.CurrentTimeForDay = initLightness;
            daynightSetting.ApplyTimeInEnvironmentSetting();
        }
        
    }


    IEnumerator waitForLoadScene(UnityEngine.SceneManagement.Scene currentScene)
    {
        while (true)
        {
            if (currentScene.isLoaded)
            {
                linkComponent(currentScene.GetRootGameObjects());
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }


    void linkComponent(GameObject[] rootGameObjects)
    {
        if (rootGameObjects != null && rootGameObjects.Length > 0)
        {
            int i, len = rootGameObjects.Length;
            for (i = 0; i < len; i++)
            {
                daynightSetting = findComponentLoop(rootGameObjects[i].transform);
                GetInitialLightness();    //get lightness
                break;
            }
        }
    }

    //find DayLightSetting
    DayLightSetting findComponentLoop(Transform t)
    {
        DayLightSetting cp = t.GetComponent<DayLightSetting>();
        if (!cp)
        {
            if (t.childCount > 0)
            {
                int i, len = t.childCount;
                for (i = 0; i < len; i++)
                {
                    Transform st = t.GetChild(i);
                    cp = findComponentLoop(st);
                    if (cp)
                    {
                        return cp;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        return cp;
    }



    void FixedUpdate()
    {
        if (daynightSetting)
        {
            daynightSetting.CurrentTimeForDay = lightness;
            daynightSetting.ApplyTimeInEnvironmentSetting();
        }
        
    }


    //记录最初时间值
    void GetInitialLightness()
    {
        if (daynightSetting)
        {
            if (isInitalLightness)
            {
                initLightness = daynightSetting.CurrentTimeForDay;
                //Debug.Log(initLightness);
            }
        }
    }

}
