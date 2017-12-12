// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//神殿云层shader,支持lightMap
//@@@DynamicShaderInfoEnd

Shader "Hidden/shendian_Cloud" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
		_NoiseTex("NoiseTex ", 2D) = "white" {}
		_Lighting("Lighting", Range(0, 2)) = 2
		_Color("Color", Color) = (0.5,0.5,0.5,1)
	}
		SubShader{
			Tags{
			"Queue" = "Geometry"
			"IgnoreProjector" = "True"
			"RenderType" = "Geometry"
		}

			Pass {

				Tags {
					"LightMode" = "ForwardBase"
				}
	CGPROGRAM
	#pragma multi_compile FOG_OFF FOG_ON
	#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	#pragma multi_compile LIGHTMAP_LOGLUV LIGHTMAP_SCALE

	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile_fwdbase  
	#include "Assets/ObjectBaseShader.cginc"
	#pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 


		 
				uniform sampler2D _NoiseTex; uniform float4 _NoiseTex_ST;
 
				struct VertexInput {
					float4 vertex : POSITION;
					float2 texcoord0 : TEXCOORD0;

					#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD1;
					#endif

				};
				struct VertexOutput {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD2;
					#endif

					#ifdef FOG_ON		
					half fogFactor: TEXCOORD3;
					#endif




				};
				VertexOutput vert(VertexInput v) {
					VertexOutput o = (VertexOutput)0;
					o.uv0 = v.texcoord0;
					o.pos = UnityObjectToClipPos(v.vertex);


					#ifdef LIGHTMAP_ON
					o.lightmapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

					#ifdef FOG_ON		
					float4 viewpos = mul(UNITY_MATRIX_MV, v.vertex);

					//体积雾
					o.fogFactor = -(mul(unity_ObjectToWorld, v.vertex).y + _volumeFogOffset) * _volumeFogDestiy;
					//大气雾
					o.fogFactor = max(length(viewpos.xyz) + _fogDestance, o.fogFactor);
					#endif

					return o;
				}
				float4 frag(VertexOutput i) : COLOR {

					float2 time = _Time;
					float2 uv = (i.uv0 + time*float2(0,0.002));
					float4 mainCol = tex2D(_MainTex,TRANSFORM_TEX(uv, _MainTex));

					float2 uv2 = (i.uv0 + time*float2(0,0.01));
					float4 noiseCol = tex2D(_NoiseTex,TRANSFORM_TEX(uv2, _NoiseTex));
					float3 emissive = (mainCol.rgb*(noiseCol.rgb + _Color.rgb)*_Lighting);
					fixed alpha = 1;
					fixed3 lm=fixed3(0,0,0);
					#ifdef LIGHTMAP_ON
					#ifdef LIGHTMAP_LOGLUV
					lm = DecodeLogLuv(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV));
					#else
					lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV).rgb;
					lm = lm*lm * 2;
					lm *= 2;
					#endif
					emissive.rgb *= lm;
					#endif
					#ifdef FOG_ON
					alpha = saturate(exp2(-i.fogFactor / _fogDestiy));
					#endif

					return fixed4(emissive, alpha);
				}
				ENDCG
			}
		}

}
