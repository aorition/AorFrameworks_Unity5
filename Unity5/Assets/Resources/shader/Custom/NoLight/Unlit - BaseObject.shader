// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//无光照物体材质,只支持lightMap
//@@@DynamicShaderInfoEnd


Shader "Custom/NoLight/Unlit - BaseObject"  {




	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		[Toggle] _Fog("Fog?", Float) = 1
		[HideInInspector] _CutOut("CutOut", float) = 0.1

	}

	SubShader{
 

			Tags {
			   "Queue" = "Geometry"
				"IgnoreProjector" = "True"
				"RenderType" = "Geometry"
		    }
 
		   Pass {
				Name "BASEOBJECT"
			   Tags { "LightMode" = "ForwardBase"}



				CGPROGRAM

				#pragma multi_compile CLIP_OFF CLIP_ON 
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

				#ifdef LIGHTMAP_ON
				    o.lightmapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				#if _FOG_ON
				    UNITY_TRANSFER_FOG(o, o.vertex);
				#endif

				return o;
			}

			fixed4 frag(v2f_base i) : COLOR
			{

				fixed4 col = tex2D(_MainTex, i.texcoord);

		    #ifdef LIGHTMAP_ON
				fixed3	 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV.xy));
				col.rgb *= lm;
			#endif
	 
				col.rgb *= (_Lighting + _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w + UNITY_LIGHTMODEL_AMBIENT.xyz;
				col.a *= _Color.a;

				float isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
				float3 grayCol = dot(col.rgb, float3(0.299, 0.587, 0.114));
				col.rgb = lerp(col.rgb*_Color.rgb, grayCol.rgb, isGray);

			#if _FOG_ON
				UNITY_APPLY_FOG(i.fogCoord , col);
			#endif

			#ifdef CLIP_ON
				clip(col.a - _CutOut);
			#endif

			return col;


			}
		    ENDCG
	    }

	}


}