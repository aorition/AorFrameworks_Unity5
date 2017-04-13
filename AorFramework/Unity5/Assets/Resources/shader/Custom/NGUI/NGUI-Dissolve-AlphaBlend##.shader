//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//溶解 ,alpha混合模式
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-Dissolve-AlphaBlend##" {
//@@@DynamicShaderTitleRepaceEnd

	Properties
	{
		[PerRendererData]_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_noiseTex("Noise Texture", 2D) = "white" {}
		_mask("Mask", Range(-1,1)) = 0
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
			sampler2D _noiseTex;
			sampler2D _MainTex;

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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
			
			float _Rotation;
			fixed4 frag (v2f IN) : SV_Target
			{
				float4 noiseCol = tex2D(_noiseTex,IN.texcoord);
				float4 col = tex2D(_MainTex,IN.texcoord)*IN.color;

				noiseCol += _mask;
				col.a = min(saturate(noiseCol), col.a);
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
