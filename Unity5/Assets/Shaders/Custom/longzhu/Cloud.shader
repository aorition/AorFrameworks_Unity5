// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/longzhu/Cloud" {


	Properties{
		_MainTex("Texture", 2D) = "white" {}
	_Mask("Mask", 2D) = "white" {}
	_TintColor("Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		[Toggle] _Fog("Fog?", Float) = 0
		[Toggle] _HasNight("HasNight?", Float) = 0    //夜晚效果开关

		_AnimTime("AnimTime", float) = 0

		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4

	}


		SubShader{


		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{

		ZWrite Off
		ZTest[_ZTest]
		Cull[_Cull]

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Assets/ObjectBaseShader.cginc"
#pragma shader_feature _HASNIGHT_ON
#pragma shader_feature _FOG_ON
#pragma multi_compile_fog

		sampler2D _Mask;
	float4 _Mask_ST;

	struct v2f {
		float4  pos : SV_POSITION;
		float2  uv : TEXCOORD0;
		float2	uv2 : TEXCOORD1;
		float4 color : COLOR;
#if _FOG_ON
		UNITY_FOG_COORDS(3)
#endif

	};

	struct appdata {
		float4 vertex : POSITION;
		float2 texcoord:TEXCOORD0;
		float4 color : COLOR;
	};


	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.uv2 = TRANSFORM_TEX(v.texcoord, _Mask);
		o.color = v.color;

#if _FOG_ON
		UNITY_TRANSFER_FOG(o, o.pos);
#endif
		return o;
	}

	float _AnimTime;

	fixed4 _TintColor;

	float4 frag(v2f i) : COLOR
	{
		float t = _AnimTime;
	float4 col = tex2D(_MainTex,i.uv + t * 2);
	float4 mask = tex2D(_Mask, i.uv2 + t);

	col.rgb *= _Lighting;

	float isGray = step(dot(_TintColor.rgb, fixed4(1, 1, 1, 0)), 0);
	float3 grayCol = dot(col.rgb, float3(0.299, 0.587, 0.114));
	col.rgb = lerp(col.rgb* _TintColor*i.color, grayCol.rgb, isGray);

#ifdef _HASNIGHT_ON
	col.rgb *= _HdrIntensity + _DirectionalLightColor*_DirectionalLightDir.w + col.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
#endif

#if _FOG_ON
	UNITY_APPLY_FOG(i.fogCoord, col);
#endif

	col.rgb = col.rgb * mask.rgb + mask.rgb / 10;
	col.a *= _TintColor.a*i.color.a;

	return max(0, min(1, col));
	}



		ENDCG
	}


	}
}