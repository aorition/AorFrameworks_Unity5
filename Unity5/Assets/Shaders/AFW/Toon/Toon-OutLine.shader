//@@@DynamicShaderInfoStart
//<readonly>卡通材质（描边）
//@@@DynamicShaderInfoEnd
Shader "AFW/Toon/Toon - OutLine"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor("Color", Color) = (1,1,1,1)
		_HideFaceThreshold("HideFace Threshold", Float) = 0
		[Space(10)]
		[Toggle] _Clip("Clip?", Float) = 0
		[Toggle] _Fog("Fog?", Float) = 0
		[Toggle]_HideFace("HideFace", Float) = 0
		_HideFaceThreshold("HideFace Threshold", Float) = 0
		[HideInInspector] _CutOut("CutOut", Float) = 0.1
		_OutlineWidth("OutlineWidth", Float) = 1
		_OutLineColor("OutLineColor", Color) = (0,0,0,1)

		//此段可以将 Blend 选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 0

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		//LINEPASS
		Pass 
		{

			Name "LINEPASS"

			Tags{ "LightMode" = "Always" }

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]

			ZWrite On
			ZTest Less

			Cull Front
			Offset[_OutLineOffset],1

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma shader_feature _FOG_ON
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _HIDEFACE_ON

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			fixed _HideFaceThreshold;
			fixed4 _TintColor;
			fixed _CutOut;

			float _OutlineWidth;
			fixed4 _OutLineColor;
			//float _Factor=0.5;
			 
			struct a2v {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				half4 color : COLOR;
			};

			struct v2f 
			{
				float4 pos:POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v) 
			{
				v2f o;
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = UnityObjectToClipPos(v.vertex) ;

				float4 scrPos = ComputeScreenPos(o.pos);
				float3 offset = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				float2 scpos = (scrPos.xy / scrPos.w)*_ScreenParams.xy;
				scpos += normalize(offset.xy) * _OutlineWidth * v.color.b;
				scrPos.xy = scpos / _ScreenParams.xy * scrPos.w;
				#if defined(UNITY_HALF_TEXEL_OFFSET)
								scrPos.xy = (scrPos.xy - o.pos.w*0.5 * _ScreenParams.zw);
								scrPos.y = scrPos.y / _ProjectionParams.x;
								scrPos *= 2;
				#else
								scrPos.xy -= o.pos.w*0.5;
								scrPos.y = scrPos.y / _ProjectionParams.x;
								scrPos.xy *= 2;
				#endif
				o.pos.xy = scrPos.xy;

				return o;
			}

			fixed4 frag(v2f i) :COLOR
			{ 
				#ifdef _HIDEFACE_ON
					clip(i.color.a - _HideFaceThreshold);
				#endif

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed alpha = col.a * _TintColor.a;
				
				#ifdef _CLIP_ON
					clip(alpha - _CutOut);
				#endif
				
				_OutLineColor.a *= alpha;
				fixed4 final = _OutLineColor;

				#ifdef _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, final);
				#endif

				return final;
			}

			ENDCG
		}//end of pass

	}//end SubShader

}//end Shader
