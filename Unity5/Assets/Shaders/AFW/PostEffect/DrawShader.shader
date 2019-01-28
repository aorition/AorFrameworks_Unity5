Shader "Hidden/PostEffect/DrawShader" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskColor("_MaskColor", Color) = (0,0,0,0)
		_CurveTex("_CurveTex", 2D) = "Black" {}
		_Exposure("_Exposure", float) = 0
	}

	SubShader
	{

		Pass {
			
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _SkyTex;
			sampler2D _CurveTex;
			
			float4 _MaskColor;
			float _Exposure;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD2;
			};

			//顶点函数没什么特别的，和常规一样
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				
				float4 mainCol = tex2D(_MainTex, i.uv);

				float2 uv = i.uv;
				// #if UNITY_UV_STARTS_AT_TOP
					// uv.y = 1 - uv.y;
				// #endif

				fixed4 curveCol = tex2D(_CurveTex, uv);
				mainCol.rgb = mainCol.rgb + curveCol.rgb + _MaskColor.rgb;
				return  mainCol*_Exposure;
			}

			ENDCG
		}//end pass
	}//end SubShader
}//end Shader