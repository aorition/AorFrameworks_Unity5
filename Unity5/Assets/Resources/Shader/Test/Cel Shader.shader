// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Max820/Cel Shader" {
	Properties{
	   // _Color("Color", Color) = (1,1,1,1)

		_MainTex("Base Texture (RGB)", 2D) = "white" {}
		_SSSTex("SSS Texture (RGB)", 2D) = "white" {}
		_ILMTex("ILM Texture (RGB)", 2D) = "white" {}

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline Width", Range(.0, 2)) = .5
		_ShadowContrast("Shadow Contrast", Range(0, 20)) = 1
		_DarkenInnerLine("Darken Inner Line", Range(0, 1)) = 0.2

//		_LightDirection("Light Direction", Vector) = (0,0,1)
	}


CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
		sampler2D _SSSTex;
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 texCoord : TEXCOORD0;
		//float4 vertexColor : COLOR;
	};

	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
		float4 tex : TEXCOORD0;
	};

	uniform float _Outline;
	uniform float4 _OutlineColor;
	uniform float _ShadowContrast;
	uniform float _DarkenInnerLine;
//	uniform half3 _LightDirection;

	v2f vert(appdata v) {
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
		o.pos.xy += offset * _Outline * 0.5 ;
		o.tex = v.texCoord;
		
		o.color = _OutlineColor ;
		return o;
	}
ENDCG

SubShader 
{
	//Tags {"Queue" = "Geometry+100" }
	CGPROGRAM
	#pragma surface surfA Lambert

	//fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surfA(Input IN, inout SurfaceOutput o) {

		half4 c2 = half4(1, 0, 1, 1);

		o.Albedo = c2.rgb ;
		o.Alpha = c2.a;
	}
	ENDCG

	// note that a vertex shader is specified here but its using the one above
	Pass{
		Name "OUTLINE"
		Tags{ "LightMode" = "Always" }
		Cull Front
		ZWrite On
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		//Offset 50,50
		//Lighting Off

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

	half4 frag(v2f i) :COLOR { 

		fixed4 cLight = tex2D(_MainTex, i.tex.xy);
		fixed4 cSSS = tex2D(_SSSTex, i.tex.xy);
		fixed4 cDark = cLight * cSSS;

		cDark = cDark *0.5f;// *cDark * cDark;
		cDark.a = 1; // weapon had alpha?
		//return cDark;
		return (i.color + cDark ); 
	}

		ENDCG
	} // Pass


	// ###############################
	CGPROGRAM
  
		// noforwardadd: important to remove multiple light passes
	#pragma surface surf  CelShadingForward  vertex:vertB 

	#pragma target 3.0


	sampler2D _ILMTex;

	struct Input {
		float2 uv_MainTex;
		float3 vertexColor; // Vertex color stored here by vert() method
	};

	struct v2fB {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
	};

	void vertB(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.vertexColor = v.color ; // Save the Vertex Color in the Input for the surf() method
	}

	struct SurfaceOutputCustom
	{
		fixed3 Albedo;
		fixed3 Normal;
		fixed3 Emission;
		fixed Alpha;

		half3 BrightColor;
		half3 ShadowColor;
		half3 InnerLineColor;
		half ShadowThreshold;

		half SpecularIntensity;
		half SpecularSize;

		//fixed4 ILM;
	};

	half4 LightingCelShadingForward(SurfaceOutputCustom s, half3 lightDir, /*half3 viewDir,*/ half atten) {
//		lightDir = _LightDirection;
		
	// Cell shading: Threshold (<=0, > 90 deg away from light), light vector, normal vector [control]
		half NdotL = dot(lightDir, s.Normal);
//		NdotL = smoothstep(0, 1.0f, NdotL);
//		NdotL = smoothstep(0, 0.025f, NdotL);

		half testDot = (NdotL + 1) / 2.0; // color 0 to 1. black = shadow, white = light
		half4 c = half4(0, 0, 0, 1);

		half4 specColor = half4(s.SpecularIntensity, s.SpecularIntensity, s.SpecularIntensity, 1);
		half blendArea = 0.04;

		NdotL -= s.ShadowThreshold;

		half specStrength = s.SpecularIntensity;// = 0.1f + s.SpecularIntensity;// > 1 = brighter, < 1 = darker
		if (NdotL < 0) // <= s.ShadowThreshold)
		{
			
			if ( NdotL < - s.SpecularSize -0.5f && specStrength <= 0.5f) // -0.5f)
			{
				c.rgb = s.ShadowColor *(0.5f + specStrength);// (specStrength + 0.5f);// 0.5f; //  *s.ShadowColor;
			}
			else
			{
				c.rgb = s.ShadowColor;
			}
		}
		else
		{
			if (s.SpecularSize < 1 && NdotL * 1.8f > s.SpecularSize && specStrength >= 0.5f) //  0.5f) // 1.0f)
			{
				c.rgb = s.BrightColor * (0.5f + specStrength);// 1.5f;//  *(specStrength * 2);// 2; // lighter
			}
			else
			{
				c.rgb = s.BrightColor;
			}

		}
		
		// add inner lines
		c.rgb = c.rgb * s.InnerLineColor ;

				

		return c;
	}

	void surf(Input IN, inout SurfaceOutputCustom  o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

		fixed4 cSSS = tex2D(_SSSTex, IN.uv_MainTex);
		fixed4 cILM = tex2D(_ILMTex, IN.uv_MainTex);

		o.BrightColor = c.rgb;
		o.ShadowColor = c.rgb * cSSS.rgb;

		float clampedLineColor = cILM.a;
		if (clampedLineColor < _DarkenInnerLine)
			clampedLineColor = _DarkenInnerLine; 

		o.InnerLineColor = half3(clampedLineColor, clampedLineColor, clampedLineColor);


		float vertColor =  (IN.vertexColor.r - 0.5) * _ShadowContrast + 0.5; //IN.vertexColor.r;
			// easier to combine black dark areas 
			o.ShadowThreshold = cILM.g;
			o.ShadowThreshold *= vertColor;
			o.ShadowThreshold = 1 - o.ShadowThreshold; // flip black / white


		o.SpecularIntensity = cILM.r;// 1 + (1 - cILM.r);// +cILM.r;// *2; // make whiter

		o.SpecularSize =  1-cILM.b;// * 0.25f);

	}

	ENDCG

	}

	FallBack "Diffuse"
}
