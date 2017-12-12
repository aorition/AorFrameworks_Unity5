// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Custom/Light/Diffuse - Toon - Normal -line - Dragon" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask Tex", 2D) = "black" {}
	_NormalTex("NormalTex", 2D) = "white" {}
	_OutlineColor("Outline Color", Color) = (0,0,0,1)

	//[HideInInspector]_LowOutlineWidth("LowOutlineWidth", float) = 4
	_HightOutlineWidth("HightOutlineWidth", float) = 4

	_ToonShade("ToonShader", 2D) = "white" {}
	_HideFace("_HideFace", int) = 0
		[Toggle] _Fog("Fog?", Float) = 0
		 _Color("Color", Color) = (1,1,1,1)
		[HideInInspector]_Lighting("Lighting", float) = 1
		[HideInInspector] _CutOut("CutOut", float) = 0.1
	}
	
	SubShader {
		Tags {
		"RenderType" = "Transparent"
		}

		LOD 200

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

 

					float _HideFace;
					sampler2D _ToonShade;
					sampler2D _MaskTex;
					sampler2D _NormalTex;

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
					 	o.eyeDir = normalize(worldPos  - _WorldSpaceCameraPos.xyz);
					 
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
						half4 mask = tex2D(_MaskTex, i.uv0);

						//主光方向
						half3 lightDirection =  _RoleDirectionalLightDir.xyz;

						//法线
						half3 normal = i.normal;
						fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
						 normalMap = normalize(normalMap);

						half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
						normal = normalize(mul(normalMap.xyz, tbnMatrix));

						//卡通分界线
						half d = max(0, dot(normal, lightDirection));

						//高光
						 //half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  mask.b * 16 + 4),0)* sign(mask.b);

						//描边
						half eyeD = dot(normal, -i.eyeDir);

						//固有色
						fixed4 col = tex2D(_MainTex,i.uv0);

						//环境光
						fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;
						//3方向环境光
						d =   i.lightColor.r  ;  ////////////////////

						half3 ramp = tex2D(_ToonShade, float2(d, d)).rgb;

						ramp = pow(ramp, 3)*mask.r;
						//分区域
						d = lerp(ramp.r, ramp.g, mask.g);

						//return float4(d,d,d,1);
						ramp = lerp(i.color.rgb, fixed3(2, 2, 2), saturate(d));
						//return fixed4(d/2, d / 2, d / 2, 1);
						//最终合成
						half4 final;
						final.a = col.a;

						//half3 mainlight = col.rgb*(ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb+ ramp*i.lightColor.rgb+ ambientLight.rgb);
						//half3 mainlight = col.rgb*(ramp + i.lightColor.rgb + ambientLight.rgb)/2;    ///////////////////////////
						half3 mainlight = ( col.rgb*i.lightColor.rgb + col.rgb/5 + col.rgb*ramp + ambientLight.rgb)/1.8f;

						final.rgb = mainlight*_Color.rgb ;

						clip(i.color.a - _HideFace);
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

}
