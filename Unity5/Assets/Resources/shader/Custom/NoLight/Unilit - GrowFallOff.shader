﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//带更多选项的自发光贴图材质
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - GrowFallOff"  {
	//@@@DynamicShaderTitleRepaceEnd

	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_CoreColor("Core Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_TintStrength("Tint Color Strength", Range(0, 5)) = 1
		_CoreStrength("Core Color Strength", Range(0, 8)) = 1
		_CutOutLightCore("CutOut Light Core", Range(0, 1)) = 0.5

		_FallOffColor("FallOffColor", color) = (0.2, 0.5, 1,1)
		_Lighting("Light", float) = 1

		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
	}



		SubShader{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass{


		Lighting Off

		Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
		ZWrite[_ZWrite]
		ZTest[_ZTest]
		Cull[_Cull]

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _MainTex;
	fixed4 _TintColor;
	fixed4 _CoreColor;
	float _CutOutLightCore;
	float _TintStrength;
	float _CoreStrength;

	float4 _FallOffColor;
	float _Lighting;

	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		float3 worldvertpos : TEXCOORD1;
		float3 normal : TEXCOORD2;
	};

	float4 _MainTex_ST;

	v2f vert(appdata_base v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.normal = v.normal;
		o.worldvertpos = ObjSpaceViewDir(v.vertex).xyz;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{

		fixed4 tex = tex2D(_MainTex, i.texcoord);
		fixed4 col = (_TintColor * tex.g * _TintStrength + tex.r * _CoreColor * _CoreStrength - _CutOutLightCore);

		i.normal = normalize(i.normal);
		float3 viewdir = normalize(i.worldvertpos);

		float4 texCol = 1;

		texCol.a = min(pow((1 - saturate(dot(viewdir, i.normal))), 3), 1);
		texCol.rgb = texCol.a * 3 * _FallOffColor.xyz;

		col.rgb += texCol * _Lighting;
		col.a = tex.a;
		
		return col;
	}
		ENDCG
	}
	}
}