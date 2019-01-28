//@@@DynamicShaderInfoStart
//<readonly>无光照Shader <Gray><Fog><Clip>
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - Color"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Color", Color) = (1,1,1,1)
		_Lighting ("Lighting",  float) = 1
		[Space(10)]
		[Toggle] _Fog("Fog?", Float) = 0
		[Toggle] _Clip("Clip?", Float) = 0
		_CutOut("CutOut", Float) = 0.1
		[Space(10)]
		[Toggle] _useGray("Use Gray?", Float) = 0
		_gray_factor("Gray Factor", Float) = 1
		[Space(10)]
		//此段可以将 ZWrite 选项暴露在Unity的Inspector中
		[Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 0
		//此段可以将 Ztest  选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 8
		//此段可以将 Cull  选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
		//此段可以将 Blend 选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#pragma shader_feature _FOG_ON
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _USEGRAY_ON

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			fixed4 _TintColor;
			fixed _Lighting;

			fixed _gray_factor;
			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				#ifdef _FOG_ON
					UNITY_FOG_COORDS(1)
				#endif
				float4 pos : SV_POSITION;
			};
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o,o.pos);
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _TintColor;

				#ifdef _USEGRAY_ON
					fixed isGray = step(dot(_TintColor.rgb, fixed4(1, 1, 1, 0)), 0);
					col.rgb = dot(col.rgb, float3(0.299*_gray_factor, 0.587*_gray_factor, 0.114*_gray_factor));
				#endif

				//先clip，再fog 不然会出错	
 				#ifdef CLIP_ON
					clip(col.a - _CutOut);
				#endif

				// apply fog
				#ifdef _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, col);
				#endif

				return col;
			}
			ENDCG
		}
	}
}
