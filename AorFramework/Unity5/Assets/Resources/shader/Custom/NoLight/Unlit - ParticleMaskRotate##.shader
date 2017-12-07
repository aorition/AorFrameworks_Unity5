// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//粒子专用的体积shader,通过mask的旋转控制整体明暗,牺牲了粒子的上色功能,用来控制主贴图的旋转
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - ParticleMaskRotate##" {
	//@@@DynamicShaderTitleRepaceEnd

	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
		_MaskRotation("Main Rotation Angle", Float) = 0
	//	_Main_Scale("Main Scale",Range(0.001,10)) = 1

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
		float4 _MainTex_ST;

		sampler2D _MaskTex;
		float4 _MaskTex_ST;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
		float2 maskUv : TEXCOORD1;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
		float2 maskUv  : TEXCOORD1;
	};

	
 

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
		OUT.maskUv = TRANSFORM_TEX(IN.maskUv, _MaskTex);
		OUT.color = IN.color;

		return OUT;
	}

	float _MaskRotation;
 
	float2 rot(float2 uv,float mag) {
	
	return	float2(uv.x*cos(mag) - uv.y*sin(mag),
			uv.x*sin(mag) + uv.y*cos(mag));
	}

	fixed4 frag(v2f IN) : COLOR
	{

		//mainTex
		float2 uv = IN.texcoord.xy - float2(0.5,0.5);
		float mag = radians(lerp(0,360, IN.color.r));
		uv = rot(uv, mag);
		uv += float2(0.5,0.5);
		float4 col = tex2D(_MainTex,uv);

		//MaskTex
		uv = IN.maskUv.xy - float2(0.5, 0.5);
		mag = radians(_MaskRotation);
		uv = rot(uv, mag);
		uv += float2(0.5, 0.5);
		float4 mask = tex2D(_MaskTex, uv);
		col *= mask;
		return col;
	}
		ENDCG
	}
	}
}