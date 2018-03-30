using AorFrameworks;
using Framework.Graphic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DemoGraphicsManagerLuncher : GraphicsManagerLuncher {

    [SerializeField]
    private RoleGlowSettingAsset _GlowEffectSetting;

    [SerializeField]
    private RoleShadowSettingAsset _ShadowEffectSetting;

    protected override void onGraphicsManagerInstanced()
    {
        GraphicsManager.instance.OnSubCameraInited += (cam, info) =>
        {
            if (info.type == SubGCamType.FinalOutput)
            {
                PostProcessingBehaviour ppb = cam.gameObject.AddComponent<PostProcessingBehaviour>();
                ResourcesLoadBridge.LoadScriptableObject("data/DemoPPAssetData", so =>
                {
                    if (so)
                    {
                        ppb.profile = so as PostProcessingProfile;
                    }
                });
            }
        };
    }

    protected override void onGraphicsManagerSetuped()
    {
        if (_GlowEffectSetting)
        {
            GraphicsManager.instance.registerSubSettingData<RoleGlowSettingAsset>(_GlowEffectSetting);
        }

        if (_ShadowEffectSetting)
        {
            GraphicsManager.instance.registerSubSettingData<RoleShadowSettingAsset>(_ShadowEffectSetting);
            if (_ShadowEffectSetting.Enable)
            {
                RoleShadowEffectCamera.Create();
            }
        }
    }

}
