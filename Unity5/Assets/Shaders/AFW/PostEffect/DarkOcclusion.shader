Shader "Hidden/PostEffect/DarkOcclusion" 
{
	
	Properties
	{
		_MainTex("Tex", 2D) = "white"
		_Alpha("Alpha", float) = 1
		_Lum("Lum", float) = 0.5
	}

	SubShader
	{

		Pass
		{
			Tags{ "RenderType" = "Transparent"}
			
			LOD 200
			
			Blend DstColor SrcColor
			Cull Off
			ZWrite Off
			ZTest Always

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma multi_compile FOG_OFF FOG_ON
			#pragma vertex vert
			#pragma fragment frag

			float _Alpha;
			float _Lum;
			sampler2D _MainTex;

			struct v2f 
			{
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

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex =  tex2D(_MainTex, i.texcoord.xy);
				clip(_Alpha);
				return tex * _Lum * 0.5;// _Lum;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader