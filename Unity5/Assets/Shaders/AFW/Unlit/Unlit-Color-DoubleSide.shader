//@@@DynamicShaderInfoStart
//<readonly>特效用 双面 Color
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - Color - DoubleSide" {
	
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_TintColor_f ("Front Color", Color) = (1,1,1,1)
		_TintColor_b ("Back Color", Color) = (1,1,1,1)
		_Lighting_f ("Front Lighting",  float) = 1
		_Lighting_b ("Lighting",  float) = 1
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
		//此段可以将 Blend 选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
    }
    SubShader {
		
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        
		//Front
		Pass
		{

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#pragma shader_feature _FOG_ON
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _USEGRAY_ON

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			fixed4 _TintColor_f;
			fixed _Lighting_f;

			fixed _gray_factor;
			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
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
				o.color = v.color;
				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o,o.pos);
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _TintColor_f * i.color * _Lighting_f;

				//先clip，再fog 不然会出错	
 				#ifdef CLIP_ON
					clip(col.a - _CutOut);
				#endif

				#ifdef _USEGRAY_ON
					fixed isGray = step(dot(_TintColor_f.rgb, fixed4(1, 1, 1, 0)), 0);
					col.rgb = dot(col.rgb, float3(0.299*_gray_factor, 0.587*_gray_factor, 0.114*_gray_factor));
				#endif

				// apply fog
				#ifdef _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, col);
				#endif

				return col;
			}
			ENDCG
		}
		
		//Back
		Pass
		{

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull Back

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#pragma shader_feature _FOG_ON
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _USEGRAY_ON

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			fixed4 _TintColor_b;
			fixed _Lighting_b;

			fixed _gray_factor;
			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
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
				o.color = v.color;
				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o,o.pos);
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _TintColor_b * i.color * _Lighting_b;

				//先clip，再fog 不然会出错	
 				#ifdef CLIP_ON
					clip(col.a - _CutOut);
				#endif

				#ifdef _USEGRAY_ON
					fixed isGray = step(dot(_TintColor_b.rgb, fixed4(1, 1, 1, 0)), 0);
					col.rgb = dot(col.rgb, float3(0.299*_gray_factor, 0.587*_gray_factor, 0.114*_gray_factor));
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

    FallBack "Mobile/Particles/Alpha Blended"

}
