// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/longzhu/Chest" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_SpPower("SpPower", float) = 1
	_SpRange("SpRange", float) = 12
	_Color("Color", Color) = (1,1,1,1)
	_Lighting("Lighting", float) = 1

	[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
	[HideInInspector] _CutOut("CutOut", float) = 0.1

	}
		SubShader{
				Tags {
					"RenderType" = "Transparent"
				}

		LOD 200
		Pass {
			Tags { "LightMode" = "ForwardBase" }
			Cull[_Cull]
			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	

				float _SpRange;
				float _SpPower;
	 
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					half4 normal   : TEXCOORD1;
					half3 eyeDir   : TEXCOORD2;
					fixed3 lightColor : TEXCOORD5;    //点光
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.pos = UnityObjectToClipPos(v.vertex);
	 
					o.normal.xyz = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
					o.normal.w = 0;
			 
					half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
					o.lightColor = Shade4PointLights(worldPos, o.normal);

					return o;

					}
				fixed4 frag(v2f i) : COLOR {

					//主光方向
					half3 lightDirection = _DirectionalLightDir.xyz;

					//法线
					half3 normal = i.normal;

					//高光
					half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))), _SpRange),0);
					splight *= _SpPower;

					//固有色
					fixed4 col = tex2D(_MainTex,i.uv0);
 
					half4 final;
					final.a = min(col.a, _Color.a);


					half3 mainlight = col.rgb + splight*col.rgb + col.rgb* i.lightColor.rgb*3;

					final.rgb = mainlight * _Lighting;

					#ifdef CLIP_ON
						clip(col.a - _CutOut);
					#endif

						float isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
						float3 grayCol = dot(final.rgb, float3(0.299, 0.587, 0.114));
						final.rgb = lerp(final.rgb*_Color.rgb, grayCol.rgb, isGray);


					return  final;

					}
				ENDCG
				}

	}


}
