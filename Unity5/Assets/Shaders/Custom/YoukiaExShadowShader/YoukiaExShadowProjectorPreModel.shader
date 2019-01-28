// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "YoukiaExShadowProjector/PreModel"
{
	Properties{
		_Intensity("Intensity", float) = 0
	}

	SubShader{
		//Tags{ "RenderType" = "Opaque" }
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
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
				return fixed4(_Intensity, _Intensity, _Intensity, 1);
			}
			ENDCG
		}
	}
}