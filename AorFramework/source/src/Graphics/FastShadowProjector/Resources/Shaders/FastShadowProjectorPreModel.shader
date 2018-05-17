// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fast Shadow Projector/PreModel"
{
	Properties{
		_Intensity("Intensity", float) = 0.6
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
				return UnityObjectToClipPos(v.vertex);
			}

			fixed4 frag() : COLOR
			{
				return _Intensity;
			}
			ENDCG
		}
	}

}