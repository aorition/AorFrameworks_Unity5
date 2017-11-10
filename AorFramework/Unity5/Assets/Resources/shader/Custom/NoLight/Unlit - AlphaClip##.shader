// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//AlphaClip 通过设置Clip图（灰度）确定被裁减的区域
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - AlphaClip##" {
	//@@@DynamicShaderTitleRepaceEnd

	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_ClipTex("Clip Texture", 2D) = "white" {}

		_Main_Rotation("Main Rotation Angle", Float) = 0
		_Main_Scale("Main Scale",Range(0.001,10)) = 1
		_Clip_Rotation("Mask Rotation Angle", Float) = 0
		_Clip_Scale("Main Scale", Range(0.001, 10)) = 1

		[MaterialToggle] CLIPSCALE2C("CLIPSCALE2C", Float) = 0

	}

		SubShader{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
	{
		Lighting Off
		Fog{ Mode Off }
			//@@@DynamicShaderBlendRepaceStart
			Cull Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			//@@@DynamicShaderBlendRepaceEnd
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY CLIPSCALE2C_ON
		#include "UnityCG.cginc"

		sampler2D _MainTex;
	sampler2D _ClipTex;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		half2 texcoord  : TEXCOORD0;
		half2 clipcoord : TEXCOORD1;
	};

	float4 _MainTex_ST;
	float4 _ClipTex_ST;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
		OUT.clipcoord = TRANSFORM_TEX(IN.texcoord,_ClipTex);
		OUT.color = IN.color;

		return OUT;
	}

	float _Main_Rotation;
	float _Main_Scale;
	float _Clip_Rotation;
	float _Clip_Scale;

	fixed4 frag(v2f IN) : COLOR
	{
		
		float2 uv = IN.texcoord.xy - float2(0.5,0.5);
		float mag = radians(_Main_Rotation);

		uv = float2(uv.x/_Main_Scale*cos(mag) - uv.y/_Main_Scale*sin(mag),
			uv.x/_Main_Scale*sin(mag) + uv.y/_Main_Scale*cos(mag));
		uv += float2(0.5,0.5);

		float2 cuv = IN.clipcoord.xy - float2(0.5,0.5);
		float ag = radians(_Clip_Rotation);

		float cs = _Clip_Scale;

		#ifdef CLIPSCALE2C_ON
		cs /= max(IN.color.a, 0.001f);
		#endif

		cuv = float2(cuv.x/ cs*cos(ag) - cuv.y/ cs*sin(ag),
			cuv.x/ cs*sin(ag) + cuv.y/ cs*cos(ag));
		cuv += float2(0.5,0.5);

		float4 clip = tex2D(_ClipTex,cuv);
		float4 col = tex2D(_MainTex,uv) * IN.color;

		col.a *= (1 - clip.a);
		return col;
	}
		ENDCG
	}
	}
}