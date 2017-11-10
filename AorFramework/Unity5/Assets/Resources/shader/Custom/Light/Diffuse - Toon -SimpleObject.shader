// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE : unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "Custom/Light/Diffuse - Toon - SimpleObject" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_SpPower("SpPower", float) = 1
	_SpRange("SpRange", float) = 12

	_ToonShade("ToonShader", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
	_Lighting("Lighting", float) = 1


		[HideInInspector] _CutOut("CutOut", float) = 0.1

	}
		SubShader{
				Tags {
					"RenderType" = "Transparent"
				}



		Pass {
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma multi_compile FOG_OFF FOG_ON 
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	

				sampler2D _ToonShade;
				sampler2D _MaskTex;
				float _SpRange;
				float _SpPower;
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					half4 normal   : TEXCOORD1;
					half3 eyeDir   : TEXCOORD2;

					#ifdef LIGHTMAP_ON
					half2 lightmapUV   : TEXCOORD3;
					#endif
				//	half4 color : COLOR;
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.pos = UnityObjectToClipPos(v.vertex);
				//	o.color = v.color;
					o.normal.xyz = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
					o.normal.w = 0;
					//half3  worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);

					#ifdef LIGHTMAP_ON
					o.lightmapUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif


					#ifdef FOG_ON		
					float4 viewpos = mul(UNITY_MATRIX_MV, v.vertex);

					//体积雾
					o.normal.w = -(mul(unity_ObjectToWorld, v.vertex).y + _volumeFogOffset) * _volumeFogDestiy;
					// 大气雾
					o.normal.w = max(length(viewpos.xyz) + _fogDestance, o.normal.w);
					#endif
					return o;

					}
				fixed4 frag(v2f i) : COLOR {

					//控制贴图
					half4 mask = tex2D(_MaskTex, i.uv0);

					//主光方向
					half3 lightDirection = _DirectionalLightDir.xyz;

					//法线
					half3 normal = i.normal;

					//卡通分界线
					half d = max(0, dot(normal, lightDirection));

					//高光
						 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))), _SpRange),0);
						 splight *= _SpPower;

						 // return fixed4(sign(mask.b), sign(mask.b), sign(mask.b), 1);
						 //描边
						 half eyeD = dot(normal, -i.eyeDir);

						 //固有色
						fixed4 col = tex2D(_MainTex,i.uv0);

 
					   d = d;

					   half3 ramp = tex2D(_ToonShade, float2(d, d)).rgb;
 
						   half4 final;
						   final.a = col.a;

						   half3 mainlight = col.rgb*(ramp*_DirectionalLightColor.rgb) + splight*col.rgb;

						   final.rgb = mainlight;

						#ifdef LIGHTMAP_ON
				 
						   fixed3 lm = DecodeLogLuv(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV));
						   final.rgb *= lm;
 
						#endif



					   #ifdef CLIP_ON
						   clip(col.a - _CutOut);
					   #endif
						   final.rgb *= (_Lighting + _HdrIntensity)*_Color.rgb;

							#ifdef FOG_ON
 
						   final.a = exp2(-i.normal.w / _fogDestiy);
							#endif

						   return  final;

							   }
						   ENDCG
						   }

	}

}
