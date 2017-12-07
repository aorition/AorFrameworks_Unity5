// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//支持两个点灯
Shader "Custom/NoLight/Unlit - 4CombieTexture" {
	Properties{
		_Splat0("Layer1 (RGB)", 2D) = "white" {}
	_Splat1("Layer2 (RGB)", 2D) = "white" {}
	_Splat2("Layer3 (RGB)", 2D) = "white" {}
	_Splat3("Layer3 (RGB)", 2D) = "white" {}
	_Control("Control (RGBA)", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		[Toggle] _Fog("Fog?", Float) = 1
	}
		SubShader{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType" = "Geometry" }


		Pass{
		Tags{
		"LightMode" = "Vertex" }
		Lighting Off
		SetTexture[_Splat0]{
		combine texture }
	}

		Pass{
		Name "BASECOMBIE"
		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM
#pragma target 2.0
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile ___ LIGHTMAP_ON
#pragma shader_feature _FOG_ON
#pragma multi_compile_fog
		//	#pragma multi_compile LIGHTMAP_LOGLUV LIGHTMAP_SCALE
		//	#pragma multi_compile_fwdbase  
#include "Assets/ObjectBaseShader.cginc"


		sampler2D _Splat0;
	sampler2D _Splat1;
	sampler2D _Splat2;
	sampler2D _Splat3;
	sampler2D _Control;

	struct v2f {
		float4  pos : SV_POSITION;
		float2  uv0:TEXCOORD0;
		float2  uv1:TEXCOORD1;
		float2  uv2:TEXCOORD2;
		float2  uv3:TEXCOORD3;
		float2  uv4:TEXCOORD4;
		float2  uv5:TEXCOORD5;

#if _FOG_ON
		UNITY_FOG_COORDS(6)
#endif


	};

	float4 _Splat0_ST;
	float4 _Splat1_ST;
	float4 _Splat2_ST;
	float4 _Splat3_ST;
	float4 _Control_ST;


	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv0 = TRANSFORM_TEX(v.texcoord, _Splat0);
		o.uv1 = TRANSFORM_TEX(v.texcoord, _Splat1);
		o.uv2 = TRANSFORM_TEX(v.texcoord, _Splat2);
		o.uv3 = TRANSFORM_TEX(v.texcoord, _Splat3);
		o.uv4 = TRANSFORM_TEX(v.texcoord, _Control);
		o.uv5 = float2(0, 0);
#ifdef LIGHTMAP_ON
		o.uv5 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#if _FOG_ON
		UNITY_TRANSFER_FOG(o, o.pos);
#endif

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		float4 Mask = tex2D(_Control, i.uv4.xy);
		float4 lay1 = tex2D(_Splat0, i.uv0.xy);
		float4 lay2 = tex2D(_Splat1, i.uv1.xy);
		float4 lay3 = tex2D(_Splat2, i.uv2.xy);
		float4 lay4 = tex2D(_Splat3, i.uv3.xy);
		float4 c;
		c.rgb = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b + lay4.xyz * Mask.a);
		c.a = 1;

#ifdef LIGHTMAP_ON

		float3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,  i.uv5));
		c.rgb *= lm;

#endif


		c.rgb *= (_Lighting + _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w + UNITY_LIGHTMODEL_AMBIENT.xyz;
		c *= _Color;

		c.rgb = min(c.rgb, float3(1, 1, 1));
		c.a = 1;

#if _FOG_ON
		UNITY_APPLY_FOG(i.fogCoord, c);
#endif

		return c;

	}
		ENDCG
	}


	}
}