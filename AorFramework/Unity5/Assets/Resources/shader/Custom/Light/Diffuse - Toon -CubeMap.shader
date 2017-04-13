#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "Custom/Light/Diffuse - Toon - CubeMap" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
	_Outline("Outline Range", float) = 4
	_ToonShade("ToonShader", 2D) = "white" {}
	_Cube("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	_RefPower("Reflection Power", float) = 1
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
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	

				float _Outline;
				float _RefPower;
				float4	_OutlineColor;
				sampler2D _ToonShade;
				samplerCUBE _Cube;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					half3 normal   : TEXCOORD1;
					half3 eyeDir   : TEXCOORD2;
					half3 lightColor : TEXCOORD3;
					half4 color : COLOR;
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					o.normal = normalize( mul(v.normal, (float3x3)unity_WorldToObject));
					half3  worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
					o.lightColor = Shade4PointLights(worldPos, o.normal);
 
					return o;

					}
				fixed4 frag(v2f i) : COLOR {

 

					//主光方向
					half3 lightDirection = _DirectionalLightDir.xyz;

					//法线
					half3 normal =i.normal;
 
					 //卡通分界线
					 half d = max(0, dot(normal, lightDirection));
					
						 //高光
						 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  32),0) ;
					 
					 
						 //描边
						 half eyeD = dot(normal, -i.eyeDir);
						 half outlight = pow(clamp(1 - eyeD, 0, 1), _Outline);
						 //反射
						 half3 ref = reflect(normalize(i.eyeDir), i.normal);

						 //固有色
						fixed4 col = tex2D(_MainTex,i.uv0);
						fixed4 reflcol = texCUBE(_Cube, ref);
						 //环境光
						fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz  ;
						

						d = d + splight;
				
						half ramp = tex2D(_ToonShade, float2(d, d)).g;
				 
						ramp = pow(ramp, 3);
					
							//分区域
 
							ramp = lerp(i.color.rgb, fixed3(2, 2, 2), saturate(ramp));
						
							//最终合成
							half4 final;
							final.a = col.a;

							half3 mainlight = col.rgb*(ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb) + i.lightColor.rgb *d;
							
							 
							final.rgb = mainlight+ reflcol.rgb*_RefPower;
							 
							final.rgb = lerp(final.rgb, _OutlineColor*_OutlineColor.a * 10, outlight);
 
						#ifdef CLIP_ON
							clip(col.a - _CutOut);
						#endif
							final.rgb *= (_Lighting + _HdrIntensity)*_Color.rgb;

							return  final;

								}
							ENDCG
							}

	}

}
