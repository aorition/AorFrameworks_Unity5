//@@@DynamicShaderInfoStart
//<readonly>卡通材质 
//@@@DynamicShaderInfoEnd
Shader "AFW/Toon/Toon - Base"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DarkTex("DarkTex", 2D) = "black" {}
		_MaskTex("Mask Tex", 2D) = "black" {}
		[Space(20)]
		_SunLightThreshold("SunLight Threshold",Range(0,1)) = 0.5
		_DarkThreshold ("Dark Threshold", Range(0,1)) = 1
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
		[Space(20)]
		_TintColor("Color", Color) = (1,1,1,1)
		_Specular ("Specular", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range(0.0,2)) = 1
		_Gloss ("Gloss", Float) = 1280000
		_SpecSmooth("Specular Smoothing", Range(0.001,1)) = 0.1
		_SpecToonThreshold("Specular Toon Threshold", Range(0,1)) = 0
		[Space(10)]
		[Toggle] _Fog("Fog?", Float) = 0
		[Toggle]_HideFace("HideFace", Float) = 0
		_HideFaceThreshold("HideFace Threshold", Float) = 0
		[Space(10)]
		[Toggle] _Clip("Clip?", Float) = 0
		_CutOut("CutOut", Float) = 0
		[Space(10)]
		[HideInInspector]_Lighting("Lighting", Float) = 1
		[Space(20)]
		//OUTLINE
		_OutlineWidth("OutlineWidth", Float) = 1
		_OutLineColor("OutLineColor", Color) = (0,0,0,1)
		[Space(20)]
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
		//此段可以将 ZWrite 选项暴露在Unity的Inspector中
		[Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 1
		//此段可以将 Ztest  选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		//此段可以将 Cull  选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
		//此段可以将 Blend 选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			
			Tags {"LightMode" = "ForwardBase"}

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DarkTex;
			sampler2D _MaskTex;

			fixed _SunLightThreshold;
			fixed _DarkThreshold;

			fixed _RampThreshold;
			fixed _RampSmooth;

			fixed4 _TintColor;
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

			fixed3 _Specular;
			fixed _Shininess;
			fixed _Gloss;

			fixed _SpecSmooth;
			fixed _SpecToonThreshold;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4  color : COLOR;
				fixed3	normal : NORMAL;
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
			};
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o,o.pos);
				#endif
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed ShadowATTENUATION(v2f i){
				fixed atten = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
				float4 shadowCoord = mul(unity_WorldToShadow[0],unityShadowCoord4(i.worldPos,1));
				float3 s = abs((shadowCoord - 0.5) * 2);
				atten = max(1-step(max(max(s.x, s.y), s.z), 1), atten); 
				return atten;
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

				col.rgb *= _LightColor0.w;
				col.rgb =  lerp(col.rgb, col.rgb * _LightColor0.rgb, _SunLightThreshold);
				fixed3 darkCol =tex2D(_DarkTex, i.uv).rgb;
				darkCol = min(darkCol, darkCol *_LightColor0.w);
				fixed4 mask = tex2D(_MaskTex, i.uv);

				fixed3 normal = normalize(i.normal);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				//环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				
				//shadow
				fixed atten = ShadowATTENUATION(i);

				//darkCol = lerp(fixed3(0,0,0),darkCol,_LightColor0.w);
				darkCol = lerp(col.rgb,darkCol,_DarkThreshold);
				//col.rgb = lerp(darkCol, col.rgb, _LightColor0.w);

				fixed NL = dot(normal, lightDir);
				fixed NV = dot(normal, viewDir);

				//Toon Threshold
				fixed TexThreshold = mask.r - 0.5;
				fixed ndl = saturate(max(0, dot(normal, lightDir)*0.5 + 0.5) + TexThreshold);
				fixed d1 = smoothstep(_RampThreshold - _RampSmooth * 0.5, _RampThreshold + _RampSmooth * 0.5, ndl);
				d1 *= atten;

				//specular
				fixed gloss = _Gloss * mask.b + 2; 
                //fixed3 R = 2 * dot(normal, lightDir) * normal - lightDir; //phong               //反射向量
				//fixed3 R = normalize(reflect(-L, N));			      //反射向量(算法2)
                float3 H = normalize(viewDir + lightDir);    //blinnphong          //半角向量：点到光源+点到摄像的单位向量，平均值
                //fixed specPower = pow(saturate(dot(R, viewDir)), gloss) * _Shininess;//phong
                fixed specPower = pow(saturate(dot(H, normal)), gloss) * _Shininess;  //blinnphong
				fixed3 specular = max(_Specular.rgb * specPower * mask.g, 0);
				fixed3 specular2 = smoothstep(0.5 - _SpecSmooth*0.5, 0.5 + _SpecSmooth*0.5, specular);
				specular = lerp(specular,specular2,_SpecToonThreshold);


				//微体积
				fixed vol = max(pow(saturate(NV),1.0 / _VolPower), 0);
				//边缘光
				fixed rimScale = max(pow(saturate(1.0 - NV),1.0 / _RimLightPower), 0);
				fixed3 rim = _RimLightColor * rimScale * _RimLightThreshold;

				fixed ambiRimSacle =  max(pow(saturate(1.0 - NV),1.0 / _AmbientRimPower), 0);
				fixed3 ambiRim = ambient * ambiRimSacle * _AmbientRimThreshold;
				darkCol += ambiRim;

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
