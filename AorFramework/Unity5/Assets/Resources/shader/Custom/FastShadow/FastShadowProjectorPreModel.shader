Shader "Fast Shadow Projector/PreModel"
{
	Properties{
		_Intensity("Intensity", float) = 0
}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

			Pass{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

			float _Intensity;

			struct v2f {
				float4 vertex : SV_POSITION;
			};

			float4 vert(appdata_tan v) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, v.vertex);
			}

			fixed4 frag() : COLOR
			{
				return fixed4(_Intensity,_Intensity,_Intensity,1);
			}
			ENDCG
		}
	}

}