// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//溶解 ,像素丢弃模式，边缘粗，支持写深度
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-Dissolve-AlphaClip##" {
//@@@DynamicShaderTitleRepaceEnd

	Properties
	{
		[PerRendererData]_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_noiseTex("Noise Texture", 2D) = "white" {}
		_lineColor("LineColor", Color) = (1,1,1,1)
		_lineSize("LineSize", Range(0.01,0.1)) = 0.1
		_mask("Mask", Range(0,1)) = 0
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

			float _mask;
			float _lineSize;
			float4 _lineColor;
			sampler2D _MainTex;
			sampler2D _noiseTex;

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

			v2f vert (appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
			
			float _Rotation;
			fixed4 frag (v2f IN) : SV_Target
			{
				float4 noiseCol = tex2D(_noiseTex,IN.texcoord);
				float clipValue = max(noiseCol.r - _mask, -0.1);

				clip(clipValue);
				clipValue = max((_lineSize - clipValue), 0.0) / _lineSize;
				float4 col = tex2D(_MainTex, IN.texcoord)* IN.color + clipValue*_lineColor*_lineColor.a;
				clip(col.a - 0.1);
				return col;
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
			Fog { Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
