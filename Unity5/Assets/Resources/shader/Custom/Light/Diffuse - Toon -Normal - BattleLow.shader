// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Light/Diffuse - Toon - Normal -BattleLow" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_ToonShade("ToonShader", 2D) = "white" {}
	[Toggle] _Fog("Fog?", Float) = 0
	_Color("Color", Color) = (1,1,1,1)
	[HideInInspector] _Lighting("Lighting", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
	[HideInInspector] _OutlineColor("OutlineColor", Color) = (0.2, 0.2, 0.2, 1)

	}
		SubShader{
				Tags {
					"RenderType" = "Transparent"
				}

		LOD 600

		Pass {
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma shader_feature _FOG_ON

			#pragma multi_compile_fog
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	

				float4 _OutlineColor;
				sampler2D _ToonShade;
				sampler2D _MaskTex;


				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					half3 normal : TEXCOORD1;
					half3 eyeDir   : TEXCOORD2;
					half4 color : COLOR;
#if _FOG_ON
					UNITY_FOG_COORDS(2)
#endif
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
					o.eyeDir.xyz = normalize(worldPos - _WorldSpaceCameraPos.xyz);
 

					#if _FOG_ON
					UNITY_TRANSFER_FOG(o, o.pos);
						#endif

					return o;

					}
				fixed4 frag(v2f i) : COLOR {


					//主光方向
					half3	lightDirection = _RoleDirectionalLightDir.xyz;
 
					//法线
					half3 normal = i.normal;

				 
					 //卡通分界线
					 half d = max(0, dot(normal, lightDirection));
					
					 //高光
					 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  4),0) ;
					
			 
					 //固有色
					fixed4 col = tex2D(_MainTex,i.uv0);

					//环境光
				   fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;

				   half3 ramp = tex2D(_ToonShade, float2(d, 1)).rgb;
				  ramp = pow(ramp, 3);
		
						  //分区域
						// half  area = lerp(ramp.r, ramp.g, mask.g);
						  ramp = lerp(i.color.rgb, fixed3(1.8,1.8, 1.8), saturate(ramp.r));

						  //最终合成
						  half4 final;
						  final.a = col.a;
						  
						  half3 mainlight = col.rgb*(ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb+ splight*0.5) ;
						  final.rgb = mainlight;

					  #ifdef CLIP_ON
						  clip(col.a - _CutOut);
					  #endif

						  final.rgb *= (_Lighting + _HdrIntensity);
						  float isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
						  float3 grayCol = dot(final.rgb, float3(0.299, 0.587, 0.114));
						  final.rgb = lerp(final.rgb*_Color.rgb, grayCol.rgb, isGray);


#if _FOG_ON
						  UNITY_APPLY_FOG(i.fogCoord, final);
#endif
						  return  final;

							  }
						  ENDCG
						  }

	}
	SubShader{
		Tags{
		"RenderType" = "Transparent"
	}

		LOD 200

		UsePass "Custom/Light/Diffuse - Toon/BASETOON"

	}
}
