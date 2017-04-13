Shader "Custom/Other/DarkOcclusion" {
	Properties{
	_Lum("Lum", float) = 0.5
}

	SubShader{

		Pass{
		Tags{ "RenderType" = "Overlay"}
		LOD 200
			Blend DstColor Zero
			Cull Front
			ZWrite Off
			ZTest Always

			CGPROGRAM
#pragma multi_compile FOG_OFF FOG_ON
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			float _Lum;

		struct v2f {
			float4  pos : SV_POSITION;
		};

		v2f vert(appdata_base v)
		{
			v2f o;

			//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = float4(v.vertex.xy *2, 0, 1);
			return o;
		}

		float4 frag(v2f i) : COLOR
		{
			return _Lum;
		}
		ENDCG
	}
	}
}
