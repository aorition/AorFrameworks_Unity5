// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//简单的UV双色控制
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-UVColor##" {
	//@@@DynamicShaderTitleRepaceEnd

	Properties
	{
		[PerRendererData]_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_ColorStart("Start Color", Color) = (1,1,1,1)
		_ColorEnd("End Color", Color) = (1,1,1,1)
		_lerp("Lerp", Range(0,1)) = 0
	}

		SubShader
	{
		LOD 200

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		Pass
	{
		Lighting Off
		Fog{ Mode Off }
		Offset -1, -1
		//@@@DynamicShaderBlendRepaceStart
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		//@@@DynamicShaderBlendRepaceEnd
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag			
#include "UnityCG.cginc"

			sampler2D _MainTex;
		fixed4 _ColorStart;
		fixed4 _ColorEnd;
		float   _lerp;

	struct appdata_t
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	v2f o;

	v2f vert(appdata_t v)
	{
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord;
		o.color = v.color;
		return o;
	}

	float _Rotation;
	fixed4 frag(v2f IN) : SV_Target
	{
		float lp = saturate((1 - IN.texcoord.y)*_lerp * 10);
	fixed4 lerpCol = lerp(_ColorStart,_ColorEnd,lp);
	fixed4	 col = tex2D(_MainTex, IN.texcoord) * IN.color * lerpCol;

	return  col;
	}
		ENDCG
	}
	}

		SubShader
	{
		LOD 100

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		Pass
	{
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Offset -1, -1
		//ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMaterial AmbientAndDiffuse

		SetTexture[_MainTex]
	{
		Combine Texture * Primary
	}
	}
	}
}
