// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//@@@DynamicShaderInfoStart
//带边缘光的漫反射物体材质 不支持lightMap和顶点动画
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
 Shader "Custom/Light/Diffuse - RimLight##" {
 //@@@DynamicShaderTitleRepaceEnd

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}

	_Color ("Main Color", Color) = (1,1,1,1)
	_Lighting ("Lighting",  float) = 1

	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	
	_CutOut("CutOut", float) = 0.1
}

SubShader {
//@@@DynamicShaderTagsRepaceStart
	Tags {
	"Queue"="Geometry"
	"IgnoreProjector"="True" 
	"RenderType"="Geometry"
	 }
 //@@@DynamicShaderTagsRepaceEnd

	 	// Non-lightmappedx
	Pass {
	
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}


	Pass {  

	Tags { "LightMode" = "ForwardBase" }  

	 //@@@DynamicShaderBlendRepaceStart

	 //@@@DynamicShaderBlendRepaceEnd


		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase  
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma multi_compile FOG_OFF FOG_ON 
			#include "Assets/ObjectBaseShader.cginc"


			struct v2f {
				half4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed3 eyeDir   : TEXCOORD1;
				fixed3 normal   : TEXCOORD2;
				
				#ifdef FOG_ON		
 				half fogFactor: TEXCOORD3;
 				#endif

			};

			fixed _RimPower;
			fixed3  _RimColor;

			struct appdata_normal {
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
				float2 texcoord: TEXCOORD0;
				
			};


			v2f vert (appdata_normal v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				half4 worldPos = mul( unity_ObjectToWorld, v.vertex );
			 	o.eyeDir = normalize( _WorldSpaceCameraPos.xyz - worldPos.xyz ).xyz;
				return o;
			}

			
			fixed4 frag (v2f i) : COLOR
			{
			
			fixed4 col=tex2D (_MainTex, i.texcoord);
			float diff = saturate(dot(i.normal, normalize(i.eyeDir)));  
			half rim = 1.0 - diff;
			col.rgb= diff*col*_Lighting*_Color+_RimColor.rgb * pow (rim, _RimPower);
		
			 
			  			
 				//先clip，再fog 不然会出错	
 			#ifdef CLIP_ON
			clip(  col.a-_CutOut);
			#endif
			
 			#ifdef FOG_ON
			col.a = saturate( exp2(- i.fogFactor /_fogDestiy));
			col.a=0.5;
			#endif

			  
			  return fixed4(col);



			}
		ENDCG
	}
}
//	Fallback "Unlit/Texture"
}
