// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/EdgeDraw"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _CircleTime;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 offset = 1 / float2(1280, 720) * 1.5;
				fixed4 color = tex2D(_MainTex, i.uv);
				fixed4 colorN = tex2D(_MainTex, i.uv + float2(0, offset.y));
				fixed4 colorW = tex2D(_MainTex, i.uv + float2(-offset.x, 0));
				fixed4 colorE = tex2D(_MainTex, i.uv + float2(offset.x, 0));
				fixed4 colorS = tex2D(_MainTex, i.uv + float2(0, -offset.y));

				fixed lumaM = color.a;
				fixed lumaN = colorN.a;
				fixed lumaW = colorW.a;
				fixed lumaE = colorE.a;
				fixed lumaS = colorS.a;

				fixed rangeMin = min(lumaM, min(min(lumaN, lumaW), min(lumaS, lumaE)));
				fixed rangeMax = max(lumaM, max(max(lumaN, lumaW), max(lumaS, lumaE)));
				fixed range = rangeMax - rangeMin;
				return range * float4(0,0.5,0,0) * ((sin(_CircleTime * 3.1415926  * 5.0 * 2) + 1.0) / 1.5 * 0.65 + 0.35) + color;
			}
			ENDCG
		}
	}
}
