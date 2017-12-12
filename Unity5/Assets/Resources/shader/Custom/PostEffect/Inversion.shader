// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PostEffect/inversion" {
	Properties{
		_InversionPower("Inversion Power", Range(0,1)) = 1
		_InversionColor("Inversion Color", Color) = (1,1,1,1)
		_GrayAmount("Gray Amount", Range(0,1)) = 0

		_MainTex("Base (RGB)", 2D) = "white" {}
	    
	}

		Category{
			SubShader {
						Pass {
					ZTest Always
					Fog { Mode off }

		//CGPROGRAM
		//#pragma vertex vert_img
		//#pragma fragment frag
		//#pragma fragmentoption ARB_precision_hint_fastest 
		//#include "UnityCG.cginc"

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"


			sampler2D _MainTex;
			  float4 _MainTex_ST;

			  float _InversionPower;
			  float4 _InversionColor;
			  float _GrayAmount;
		

				struct v2f {
						float4  pos : SV_POSITION;
						float2  uv : TEXCOORD1;
					};

				//顶点函数没什么特别的，和常规一样
				v2f vert(appdata_base v)
				{
					v2f o;
					   o.pos = UnityObjectToClipPos(v.vertex);
					  o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
	
					return o;
				}



							  float4 frag(v2f i) : COLOR
				{
								  float4 mainCol = tex2D(_MainTex, i.uv);
								  fixed4 inverCol = (1 - mainCol);

								  fixed4 final;
								  final.a = mainCol.a;

								  final.rgb = lerp(mainCol.rgb, inverCol.rgb, _InversionPower) * _InversionColor;      //go to inversion

								  float3 grayCol = dot(final.rgb, float3(0.299, 0.587, 0.114));
								  final.rgb = lerp(final.rgb, grayCol.rgb, _GrayAmount);      //go to gray
 
							  return final;

				}



	ENDCG

			}


		}
	}

}