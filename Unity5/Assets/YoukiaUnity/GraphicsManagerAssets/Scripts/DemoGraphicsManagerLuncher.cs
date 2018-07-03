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
                ResourcesLoadBridge.Load<PostProcessingProfile>("data/DemoPPAssetData", (so,param) =>
                {
                    if (so)
                    {
                        ppb.profile = so;
                    }
                });
            }
        };
    }

    protected override void onManagerAfterInitialization()
    {
        
        if (_GlowEffectSetting)
        {
            GraphicsManager.Instance.RegisterSubSettingData<RoleGlowSettingAsset>(_GlowEffectSetting);
        }

        if (_ShadowEffectSetting)
        {
            GraphicsManager.Instance.RegisterSubSettingData<RoleShadowSettingAsset>(_ShadowEffectSetting);
            if (_ShadowEffectSetting.Enable)
            {
                RoleShadowEffectCamera.Create();
            }
        }
    }

}
