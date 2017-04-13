Shader "Custom/Light/Diffuse - Toon - Normal" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask Tex", 2D) = "black" {}
	_NormalTex("NormalTex", 2D) = "white" {}
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
	_LowOutlineWidth("LowOutlineWidth", float) = 4
	//[HideInInspector] _HightOutlineWidth("HightOutlineWidth", float) = 4
	_ToonShade("ToonShader", 2D) = "white" {}
	_HideFace("_HideFace", int) = 0
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

				float _LowOutlineWidth;
				float4	_OutlineColor;
				int _HideFace;
				sampler2D _ToonShade;
				sampler2D _MaskTex;
				sampler2D _NormalTex;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					half3 normal   : TEXCOORD1;
					half3 eyeDir   : TEXCOORD3;
					half3 lightColor : TEXCOORD4;
					half4 Tangent : TEXCOORD5;
					half3 BiNormal : TEXCOORD6;
					half4 color : COLOR;
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
					half3  worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
					o.lightColor = Shade4PointLights(worldPos, o.normal);
					o.Tangent = normalize(float4(mul(v.tangent.xyz, (float3x3)unity_WorldToObject), v.tangent.w));
					o.BiNormal = cross(o.normal,  o.Tangent) * o.Tangent.w;
					return o;

					}
				fixed4 frag(v2f i) : COLOR {

					//控制贴图
					half4 mask = tex2D(_MaskTex, i.uv0);

					//主光方向
					half3 lightDirection = _DirectionalLightDir.xyz;

					//法线
					half3 normal = i.normal;
					fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
					//normalMap = normalize(normalMap);

				   half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
					normal = normalize(mul(normalMap.xyz, tbnMatrix));
					// return fixed4(normal, 1);
					 //卡通分界线
					 half d = max(0, dot(normal, lightDirection));

					 //高光
					 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  mask.b * 16 + 4),0) *sign(mask.b);

					 // return fixed4(sign(mask.b), sign(mask.b), sign(mask.b), 1);
					 //描边
					 half eyeD = dot(normal, -i.eyeDir);
					 half outlight = pow(clamp(1 - eyeD, 0, 1), _LowOutlineWidth)*mask.a;

					 //固有色
					fixed4 col = tex2D(_MainTex,i.uv0);

					//环境光
				   fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;


				   d = d + splight;
				   //return fixed4(d,d,d, 1);
				   half3 ramp = tex2D(_ToonShade, float2(d*mask.r, d)).rgb;

				   ramp = pow(ramp, 3);
				   //	return fixed4(ramp, 1);
						  //分区域
						  d = lerp(ramp.r, ramp.g, mask.g);
						  ramp = lerp(i.color.rgb, fixed3(2, 2, 2), saturate(d));
						  //return fixed4(ramp, 1);
						  //最终合成
						  half4 final;
						  final.a = col.a;

						  half3 mainlight = col.rgb*(ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb) + i.lightColor.rgb *d;


						  final.rgb = mainlight;
						  final.rgb = lerp(final.rgb, _OutlineColor*_OutlineColor.a * 10, outlight);

						  clip(i.color.a - _HideFace);

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
