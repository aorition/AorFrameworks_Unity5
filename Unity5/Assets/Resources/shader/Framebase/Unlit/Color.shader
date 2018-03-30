﻿//@@@DynamicShaderInfoStart
//<Readonly>自发光贴图材质 支持上色和亮度 一般特效贴图用此shader
//@@@DynamicShaderInfoEnd

Shader "AorFrame/Unlit/Color" {

	Properties{

		_MainTex("Texture", 2D) = "white" { }
		_TintColor("Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		[Space(24)]

		[Toggle(_CLIP_ON)] _ClipToggle("Clip?", Float) = 0
		[HideInInspector]_CutOut("CutOut", float) = 0.1
//		[Toggle(_HASNIGHT_ON)] _hasNightToggle("hasNight?", Float) = 0
		[Toggle(_FOG_ON)] _hasNightToggle("Fog?", Float) = 0
		[Space(24)]

		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
		[Space(12)]
		[Enum(Off,0,On,1)] _zWrite("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 0
		[Space(12)]
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
	}

	SubShader{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	//		#include "Assets/ObjectBaseShader.cginc"
			#include "UnityCG.cginc"
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _HASNIGHT_ON
			#pragma shader_feature _FOG_ON
		#if _FOG_ON
			#pragma multi_compile_fog
		#endif
			
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _TintColor;
			float _Lighting;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float4 color : COLOR;
		#if _FOG_ON
				UNITY_FOG_COORDS(2)
		#endif
			};

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord:TEXCOORD0;
				float4 color : COLOR;
			};

			//顶点函数没什么特别的，和常规一样 
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.color = v.color;

		#if _FOG_ON
				UNITY_TRANSFER_FOG(o, o.pos);
		#endif
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float4 col = tex2D(_MainTex,i.uv);

				col.rgb *= _Lighting;

				float isGray = step(dot(_TintColor.rgb, fixed4(1, 1, 1, 0)), 0);

				float3 grayCol = dot(col.rgb, float3(0.299, 0.587, 0.114));

				col.rgb = lerp(col.rgb* _TintColor*i.color, grayCol.rgb, isGray);

//		#ifdef _HASNIGHT_ON
	//			col.rgb *= _HdrIntensity + _DirectionalLightColor*_DirectionalLightDir.w + col.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
//		#endif

		//先clip，再fog 不然会出错	
		#ifdef CLIP_ON
				clip(col.a - _CutOut);
		#endif

		#if _FOG_ON
				UNITY_APPLY_FOG(i.fogCoord, col);
		#endif

				col.a *= _TintColor.a*i.color.a;

				return max(col, 0);
			}

			ENDCG
		}


	}
}