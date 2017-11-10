// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Light/Diffuse - Toon - TangentCheck" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask Tex", 2D) = "black" {}
	_NormalTex("NormalTex", 2D) = "white" {}
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
	_Outline("Outline Range", float) = 4
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

				float _Outline;
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
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.normal = normalize( mul(v.normal, (float3x3)unity_WorldToObject));
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
					half3 normal =i.normal;
					fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
					 //normalMap = normalize(normalMap);
				
					half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
					 normal = normalize(mul(normalMap.xyz, tbnMatrix));
					  return fixed4(i.Tangent.xyz, 1);
					

								}
							ENDCG
							}

	}

}
