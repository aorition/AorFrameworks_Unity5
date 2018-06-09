Shader "FrameBase/Billbord/Billbord - Mesh" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Pass{
			Tags { "RenderType"="Opaque" }
			LOD 200
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets/ObjectBaseShader.cginc"

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
			      
			float4 frag (v2f i) : COLOR
			{
				//float4x4 wMat= _Object2World;
				//float3 wPos = float3(wMat[0][3], wMat[1][3], wMat[2][3]);
				//float3 wCamPos = _WorldSpaceCameraPos;
				//float2 camToObjDir = normalize((wPos - wCamPos).xz);
				//float ac = acos(camToObjDir.x);
				//return (ac * sign(camToObjDir.y)+ 3.1415926) / 3.1415926 / 2;
				float2 uv = i.uv;
				//uv.x /= 16;
				//uv.x += (floor(_Time.y * 30) / 16.0);

				float4 mainCol = tex2D(_MainTex, uv);
				if (mainCol.a < 0.7)
				{
					discard;
				}

				return mainCol;
			}
			ENDCG
		}
	} 
}
