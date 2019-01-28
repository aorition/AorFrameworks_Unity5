// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Custom/longzhu/Treasure" {
	Properties {
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask Tex", 2D) = "black" {}
	_NormalTex("NormalTex", 2D) = "white" {}

	_HightOutlineWidth("HightOutlineWidth", float) = 2

	_ToonShade("ToonShader", 2D) = "white" {}
	//_HideFace("_HideFace", int) = 0
	[Toggle] _Fog("Fog?", Float) = 0
	_Color("Color", Color) = (1,1,1,1)

	[HideInInspector]_Lighting("Lighting", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
	}
		SubShader{
			Tags {
			"RenderType" = "Transparent"
			}


		LOD 600
		pass {
		Name "LINEPASS"
		Tags{ "LightMode" = "Always" }
			Cull Front
			ZWrite On
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Assets/ObjectBaseShader.cginc"	

 
			//float _HideFace;
			float _HightOutlineWidth;
			float _Factor=0.5;

		struct v2f {
			float4 pos:POSITION;
			float2 uv0 : TEXCOORD0;
			half4 color : COLOR;
		};

		v2f vert(appdata_full v) {
			v2f o;
			o.color = v.color;
			float far = UnityObjectToClipPos(v.vertex).w;
			float3 dir = normalize(v.vertex.xyz);
			float3 dir2 = v.normal;
			float D = dot(dir,dir2);
			dir = dir*sign(D);
			dir = dir*_Factor + dir2*(1 - _Factor);
			v.vertex.xyz += dir*_HightOutlineWidth*0.0004* min(3,far);

			o.pos = UnityObjectToClipPos(v.vertex) ;
		
			o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
		 //	o.pos /= o.pos.w;
			//   o.uv0.x = o.pos.w;
			return o;
		}
		float4 frag(v2f i) :COLOR
		{ 
		 //	 return   i.uv0.x;
			fixed4 c = tex2D(_MainTex, i.uv0);
			c.rgb = lerp(fixed3(0,0,0),i.color*i.color*_Color,0.8);
			c.rgb *= _HdrIntensity + _DirectionalLightColor*_DirectionalLightDir.w + c.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;
			//clip(i.color.a + 0.2 - _HideFace);
			c.a = 1;
			return c;
		}
			ENDCG
	}//end of pass

			Pass {
				Name "COLORPASS"
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma multi_compile CLIP_OFF CLIP_ON
				#pragma shader_feature _FOG_ON
	 
				#pragma multi_compile_fog

				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/ObjectBaseShader.cginc"	

 
					//float _UILight;
					//float _HideFace;
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

						//half3	lightDirection = lerp(_RoleDirectionalLightDir.xyz,  half3(0.6,0.3, -1), _UILight);
						half3	lightDirection = half3(0.6, 0.3, -1);

						//法线
						half3 normal = i.normal;
						fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
						normalMap = normalize(normalMap);

						half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
						normal = normalize(mul(normalMap.xyz, tbnMatrix));

						//卡通分界线
						half d = max(0, dot(normal, lightDirection));
						//高光
						 half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  mask.b * 16 + 4),0);


						//固有色
						fixed4 col = tex2D(_MainTex,i.uv0);

						//环境光
						fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;
						//3方向环境光
						d = d + splight* sign(mask.b);
						half3 ramp = tex2D(_ToonShade, float2(d, d)).rgb;
						ramp = pow(ramp, 3)*mask.r;
						//分区域
						d = lerp(ramp.r, ramp.g, mask.g);
		
						ramp = lerp(i.color.rgb, fixed3(1.8, 1.8, 1.8), saturate(d));

						//最终合成
						half4 final;
						final.a = col.a;
						//return i.color;
						//half3 mainlight = col.rgb*lerp((ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb), ramp,_UILight) + i.lightColor.rgb+ col.rgb*splight*_DirectionalLightColor*0.2;
						half3 mainlight = col.rgb*ramp + i.lightColor.rgb + col.rgb*splight*_DirectionalLightColor*0.2;
						final.rgb = mainlight*_Color.rgb;
						//clip(i.color.a + 0.2 - _HideFace);
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
