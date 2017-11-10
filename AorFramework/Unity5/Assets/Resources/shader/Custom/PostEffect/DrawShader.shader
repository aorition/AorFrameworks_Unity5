// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PostEffect/DrawShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskColor("_MaskColor", Color) = (0,0,0,0)
		_HdrTex("_HdrTex", 2D) = "white" {}
		_CurveTex("_CurveTex", 2D) = "Black" {}
		_Exposure("_Exposure", float) = 0

	}

		CGINCLUDE

#include "UnityCG.cginc"


			sampler2D _MainTex;
		sampler2D _SmallTex;

		sampler2D _SkyTex;
		sampler2D _CurveTex;
		sampler2D _HdrTex;
		float4 _MainTex_ST;
		float4 _MaskColor;
		float _Exposure;
		float4 _HdrParams;


		struct v2f {
			float4  pos : SV_POSITION;
			float2  uv : TEXCOORD2;
		};


		//顶点函数没什么特别的，和常规一样
		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}

		static const half curve[7] = { 0.0205, 0.0855, 0.232, 0.324, 0.232, 0.0855, 0.0205 };  // gauss'ish blur weights

		float4 fragAdaptive(v2f i, float4 color)
		{


			float avgLum =max(tex2D(_SmallTex, i.uv).x - _HdrParams.z,0);


			//画面最暗的时候不会调整到太暴
			//avgLum = max(avgLum, 0.3);

			//当前画面亮度最低0.000001;
			// return fixed4(avgLum, avgLum, avgLum, 1);
			float cieLum = max(0.000001, Luminance(color.rgb)); //ToCIE(color.rgb);
																// return fixed4(_HdrParams.z, _HdrParams.z, _HdrParams.z, 1);

			float lumScaled = cieLum / (0.001 + avgLum.x);
			lumScaled = lumScaled / (1.0f + lumScaled);

			//float3	hdrColor = color.rgb * (lumScaled / cieLum);
			color.rgb = saturate(color.rgb*lumScaled);
			//color.rgb = lerp(hdrColor, color.rgb, avgLum-0.3);

			return fixed4(color.rgb+fixed3(_HdrParams.g, _HdrParams.g, _HdrParams.g), 1);

		}


		ENDCG

			SubShader{
				Pass {
						ZTest Always Cull Off ZWrite Off
						Fog { Mode off }

						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag
						#include "UnityCG.cginc"

						float4 frag(v2f i) : COLOR
					  {
						 float4 mainCol = tex2D(_MainTex, i.uv);
						 fixed4 curveCol = tex2D(_CurveTex, i.uv);
						 mainCol.rgb = mainCol.rgb + curveCol.rgb + _MaskColor.rgb;
						 return  mainCol*_Exposure;
						 }



			 ENDCG

					 }
					 Pass{
							 ZTest Always Cull Off ZWrite Off
							 Fog{ Mode off }

							 CGPROGRAM
			 #pragma vertex vert
			 #pragma fragment frag
			 #include "UnityCG.cginc"






						 float4 frag(v2f i) : COLOR
						 {
							 float4 mainCol = tex2D(_MainTex, i.uv);

							 fixed4 curveCol = tex2D(_CurveTex, i.uv);
							 mainCol.rgb = mainCol.rgb + curveCol.rgb + _MaskColor.rgb;
							 mainCol = fragAdaptive(i, mainCol);
							 return  mainCol*_Exposure;
						 }



							 ENDCG

						 }


		}

}