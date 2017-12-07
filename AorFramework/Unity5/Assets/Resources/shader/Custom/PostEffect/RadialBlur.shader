// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PostEffect/RadiaBlur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Level("Level",Range(1,100)) = 50
		_CenterX("Center.x",Range(0,1)) = 0.5
		_CenterY("Center.y",Range(0,1)) = 0.5
	}
		Category{
			SubShader{
			Pass{
			ZTest Always
			Fog{ Mode off }
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
	float4 _MainTex_ST;
	float _Level;
	fixed _CenterX;
	fixed _CenterY;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
 
	//	UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		//	return fixed4(1,0,0,1);
		//设置径向模糊的中心位置，一般来说都是图片重心（0.5，0.5）
		fixed2 center = fixed2(_CenterX,_CenterY);

		//计算像素与中心点的距离，距离越远偏移越大
		fixed2 uv = i.uv - center;
		float3 col1 = fixed3(0,0,0);
		_Level = _Level/1000;
 
		//根据设置的level对像素进行叠加，然后求平均值
		fixed2 duv = 0;

		for (int l = 0; l < 10; l++)
		{
			duv = min(1, max(0, uv*(1 - _Level * l) + center));
			col1 += tex2D(_MainTex, duv).rgb;
		}

		fixed4 col = fixed4(col1.rgb * 0.1,1);

		return  min(1, max(0, col));
	}
		ENDCG
	}
	}
		}
}
