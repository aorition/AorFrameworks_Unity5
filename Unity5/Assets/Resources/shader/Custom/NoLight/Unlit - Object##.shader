// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//无光照物体材质 支持lightMap 顶点动画 荧幕跳动
//@@@DynamicShaderInfoEnd


Shader "Custom/NoLight/Unlit - Object##"  {

	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		[Toggle] _Fog("Fog?", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4

		[HideInInspector] _CutOut("CutOut", float) = 0.1
		[HideInInspector] _power("noise Power",  Range(0.001,0.1)) = 0.2
		[HideInInspector] _wind("wind",  Range(1,10)) = 1

		//荧幕跳动
		[HideInInspector] _ScreenFlickSpeed("Flicker Speed", range(0,50)) = 5
		[HideInInspector] _ScreenFlickLevel("Flicker Level", range(0,1)) = 0.1
		[HideInInspector] _ScreenScrollSpeed("Scrolling Speed", range(0,2)) = 0.04
		[HideInInspector] _ScreenScrollTiling("Scrolling Tiling", range(0,25)) = 8
		[HideInInspector] _ScreenScrollColor("Scrolling Color", Color) = (1,1,1,1)
		[HideInInspector] _ScreenScrollTex("Scrolling Texture", 2D) = "white" {}
	}

	SubShader{
 

			Tags {
			   "Queue" = "Geometry"
				"IgnoreProjector" = "True"
				"RenderType" = "Geometry"
		    }
 
		   Pass {
	 
			   Tags { "LightMode" = "ForwardBase"}
			   ZWrite[_ZWrite]
			   ZTest[_ZTest]
			   Cull[_Cull]


				CGPROGRAM

			//	#pragma target 3.0
				#pragma multi_compile CLIP_OFF CLIP_ON 
			//	#pragma multi_compile FOG_OFF FOG_ON
				#pragma multi_compile ANIM_OFF ANIM_ON
				#pragma multi_compile ___ LIGHTMAP_ON
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature _FOG_ON
				#pragma multi_compile_fog

				//荧幕跳动
				#pragma multi_compile ___ SCREENJUMP_ON

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

					#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD2;
					#endif

					#if _FOG_ON
					UNITY_FOG_COORDS(3)
					#endif
				};


				v2f_base vert(appdata_lm v)
				{
					v2f_base o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					//half4  worldPos = mul(_Object2World, v.vertex);



				#ifdef LIGHTMAP_ON
				    o.lightmapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif


				//顶点动画
				#ifdef ANIM_ON
				    float4 vColor = v.vertexColor;
				    float2 pos = frac(v.vertex.xy / 128.0f) * 128.0f + float2(-64.340622f, -72.465622f);
				    float c = frac(dot(pos.xyx * pos.xyy, float3(20.390625f, 60.703125f, 2.4281209f)));
				    half3 normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				    v.vertex.xyz += vColor.rgb*v.normal*sin(_Time*c*_wind)*_power;
				    o.vertex = UnityObjectToClipPos(v.vertex);
				#else
				#endif

				#if _FOG_ON
				    UNITY_TRANSFER_FOG(o, o.vertex);
				#endif

				return o;
			}

			fixed4 frag(v2f_base i) : COLOR
			{

				fixed4 col = tex2D(_MainTex, i.texcoord);

			//荧幕跳动
			#ifdef SCREENJUMP_ON
			    fixed4 screenCol =tex2D(_ScreenScrollTex, float2(i.texcoord.x, i.texcoord.y*_ScreenScrollTiling + _ScreenScrollSpeed * _Time.y));
				col += screenCol * _ScreenScrollColor;
				float flicker = lerp(1, sin(_Time.y* _ScreenFlickSpeed), _ScreenFlickLevel);
				col *= flicker;
			#endif

		    #ifdef LIGHTMAP_ON
				fixed3	 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV.xy));
				col.rgb *= lm;
			#endif
	 
				col.rgb *= (_Lighting + _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w + UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				#ifdef CLIP_ON
					clip(col.a - _CutOut);
				#endif
				
				col *= _Color;

			#if _FOG_ON
				UNITY_APPLY_FOG(i.fogCoord , col);
			#endif

			return col;


			}
		    ENDCG
	    }

	}


}