using System;
using System.Collections.Generic;
using UnityEngine;

public class RoleGlowSettingAsset : ScriptableObject
{
    [Tooltip("建议数值: 1,2,4,8,16")]
    public int RTSize = 4;
    [Tooltip("建议数值: 0.01,0.02,0.05")]
    public float BlurRadius = 0.02f;
    [Tooltip("建议数值: 0.001,0.002")]
    public float Offset = 0.002f;
    [Tooltip("建议数值: 0.5 - 1.25")]
    public float Power = 0.75f;
}
