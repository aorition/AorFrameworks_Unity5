Shader "Custom/SpritePhoto" 
{
	Properties{
		_MainTex ("MainTex", 2D) = "white" {}
		_Lum("Lum", Range(0, 1)) = 0.5
	}

	SubShader{
		Tags{ "RenderType" = "Transparent" }
		LOD 100
		Blend srcalpha oneminussrcalpha

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Lum;
			float4 _MainTex_ST;

			struct input_data {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float4  color : COLOR;
			};

			v2f vert(input_data v)
			{
				v2f o;

				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);
				float gray = Luminance(color) * _Lum;
				float exposure = i.color.r;
				float lerpValue = i.color.a;

				float3 finial = lerp(color.xyz, float3(gray, gray, gray), lerpValue) + exposure;
				return float4(finial, color.a);
			}
			ENDCG
		}
	}

}