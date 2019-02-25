// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FShadowOutput" {
	Properties{
		_Color("Color", Color) = (0,0,0,1)
	}

	SubShader
	{
		//Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "FColorOutput" }
		Tags{ "Queue" = "Transparent" "RenderType" = "FColorOutput" }
		LOD 100

		Fog{ Mode Off }
		Cull Off
		ZWrite Off

		//ZTest Always
		//Blend One OneMinusSrcAlpha
		//Blend SrcAlpha OneMinusSrcAlpha
		
		Lighting Off

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

			sampler2D _MainTex;
			fixed4 _Color;

			float _FO_offsetX;
			float _FO_offsetY;
			float _FO_offsetW;
			float _FO_Power;

			float _FO_PxThreshold;

			v2f vert(appdata v)
			{
				v2f o;
				float4x4 offsetMatrx = {
						1, 0, 0, 0,
						_FO_offsetX, 1, _FO_offsetW, 0,
						0, 0, 1, 0,
						0, 0, 0, 1
				};
				fixed4 _vertexScale = fixed4(1, _FO_offsetY, 1, 1);
				float4 pos = mul(v.vertex, offsetMatrx);
				o.vertex = UnityObjectToClipPos(pos * _vertexScale);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);
			float lm =  1 - _FO_Power;
				lm = (col.r <= _FO_PxThreshold && col.g <= _FO_PxThreshold && col.b <= _FO_PxThreshold) ? 1 : lm;
				return min(1, max(0, fixed4(lm, lm, lm, 1)));

			}

			ENDCG
		}
	}
}