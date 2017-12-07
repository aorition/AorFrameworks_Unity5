// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Other/DigitalProjector"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D) = "black" {}
		_NormalTex("NormalTex", 2D) = "white" {}
		_EffectTex("EffectTex", 2D) = "white" {}
		//	_OutlineColor("Outline Color", Color) = (0,0,0,1)

		_ToonShade("ToonShader", 2D) = "white" {}
		_HideFace("_HideFace", int) = 0
		[Toggle] _Fog("Fog?", Float) = 0
		_Color("Color", Color) = (1,1,1,1)

		_LowOutlineWidth("LowOutlineWidth", float) = 1.33333
		_LowOutlineLighting("LowOutlineLighting", float) = 0.55

		_EffectTexScale("EffectTexScale", float) = 3.33

		_ColorP_R("ColorP_R", float) = 0.1
		_ColorP_G("ColorP_G", float) = 0.75
		_ColorP_B("ColorP_B", float) = 4

		[HideInInspector]_Lighting("Lighting", float) = 1
		[HideInInspector] _CutOut("CutOut", float) = 0.1
	}
	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{

		Tags{ "LightMode" = "ForwardBase" }
		
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		
		#pragma multi_compile CLIP_OFF CLIP_ON
		#pragma shader_feature _FOG_ON

		#pragma multi_compile_fog

		#pragma vertex vert
		#pragma fragment frag
		#include "Assets/ObjectBaseShader.cginc"	


			float _UILight;
			float _HideFace;
			sampler2D _ToonShade;
			sampler2D _MaskTex;
			sampler2D _NormalTex;
			sampler2D _EffectTex;

			float _ColorP_R;
			float _ColorP_G;
			float _ColorP_B;

			float _LowOutlineWidth;
			float _LowOutlineLighting;

			float _EffectTexScale;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				half3 normal   : TEXCOORD1;
				half3 eyeDir   : TEXCOORD3;
				fixed3 lightColor : TEXCOORD4;
				half4 Tangent : TEXCOORD5;
				half3 BiNormal : TEXCOORD6;
				half4 dPos: TEXCOORD7;
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
				o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);

				o.lightColor = Shade4PointLights(worldPos, o.normal);
				o.Tangent = normalize(float4(mul(v.tangent.xyz, (float3x3)unity_WorldToObject), v.tangent.w));
				o.BiNormal = cross(o.normal, o.Tangent) * o.Tangent.w;

				o.dPos = v.vertex;

		#if _FOG_ON
				UNITY_TRANSFER_FOG(o, o.pos);
		#endif

				return o;

			}
			fixed4 frag(v2f i) : COLOR{

				//half4 eff = tex2D(_EffectTex, float2(0.5, i.dPos.x * _EffectTexScale));
				half4 eff = tex2D(_EffectTex, float2(0.5, i.eyeDir.y * _EffectTexScale));

				//控制贴图
				half4 mask = tex2D(_MaskTex, i.uv0);
				//	_UILight =1;
				//主光方向
				half3	lightDirection = lerp(_RoleDirectionalLightDir.xyz,  half3(0.6,0.3, -1), _UILight);

				//法线
				half3 normal = i.normal;
				fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
				normalMap = normalize(normalMap);
				half3x3 tbnMatrix = half3x3(i.Tangent.xyz, i.BiNormal, normal);
				normal = normalize(mul(normalMap.xyz, tbnMatrix));

				//卡通分界线
				half d = max(0, dot(normal, normalize(lightDirection)));
				half diff = pow(d, 2) * 0.2;
				//return fixed4(diff, diff, diff,1);
				//高光
				half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  mask.b * 16 + 2),0);
				// return fixed4(splight, splight, splight,1);

				//描边
				half eyeD = dot(normal, -i.eyeDir);
				half outlight = pow(saturate(1 - eyeD), _LowOutlineWidth)*mask.a;
				outlight = floor(outlight * 2);

				//固有色
				fixed4 col = tex2D(_MainTex,i.uv0);

				//环境光
				fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz;

				//3方向环境光
				d = d + splight* sign(mask.b);
				half3 ramp = tex2D(_ToonShade, float2(d, 0.5)).rgb;
				ramp = pow(ramp, 3)*mask.r;
				//分区域
				d = lerp(ramp.r, ramp.g, mask.g);
				ramp = lerp(i.color.rgb, fixed3(1.8, 1.8, 1.8), saturate(d));

				//最终合成
				half4 final;
				final.a = col.a;
				//UI界面没辉光,用高光补强 
				half3 mainlight = col.rgb*lerp((ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + ambientLight.rgb), ramp, _UILight) + i.lightColor.rgb + col.rgb*diff*_DirectionalLightColor;


				final.rgb = mainlight*_Color.rgb;
				final.rgb += outlight * _LowOutlineLighting;

				clip(i.color.a + 0.2 - _HideFace);
		#ifdef CLIP_ON
				clip(col.a - _CutOut);
		#endif
		#if _FOG_ON
				UNITY_APPLY_FOG(i.fogCoord, final);
		#endif

				final.a *= (eff.a + 0.2) * (1 - i.dPos.x);
				final.rgb += eff.rgb;

				final.r *= _ColorP_R;
				final.g *= _ColorP_G;
				final.b *= _ColorP_B;

				final.a = max(0, (min(1, final.a)));
				final.r = max(0, (min(1, final.r)));
				final.g = max(0, (min(1, final.g)));
				final.b = max(0, (min(1, final.b)));

				return  final;

			}
				ENDCG
			}
	}
}
