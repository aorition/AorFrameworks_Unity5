Shader "Hidden/BaseBoxBlur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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

			float _BlurRadius;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				o.uv1 = v.uv + _BlurRadius * float2(1, 1);
				o.uv2 = v.uv + _BlurRadius * float2(-1, 1);
				o.uv3 = v.uv + _BlurRadius * float2(-1, -1);
				o.uv4 = v.uv + _BlurRadius * float2(1, -1);

				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);
				col += tex2D(_MainTex, i.uv1);
				col += tex2D(_MainTex, i.uv2);
				col += tex2D(_MainTex, i.uv3);
				col += tex2D(_MainTex, i.uv4);

				fixed4 final = col / 5;
				return final;
			}
			ENDCG
		}

	}
}
