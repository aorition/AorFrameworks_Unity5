// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Light/Diffuse - Toon - line" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_HightOutlineWidth("HightOutlineWidth", float) = 4
	_ToonShade("ToonShader", 2D) = "white" {}
	[Toggle] _Fog("Fog?", Float) = 0
	[HideInInspector]_Color("Color", Color) = (1,1,1,1)
	[HideInInspector]_Lighting("Lighting", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
	}
		SubShader{
			Tags {
			"RenderType" = "Transparent"
			}

			UsePass "Custom/Light/Diffuse - Toon - Normal -line/LINEPASS"

			Pass {
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma multi_compile CLIP_OFF CLIP_ON
	

				#pragma shader_feature _FOG_ON
				#pragma multi_compile_fog

				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/ObjectBaseShader.cginc"	

				 
					float4	_OutlineColor;
				
					sampler2D _ToonShade;
 

					struct v2f {
						float4 pos : SV_POSITION;
						float2 uv0 : TEXCOORD0;
						half3 normal   : TEXCOORD1;
						half3 eyeDir   : TEXCOORD3;
						fixed3 lightColor : TEXCOORD4;
						half4 Tangent : TEXCOORD5;
						half3 BiNormal : TEXCOORD6;
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
						o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
						half3  worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
						half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
						o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
						o.lightColor = Shade4PointLights(worldPos, o.normal);
						o.Tangent = normalize(float4(mul(v.tangent.xyz, (float3x3)unity_WorldToObject), v.tangent.w));
						o.BiNormal = cross(o.normal, o.Tangent) * o.Tangent.w;

#if _FOG_ON
						UNITY_TRANSFER_FOG(o, o.pos);
#endif

						return o;

						}

					fixed4 frag(v2f i) : COLOR {

						//控制贴图
						//half4 mask = tex2D(_MaskTex, i.uv0);

						//主光方向
						half3 lightDirection = _RoleDirectionalLightDir.xyz;

						//法线
						half3 normal = i.normal;

						//卡通分界线
						half d = max(0, dot(normal, lightDirection));

						//高光
						half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))), 20),0);

						//描边
						half eyeD = dot(normal, -i.eyeDir);

						//固有色
						fixed4 col = tex2D(_MainTex,i.uv0);

						//环境光
						fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;
						//3方向环境光
						d = d + splight;

						

						half3 ramp = tex2D(_ToonShade, float2(d, d)).rgb;
						ramp = pow(ramp, 3);
						ramp = lerp(i.color.rgb, fixed3(2, 2, 2), saturate(ramp));
						//最终合成
						half4 final;
						final.a = col.a;

						half3 mainlight = col.rgb*(ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb) + i.lightColor.rgb ;
						final.rgb = mainlight*_Color.rgb;

						//clip(i.color.a - _HideFace);
						#ifdef CLIP_ON
						clip(col.a - _CutOut);
						#endif
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
