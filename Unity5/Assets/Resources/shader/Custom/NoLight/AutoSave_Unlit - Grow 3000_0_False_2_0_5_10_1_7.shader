// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//不写深度的双面的Alpha混合的shader,层:Transparent
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/NoLight/Unlit - Grow 3000_0_False_2_0_5_10_1_7"  {  
//@@@DynamicShaderTitleRepaceEnd

	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_CoreColor ("Core Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_TintStrength ("Tint Color Strength", Range(0, 5)) = 1
	_CoreStrength ("Core Color Strength", Range(0, 8)) = 1
	_CutOutLightCore ("CutOut Light Core", Range(0, 1)) = 0.5
	
}


	
	SubShader {

		   //@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Transparent" } 
//@@@DynamicShaderTagsRepaceEnd
       	
       		Lighting Off


		Pass {

		      //@@@DynamicShaderBlendRepaceStart
Blend SrcAlpha OneMinusSrcAlpha,One DstAlpha
ZTest Less
 ZWrite Off
 Cull Off
//@@@DynamicShaderBlendRepaceEnd
		
	

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
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.texcoord);
				fixed4 col = (_TintColor * tex.g * _TintStrength + tex.r * _CoreColor * _CoreStrength  - _CutOutLightCore); 
				col.a = tex.a;
				return col;
			}
			ENDCG 
		}
	}	
}
