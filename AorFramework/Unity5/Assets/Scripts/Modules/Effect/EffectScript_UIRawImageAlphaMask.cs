using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 此类必须配合 RawImage-AlphaMask##.shader 系列使用，实现alphaMask的UV旋转，平移，缩放做动画
/// </summary>
[RequireComponent(typeof(RawImage))]
public class EffectScript_UIRawImageAlphaMask : MonoBehaviour
{
    [Tooltip("主图 UV 的缩放")]
    public Vector2 Main_Tiling = Vector2.one;
    [Tooltip("主图 UV 的偏移")]
    public Vector2 Main_Offset = Vector2.zero;
    [Tooltip("主图 UV 的旋转")]
    public float Main_Rotation = 0;

    [Tooltip("Mask UV 的缩放")]
    public Vector2 Mask_Tiling = Vector2.one;
    [Tooltip("Mask UV 的偏移")]
    public Vector2 Mask_Offset = Vector2.zero;
    [Tooltip("Mask UV 的旋转")]
    public float Mask_Rotation = 0;

    private Material _targetMaterial;
    void Awake()
    {
        RawImage image = GetComponent<RawImage>();
        _targetMaterial = image.materialForRendering;

    }

    void FixedUpdate()
    {

        _targetMaterial.SetTextureScale("_MainTex", Main_Tiling);
        _targetMaterial.SetTextureOffset("_MainTex", Main_Offset);
        _targetMaterial.SetFloat("_Main_Rotation", Main_Rotation);

        _targetMaterial.SetTextureScale("_MaskTex", Mask_Tiling);
        _targetMaterial.SetTextureOffset("_MaskTex", Mask_Offset);
        _targetMaterial.SetFloat("_Mask_Rotation", Mask_Rotation);
    }
}