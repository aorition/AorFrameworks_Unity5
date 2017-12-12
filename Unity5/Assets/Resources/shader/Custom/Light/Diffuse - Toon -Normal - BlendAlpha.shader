// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Light/Diffuse - Toon - Normal - BlendAlpha" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask(Must No mipMap)", 2D) = "black" {}
	_NormalTex("NormalTex", 2D) = "white" {}
	//_LowOutlineWidth("LowOutlineWidth", float) = 4
		_BlendAlpha("BlendAlpha",Range(0, 1)) = 1
	//[HideInInspector] _HightOutlineWidth("HightOutlineWidth", float) = 4
	_ToonShade("ToonShader", 2D) = "white" {}
	_HideFace("_HideFace", int) = 0
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

			//Blend one one//
		//Blend SrcAlpha DstAlpha, one zero
		Blend SrcAlpha DstAlpha, zero one        //alpha blending
		//ztest off

			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma shader_feature _FOG_ON

			#pragma multi_compile_fog
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	

				float4 _OutlineColor;
				//float _LowOutlineWidth;
				float _UILight;
				float _HideFace;
				float _BlendAlpha;
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
				//	half3  worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);

					o.eyeDir.xyz = normalize(worldPos - _WorldSpaceCameraPos.xyz);
 

					o.lightColor = Shade4PointLights(worldPos, o.normal);
					o.Tangent = normalize(float4(mul(v.tangent.xyz, (float3x3)unity_WorldToObject), v.tangent.w));
					o.BiNormal = cross(o.normal,  o.Tangent) * o.Tangent.w;


					#if _FOG_ON
					UNITY_TRANSFER_FOG(o, o.pos);
						#endif

					return o;

					}
				fixed4 frag(v2f i) : COLOR {

					//控制贴图
					half4 mask = tex2D(_MaskTex, i.uv0);

					//主光方向
					half3	lightDirection = lerp(_RoleDirectionalLightDir.xyz, half3(0.6, 0.3, -1), _UILight);
 
					//法线
					half3 normal = i.normal;
					fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
					normalMap = normalize(normalMap);
					half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
					normal = normalize(mul(normalMap.xyz, tbnMatrix));
				 
					 //卡通分界线
					 half d = max(0, dot(normal, normalize(lightDirection)));
					 half diff = pow(d,2) * 0.2;

					 // return diff;
					 //高光
					 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  mask.b * 16 + 4),0) ;
					 
					 //描边
//					 half eyeD = dot(normal, -i.eyeDir);
//					 half outlight = pow(saturate(1 - eyeD), _LowOutlineWidth)*mask.a;
//					 outlight = floor(outlight * 2);

					 //固有色
					fixed4 col = tex2D(_MainTex,i.uv0);

			 

					//环境光
					fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;
				  
					//3方向环境光
					d = d + splight*sign(mask.b);
				
					half3 ramp = tex2D(_ToonShade, float2(d, 1)).rgb;
					ramp = pow(ramp, 3)* mask.r;
		
					//分区域
					d = lerp(ramp.r, ramp.g, mask.g);
					ramp = lerp(i.color.rgb, fixed3(1.8, 1.8, 1.8), saturate(d));
				
						  //最终合成
						  half4 final;
						  final.a = col.a;
						  
						  half3 mainlight = col.rgb*lerp((ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb),ramp,_UILight) + i.lightColor.rgb+ col.rgb*diff*_DirectionalLightColor;
						  final.rgb = mainlight*_Color.rgb;

						  clip(i.color.a + 0.2 - _HideFace);

					  #ifdef CLIP_ON
						  clip(col.a - _CutOut);
					  #endif

						  
//?						  final.rgb *= (_Lighting + _HdrIntensity);

						  //单色判定
						  float isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
						  float3 grayCol = dot(final.rgb, float3(0.299, 0.587, 0.114));
						  final.rgb = lerp(final.rgb*_Color.rgb, grayCol.rgb, isGray);


#if _FOG_ON
						  UNITY_APPLY_FOG(i.fogCoord, final);
#endif
						//  final = max(0.1, final);
						  final.a = _BlendAlpha;// max(max(final.r, final.g), final.b) + min(min(final.r, final.g), final.b)) / 2;

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
