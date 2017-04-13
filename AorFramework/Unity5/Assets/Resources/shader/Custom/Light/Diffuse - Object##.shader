// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//@@@DynamicShaderInfoStart
//经典物体实时光照材质 支持lightMap 定点动画
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/Light/Diffuse - Object##"  {  
//@@@DynamicShaderTitleRepaceEnd


 //@@@DynamicShaderPropRepaceStart
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Lighting ("Lighting",  float) = 1
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

	[HideInInspector] _CutOut("CutOut", float) = 0.1
	[HideInInspector] _power ("noise Power",  Range(0.001,0.1)) = 0.2
	[HideInInspector] _wind ("wind",  Range(1,10)) = 1
}
 //@@@DynamicShaderPropRepaceEndxx xxxx 

SubShader {

 //@@@DynamicShaderTagsRepaceStart
	Tags {
	"Queue"="Geometry"
	 "IgnoreProjector"="True" 
	 "RenderType"="Geometry"
	 }


 //@@@DynamicShaderTagsRepaceEnd

	// 顶点光照模式xxx
	Pass {
	
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
						
Pass {  
 	Tags { "LightMode" = "ForwardBase"} 
 
	 //@@@DynamicShaderBlendRepaceStart

	 //@@@DynamicShaderBlendRepaceEnd



			CGPROGRAM


		//	#pragma target 3.0
			#pragma multi_compile CLIP_OFF CLIP_ON 
			#pragma multi_compile FOG_OFF FOG_ON
			#pragma multi_compile ANIM_OFF ANIM_ON
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
		//	#pragma multi_compile LIGHTMAP_LOGLUV LIGHTMAP_SCALE

			#pragma vertex vert
			#pragma fragment frag
		//	#pragma multi_compile_fwdbase  
			#include "Assets/ObjectBaseShader.cginc"

		
			struct appdata_lm {
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
				float2 texcoord: TEXCOORD0;
				
				#ifdef LIGHTMAP_ON
				float2 lightmapUV : TEXCOORD1;
				#endif
				
				#ifdef ANIM_ON
				fixed4 vertexColor : COLOR;
				#endif
			};
			

			struct v2f_base {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed3 lightColor : TEXCOORD1; 
				
				#ifdef LIGHTMAP_ON
				float2 lightmapUV : TEXCOORD2;
				#endif
				
				#ifdef FOG_ON		
 				half fogFactor: TEXCOORD3;
 				#endif
			};
 
 
			v2f_base vert (appdata_lm v)
			{
				v2f_base o;
				o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				half4  worldPos = mul( unity_ObjectToWorld, v.vertex );
				half3 normal=mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				o.lightColor =fixed3(0,0,0);
 
					//自定义光源系统在这里 xx
				 o.lightColor = Shade4PointLights(worldPos,normal);
	 


				#ifdef LIGHTMAP_ON
				o.lightmapUV =  v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				
										
				//顶点动画
				#ifdef ANIM_ON
				float4 vColor=v.vertexColor;
				float2 pos = frac(v.vertex.xy / 128.0f) * 128.0f + float2(-64.340622f, -72.465622f);
			    float c=  frac(dot(pos.xyx * pos.xyy, float3(20.390625f, 60.703125f, 2.4281209f)));
				v.vertex.xyz += vColor.rgb*v.normal*sin(_Time*c*_wind)*_power;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				#else
				#endif
						
				#ifdef FOG_ON		
				float4 viewpos=mul(UNITY_MATRIX_MV, v.vertex);
 			
				//体积雾
				o.fogFactor = -(mul(unity_ObjectToWorld, v.vertex).y+ _volumeFogOffset) * _volumeFogDestiy;
				//大气雾
			 	o.fogFactor = max(length(viewpos.xyz) + _fogDestance, o.fogFactor);
 				#endif
				return o;
			}

			fixed4 frag (v2f_base i ) : COLOR
			{
 			  
		 		fixed4 col = tex2D(_MainTex, i.texcoord);

				fixed3 lm=fixed3(0,0,0) ;	
		  	    #ifdef LIGHTMAP_ON
		 
						  lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV));
 
						lm += i.lightColor;
						col.rgb *= lm;
 		 

				col.rgb += col.rgb*i.lightColor.rgb;
				#endif

				
 				col.rgb*=(_Lighting+ _HdrIntensity);
				col*=_Color;

			#ifdef CLIP_ON
				clip(  col.a-_CutOut);
			#endif

		
 			#ifdef FOG_ON
				col.a = saturate( exp2(- i.fogFactor /_fogDestiy));
			#else
				col.a=_Color.a;
			#endif

			 
			return col;

 
			}
		ENDCG
	}

	

}

}