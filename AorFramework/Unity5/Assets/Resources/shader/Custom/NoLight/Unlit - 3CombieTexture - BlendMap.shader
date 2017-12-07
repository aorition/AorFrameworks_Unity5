// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//支持两个点灯
Shader "Custom/NoLight/Unlit - 3CombieTexture - BlendMap" {
	Properties{
		_Splat0("Layer1 (RGB)", 2D) = "white" {}
		_Splat1("Layer2 (RGB)", 2D) = "white" {}
		_Splat2("Layer3 (RGB)", 2D) = "white" {}
		_Control("Control (RGBA)", 2D) = "white" {}
		_BlendTex("BlendMap(RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
			[Toggle] _Fog("Fog?", Float) = 1
	}
		SubShader{
			Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType" = "Geometry"}


		Pass {
		  Tags {
			   "LightMode" = "Vertex" }
			Lighting Off
			SetTexture[_Splat0] {
			 combine texture }
			}

			Pass {

			   Tags {  "LightMode" = "ForwardBase" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile ___ LIGHTMAP_ON
				#pragma shader_feature _FOG_ON
				#pragma multi_compile_fog
				#include "Assets/ObjectBaseShader.cginc"



				sampler2D _Splat0;
				sampler2D _Splat1;
				sampler2D _Splat2;
				sampler2D _Control;
				sampler2D _BlendTex;

				struct v2f {
					half4  pos : SV_POSITION;
					half2  uv[5]:TEXCOORD0;
 
#if _FOG_ON
					UNITY_FOG_COORDS(7)
#endif
				};

				half4 _Splat0_ST;
				half4 _Splat1_ST;
				half4 _Splat2_ST;
				half4 _Control_ST;


				v2f vert(appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
					o.uv[1] = TRANSFORM_TEX(v.texcoord, _Splat1);
					o.uv[2] = TRANSFORM_TEX(v.texcoord, _Splat2);
					o.uv[3] = TRANSFORM_TEX(v.texcoord, _Control);
					o.uv[4] = half2(0, 0);
					#ifdef LIGHTMAP_ON
						o.uv[4] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif
					 
#if _FOG_ON
						UNITY_TRANSFER_FOG(o, o.pos);
#endif
 

						  return o;
					  }

					  fixed4 frag(v2f i) : COLOR
					  {
						  fixed3 Mask = tex2D(_Control, i.uv[3].xy).rgb;
						  fixed3 lay1 = tex2D(_Splat0, i.uv[0].xy);
						  fixed3 lay2 = tex2D(_Splat1, i.uv[1].xy);
						  fixed3 lay3 = tex2D(_Splat2, i.uv[2].xy);
						  fixed3 blend = tex2D(_BlendTex, i.uv[3].xy).rgb;
						  fixed4 c;
						  c.rgb = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b);
						  c.a = 1;
						  #ifdef LIGHTMAP_ON
 
									  fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,  i.uv[4]));
									  c.rgb *= lm;
 
									  // c.rgb *= DecodeLogLuv(tex2D(unity_Lightmap, i.uv[4]));
								  #endif
								  c.rgb *= blend;

								  //	c.rgb+=i.lightColor;
									  c = c* _Color*_Lighting;
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