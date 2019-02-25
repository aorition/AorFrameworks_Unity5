Shader "Hidden/RppGlow"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_GlowTex("Glow Tex", 2D) = "white" {}
		_Offest("Offset",float) = 0.05
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;

			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;

			};

			float _TexSizeX;
			float _TexSizeY;
			float _BlurRadius;
			float _Offest;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float2 _TexSize = float2(_TexSizeX, _TexSizeY);
				float2 _Offset2 = float2(0, _Offest);
				o.uv1 = v.uv + _BlurRadius * _TexSize * float2(1, 1) - _Offset2;
				o.uv2 = v.uv + _BlurRadius * _TexSize * float2(-1, 1) - _Offset2;
				o.uv3 = v.uv + _BlurRadius * _TexSize * float2(-1, -1) - _Offset2;
				o.uv4 = v.uv + _BlurRadius * _TexSize * float2(1, -1) - _Offset2;

				return o;
			}

			sampler2D _MainTex;
			sampler2D _GlowTex;

			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);
				
				fixed4 over = fixed4(0, 0, 0, 0);
				over += tex2D(_GlowTex, i.uv);
				over += tex2D(_GlowTex, i.uv1);
				over += tex2D(_GlowTex, i.uv2);
				over += tex2D(_GlowTex, i.uv3);
				over += tex2D(_GlowTex, i.uv4);


				fixed4 final = over;
				//final.a = lerp(col.a, 0, col.a);
				final.rgb = lerp(over.rgb, fixed3(0, 0, 0), col.a);

				return final;
			}
			ENDCG
		}

	}
}
