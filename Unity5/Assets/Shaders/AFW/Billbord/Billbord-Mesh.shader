//@@@DynamicShaderInfoStart
//<readonly> Billbord for Mesh
//@@@DynamicShaderInfoEnd
Shader "AFW/Billbord/Billbord - Mesh" 
{

	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[Space(10)]
		_CutOut("CutOut", Float) = 0.7
	}

	SubShader 
	{

		Pass
		{

			Tags { "RenderType"="Opaque" }

			//LOD 200

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			float _CutOut;

			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD2;
			};

			v2f vert (appdata_base v)
			{
			    v2f o;
				
				float4x4 mvMat = UNITY_MATRIX_MV;
				float3 scale =  float3(length(float3(mvMat[0][0], mvMat[0][1], mvMat[0][2])), length(float3(mvMat[1][0], mvMat[1][1], mvMat[1][2])), length(float3(mvMat[2][0], mvMat[0][1], mvMat[0][2])));
				mvMat = float4x4(scale.x, 0, 0, mvMat[0][3],
											 0, scale.y, 0, mvMat[1][3],
											 0, 0, scale.z, mvMat[2][3],
											 0, 0, 0, 1);
				float4x4 mvpMat = mul(UNITY_MATRIX_P, mvMat);

			    o.pos = mul(mvpMat,v.vertex);
			  	o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float4 mainCol = tex2D(_MainTex, uv);

				clip(mainCol.a - _CutOut);

				return mainCol;
			}

			ENDCG

		}//end pass
	}//end SubShader 
}//end Shader
