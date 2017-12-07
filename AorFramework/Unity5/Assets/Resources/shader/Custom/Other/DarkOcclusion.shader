Shader "Custom/Other/DarkOcclusion" {
	Properties{
	_Lum("Lum", float) = 0.5
	_Alpha("Alpha", float) = 1
	_MainTex("Tex", 2D) = "white"
}

	SubShader{

		Pass{
		Tags{ "RenderType" = "Transparent"}
		LOD 200
			Blend DstColor SrcColor
			Cull Off
			ZWrite Off
			ZTest Always

			CGPROGRAM
#pragma multi_compile FOG_OFF FOG_ON
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
		float _Alpha;
			float _Lum;
			sampler2D _MainTex;

		struct v2f {
			float4  pos : SV_POSITION;
			float2  texcoord : TEXCOORD0;
		};

		v2f vert(appdata_base v)
		{
			v2f o;

			//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = float4(v.vertex.xy *2, 0.5, 1);
			o.texcoord  = (o.pos + 1) / 2;
#ifdef UNITY_UV_STARTS_AT_TOP
			o.texcoord.y = -o.texcoord.y;
#endif
			return o;
		}

		float4 frag(v2f i) : COLOR
		{
			float4 tex =  tex2D(_MainTex, i.texcoord.xy);
			clip(_Alpha);
			return tex * _Lum * 0.5;// _Lum;
		}
		ENDCG
	}
	}
}
