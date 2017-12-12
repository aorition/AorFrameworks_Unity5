// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//AlphaMask
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - ScaleTexture##" {
	//@@@DynamicShaderTitleRepaceEnd

	Properties{
		_MainTex("Texture", 2D) = "white" {}
 
		_Main_Rotation("Main Rotation Angle", Float) = 0
		_Main_Scale("Main Scale",Range(0.001,10)) = 1

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

		Pass
	{

			Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
			ZWrite[_ZWrite]
			ZTest[_ZTest]
			Cull[_Cull]



 
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _MainTex;
 

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		half2 texcoord  : TEXCOORD0;
	};

	float4 _MainTex_ST;
 

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
		OUT.color = IN.color;

		return OUT;
	}

	float _Main_Rotation;
	float _Main_Scale;
 

	fixed4 frag(v2f IN) : COLOR
	{
		float2 uv = IN.texcoord.xy - float2(0.5,0.5);
		float mag = radians(_Main_Rotation);

		uv = float2(uv.x/_Main_Scale*cos(mag) - uv.y/_Main_Scale*sin(mag),
			uv.x/_Main_Scale*sin(mag) + uv.y/_Main_Scale*cos(mag));
		uv += float2(0.5,0.5);

		float4 col = tex2D(_MainTex,uv) * IN.color;

		return col;
	}
		ENDCG
	}
	}
}