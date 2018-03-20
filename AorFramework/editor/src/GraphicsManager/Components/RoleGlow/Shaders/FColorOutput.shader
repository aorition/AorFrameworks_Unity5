// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FColorOutput" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)

		_fPower("Fill Power", Float) = 1
		_fColor("Fill Color (RGB)", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "FColorOutput" }
		LOD 100

		Fog{ Mode Off }
		Cull Off
		ZWrite Off
		//ZTest Always
		//Blend One OneMinusSrcAlpha
		Blend SrcAlpha OneMinusSrcAlpha
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
			
			float _fPower;
			float4 _fColor;

			v2f vert(appdata v)
			{
				v2f o;
				float4 offset = float4(0, -0.03, 0, 0);//向上偏移
				o.vertex = UnityObjectToClipPos(v.vertex) + offset;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(_fColor.rgb * _Color.rgb * _fPower, tex2D(_MainTex, i.uv).a);
			}
			ENDCG
		}
	}
}