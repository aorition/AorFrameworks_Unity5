using System;
using System.Collections.Generic;
using UnityEngine;

public class RoleShadowSettingAsset : ScriptableObject
{

    public bool Enable = true;

    public int RTSize = 2;

    public float ShadowOffsetX = 0.25f;
    public float ShadowOffestY = 0.4f;
    public float ShadowOffestW = 0.2f;

    public float PxThreshold = 0.01f;
    public float ShadowPower = 0.45f;

    public bool EnableBlur = true;

    public float ShadowBlurRadius = 0.02f;

}