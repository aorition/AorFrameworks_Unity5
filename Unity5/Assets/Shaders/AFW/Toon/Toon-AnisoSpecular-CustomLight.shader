//@@@DynamicShaderInfoStart
//<readonly>卡通材质 (各向异性高光版) <支持自定灯光>
//@@@DynamicShaderInfoEnd
Shader "AFW/Toon/Toon - AnisoSpecular - CustomLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D) = "black" {}
		[Space(20)]
		_SunLightThreshold("SunLight Threshold",Range(0,1)) = 0.5
		_DarkColor("Dark Color", Color) = (0,0,0,1)
		_DarkThreshold ("Dark Threshold", Range(0,1)) = 1
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
		[Space(20)]
		_TintColor("Color", Color) = (1,1,1,1)
		[Space(10)]
		[Toggle] _Fog("Fog?", Float) = 0
		[Toggle]_HideFace("HideFace", Float) = 0
		_HideFaceThreshold("HideFace Threshold", Float) = 0
		[Space(10)]
		[Toggle] _Clip("Clip?", Float) = 0
		_CutOut("CutOut", Float) = 0
		[Space(10)]
		_Lighting("Lighting", Float) = 1
		[Space(20)]
		//OUTLINE
		_OutlineWidth("OutlineWidth", Float) = 1
		_OutLineColor("OutLineColor", Color) = (0,0,0,1)
		[Space(20)]
		_Specular ("Specular", Color) = (1,1,1,1)
		[Space(10)]
		_VolThreshold("Vol Threshold",Range(0,1)) = 0
		_VolPower("Vol Power",Range(0.000001, 3.0)) = 0.01
		[Space(20)]
		_RimLightColor("RimLight Color", Color) = (1,1,1,1)
		_RimLightThreshold("RimLight Threshold",Range(0,1)) = 0
		_RimLightPower("RimLight Power",Range(0.000001, 3.0)) = 0.01
		[Space(20)]
		_AmbientRimThreshold("AmbientRim Threshold",Range(0,1)) = 0.5
		_AmbientRimPower("AmbientRim Power",Range(0.000001, 3.0)) = 1.66
		[Space(20)]
		_HairLightRamp ("Hair Light Ramp", 2D) = "white" {}
		[Space(10)]
		_MainHairSpecularThreshold("Aniso Specular Threshold",Range(0,1)) = 0.5
		_MainHairSpecularSmooth ("Hair Specular Smooth",  float) = 1
		_MainHairSpecularOff ("Hair Specular Offset",  float) = 1
		[Space(10)]
		_FuHairSpecularThreshold("Aniso Specular Threshold",Range(0,1)) = 0.2
		_FuHairSpecularSmooth("Hair Specular Smooth 2",  float) = 1
		_FuHairSpecularOff("Hair Specular Offset 2",  float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "Lighting.cginc"  
			#include "AutoLight.cginc"

			#pragma target 2.0

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase

			#pragma shader_feature _FOG_ON
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _HIDEFACE_ON

			#pragma shader_feature NOSHADOWCASCADES
			#pragma shader_feature USECUSTOMLIGHT

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;

			fixed _SunLightThreshold;
			fixed _DarkThreshold;

			fixed _RampThreshold;
			fixed _RampSmooth;

			fixed4 _TintColor;
			fixed4 _DarkColor;
			fixed3 _Specular;
			fixed _Lighting;
			fixed _CutOut;
			fixed _HideFaceThreshold;

			fixed _VolThreshold;
			fixed _VolPower;

			fixed4 _RimLightColor;
			fixed _RimLightThreshold;
			fixed _RimLightPower;

			fixed _AmbientRimThreshold;
			fixed _AmbientRimPower;

			float _MainHairSpecularThreshold;
			fixed _MainHairSpecularSmooth;
			float _MainHairSpecularOff;

			float _FuHairSpecularThreshold;
			fixed _FuHairSpecularSmooth;
			float _FuHairSpecularOff;

			sampler2D _HairLightRamp;
			float4 _HairLightRamp_ST;

			#ifdef USECUSTOMLIGHT
				float4 _DirectionalLightDir;
				fixed4 _DirectionalLightColor;
			#endif

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4  color : COLOR;
				fixed3	normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				#ifdef _FOG_ON
					UNITY_FOG_COORDS(1)
				#endif
				half3 normal   : TEXCOORD2;
				LIGHTING_COORDS(3,4)
				half3 worldPos : TEXCOORD5;
				float2 hairLightUV:TEXCOORD6;
				float3 binormal : TEXCOORD7;
			};
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.hairLightUV = TRANSFORM_TEX(v.texcoord, _HairLightRamp);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o,o.pos);
				#endif
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				//求出沿着发梢到发根方向的切线
				half4 p_tangent = mul(unity_ObjectToWorld, v.tangent);
				o.binormal = cross(normalize(p_tangent).xyz, o.normal);
				return o;
			}

			inline fixed ShadowATTENUATION(v2f i){
				fixed atten = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
				#ifdef NOSHADOWCASCADES
					float4 shadowCoord = mul(unity_WorldToShadow[0],unityShadowCoord4(i.worldPos,1));
					float3 s = abs((shadowCoord - 0.5) * 2);
					atten = max(1-step(max(max(s.x, s.y), s.z), 1), atten); 
				#endif
				return atten;
			}
			
			float3 ShiftTangent(float3 T, float3 N, float shift)
			{
				float3 shiftedT = T + (shift * N);
				return normalize(shiftedT);
			}

			float HairSpecular(fixed3 halfDir, float3 tangent, float specularSmooth)
			{
				
				float dotTH = dot(tangent, halfDir);
				float sqrTH =max(0.01,sqrt(1 - pow(dotTH, 2)));
				float atten = smoothstep(-1,0, dotTH);
 
				//头发主高光值
				float specMain = atten * pow(sqrTH, specularSmooth);
				return specMain;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				#ifdef _HIDEFACE_ON
					clip(i.color.a - _HideFaceThreshold);
				#endif

				fixed4 col = tex2D(_MainTex, i.uv);

				fixed alpha = col.a * _TintColor.a;

				#ifdef _CLIP_ON
					clip(alpha - _CutOut);
				#endif
				
				fixed4 mask = tex2D(_MaskTex, i.uv);

				#ifdef USECUSTOMLIGHT

					fixed3 darkCol = lerp(col.rgb, _DarkColor, _DarkThreshold);
					darkCol = min(darkCol, darkCol *_DirectionalLightColor.w);

					col.rgb *= _DirectionalLightColor.w;
					col.rgb =  lerp(col.rgb, col.rgb * _DirectionalLightColor.rgb, _SunLightThreshold);

					float3 lightDir = normalize(_DirectionalLightDir.xyz);
				#else
					
					fixed3 darkCol = lerp(col.rgb, _DarkColor, _DarkThreshold);
					darkCol = min(darkCol, darkCol *_LightColor0.w);

					col.rgb *= _LightColor0.w;
					col.rgb =  lerp(col.rgb, col.rgb * _LightColor0.rgb, _SunLightThreshold);

					float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				#endif

				fixed3 normal = normalize(i.normal);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

				//环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				
				//shadow
				fixed atten = ShadowATTENUATION(i);

				fixed NL = dot(normal, lightDir);
				fixed NV = dot(normal, viewDir);

				//Toon Threshold
				fixed TexThreshold = mask.r - 0.5;
				fixed ndl = saturate(max(0, dot(normal, lightDir)*0.5 + 0.5) + TexThreshold);
				fixed d1 = smoothstep(_RampThreshold - _RampSmooth * 0.5, _RampThreshold + _RampSmooth * 0.5, ndl);
				d1 *= atten;

				fixed3 worldHalfDir = normalize(lightDir + viewDir);

				//头发高光图采样
				float3 speTex = tex2D(_HairLightRamp, i.hairLightUV);
				//头发主高光偏移
				//float3 ShiftTangent(float3 T, float3 N, float shift)
				float3 Ts =i.binormal + normal*_MainHairSpecularOff * speTex;
				//头发副高光偏移
				float3 Tf = i.binormal + normal*_FuHairSpecularOff * speTex;
				
				//头发主高光值
				float specMain = HairSpecular(worldHalfDir,Ts, _MainHairSpecularSmooth);
				float specFu = HairSpecular(worldHalfDir,Tf, _FuHairSpecularSmooth);
				float specFinal = max(specMain * _MainHairSpecularThreshold, specFu * _FuHairSpecularThreshold);
				fixed3 specular = _Specular.rgb * specFinal;

				//微体积
				fixed vol = max(pow(saturate(NV),1.0 / _VolPower), 0);
				//边缘光
				fixed rimScale = max(pow(saturate(1.0 - NV),1.0 / _RimLightPower), 0);
				fixed3 rim = _RimLightColor * rimScale * _RimLightThreshold;

				fixed ambiRimSacle =  max(pow(saturate(1.0 - NV),1.0 / _AmbientRimPower), 0);
				fixed3 ambiRim = ambient * ambiRimSacle * _AmbientRimThreshold;
				//Albedo
				fixed3 Albedo = lerp(darkCol, col, d1);

				//final
				fixed4 final = fixed4(Albedo + (Albedo * vol * _VolThreshold) + specular + rim, alpha);
				final *= _TintColor * _Lighting;
				// apply fog
				#ifdef _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, final);
				#endif

				return final;
			}
			ENDCG
		}//end Pass

		//LINEPASS
		UsePass "AFW/Toon/Toon - OutLine/LINEPASS"
	}//end SubShader

	FallBack "Mobile/Diffuse"

}//end Shader
