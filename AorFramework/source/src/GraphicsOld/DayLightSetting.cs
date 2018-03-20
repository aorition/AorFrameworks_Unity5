using System;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Scene;

namespace YoukiaUnity.Graphics
{

    public class DayLightSetting : MonoBehaviour
    {

        public EnvironmentSetting LinkEnvironmentSetting;

        public float CurrentTimeForDay = 9f;

        public Gradient AmbientColor;

        public Gradient SunLightColor;
        public AnimationCurve SunLightIntensity = AnimationCurve.EaseInOut(0, 1f, 1f, 1f);

        public bool FogEnable = true;
        public Gradient FogColor;
        public AnimationCurve FogDensity = AnimationCurve.EaseInOut(0, 0.01f, 1f, 0.01f);
        public AnimationCurve FogStart = AnimationCurve.EaseInOut(0, 100f, 1f, 100f);
        public AnimationCurve FogEnd = AnimationCurve.EaseInOut(0, 500f, 1f, 500f);

        private SceneSkyChangeController sceneSkyChangeController;

        void Awake()
        {
            sceneSkyChangeController = GetComponent<SceneSkyChangeController>();
        }


        public virtual void ApplyTimeInEnvironmentSetting()
        {
            if(!LinkEnvironmentSetting) return;

            CurrentTimeForDay = Mathf.Clamp(CurrentTimeForDay, 0, 24f);
            float fp = CurrentTimeForDay/24.0f;

            LinkEnvironmentSetting.AmbientColor = AmbientColor.Evaluate(fp);

            LinkEnvironmentSetting.LightColor = SunLightColor.Evaluate(fp);
            LinkEnvironmentSetting.LightIntensity = SunLightIntensity.Evaluate(fp);

            if (FogEnable)
            {
                LinkEnvironmentSetting.FogEnable = FogEnable;
                LinkEnvironmentSetting.FogColor = FogColor.Evaluate(fp);
                LinkEnvironmentSetting.FogDensity = FogDensity.Evaluate(fp);
                LinkEnvironmentSetting.FogStart = FogStart.Evaluate(fp);
                LinkEnvironmentSetting.FogEnd = FogEnd.Evaluate(fp);
            }

            if (Application.isPlaying)
            {
                LinkEnvironmentSetting.EnvironmentUpdate(true);
                if(sceneSkyChangeController)
                    sceneSkyChangeController.ChangeBattleSceneSky();
            }
        }

    }
}
