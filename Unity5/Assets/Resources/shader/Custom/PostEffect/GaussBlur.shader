﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PostEffect/GaussBlur" {
	Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Level("Level",Range(0,2)) = 1
	}

	Category{
	SubShader{
    
	Pass{

    ZTest Always

    Fog{ Mode off }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

    sampler2D _MainTex;
	float4 _MainTex_ST;

    float _Level;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};


	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		return o;
	}



	fixed4 frag(v2f i) : SV_Target
	{

		float pixelMove = 0.01;
		
        float3 col = tex2D(_MainTex, i.uv).rgb * 2;

        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(pixelMove, 0)))).rgb;
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(-pixelMove, 0)))).rgb;
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(0, pixelMove)))).rgb;
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(0, -pixelMove)))).rgb;
		
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(pixelMove, pixelMove) * 0.5))).rgb;
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(-pixelMove, pixelMove) * 0.5))).rgb;
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(pixelMove, -pixelMove) * 0.5))).rgb;
        col += tex2D(_MainTex, min(1, max(0, i.uv + float2(-pixelMove, -pixelMove) * 0.5))).rgb;
		
        fixed4 final = fixed4(col * 0.1 * _Level,1);
		
		return max(0, final);

	}
		ENDCG

	}


	}
	}

}