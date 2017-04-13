//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//按UV Y方向做半透明过度 可反向
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-UVMask##" {
	//@@@DynamicShaderTitleRepaceEnd

	Properties
	{
		[PerRendererData]_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
	_mask("Mask", Range(0,1)) = 0
		_range("Range", Range(0.01,1)) = 0
		[MaterialToggle] _reverse("MaskReverse", Float) = 0
	}

		SubShader
	{
		LOD 200

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		Pass
	{
		Lighting Off
		Fog{ Mode Off }
		Offset -1, -1
		//@@@DynamicShaderBlendRepaceStart
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		//@@@DynamicShaderBlendRepaceEnd
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag			
#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _color;
	float _mask;
	float _reverse;
	float _range;

	struct appdata_t
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	v2f o;

	v2f vert(appdata_t v)
	{
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord;
		o.color = v.color;
		return o;
	}

	float _Rotation;
	fixed4 frag(v2f IN) : SV_Target
	{
		_mask = _mask * (1.0 + _range) - _range;
		float4 col = tex2D(_MainTex,IN.texcoord)*IN.color;
		col.a = min(col.a, saturate((IN.texcoord.y * (1.0 - _reverse) + (1.0 - IN.texcoord.y) * _reverse - _mask) / _range));

		clip(col.a);
		return col;
	}
		ENDCG
	}
	}

		SubShader
	{
		LOD 100

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		Pass
	{
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Offset -1, -1
		//ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMaterial AmbientAndDiffuse

		SetTexture[_MainTex]
	{
		Combine Texture * Primary
	}
	}
	}
}
