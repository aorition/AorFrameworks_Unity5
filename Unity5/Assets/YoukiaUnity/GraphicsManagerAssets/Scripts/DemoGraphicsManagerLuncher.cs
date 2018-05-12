using Framework;
using Framework.Graphic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DemoGraphicsManagerLuncher : GraphicsManagerLuncher {

    [SerializeField]
    private RoleGlowSettingAsset _GlowEffectSetting;

    [SerializeField]
    private RoleShadowSettingAsset _ShadowEffectSetting;

    protected override void onManagerBeforeInitialization()
    {
        GraphicsManager.Instance.OnSubCameraInited += (cam, info) =>
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

    protected override void onManagerAfterInitialization()
    {
        
        if (_GlowEffectSetting)
        {
            GraphicsManager.Instance.registerSubSettingData<RoleGlowSettingAsset>(_GlowEffectSetting);
        }

        if (_ShadowEffectSetting)
        {
            GraphicsManager.Instance.registerSubSettingData<RoleShadowSettingAsset>(_ShadowEffectSetting);
            if (_ShadowEffectSetting.Enable)
            {
                RoleShadowEffectCamera.Create();
            }
        }
    }

}
