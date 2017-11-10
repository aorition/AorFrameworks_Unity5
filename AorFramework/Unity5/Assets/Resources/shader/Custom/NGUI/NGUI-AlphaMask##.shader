// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//按UV Y方向做半透明过度 可反向
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-AlphaMask##" {
//@@@DynamicShaderTitleRepaceEnd

	Properties
	{
		[PerRendererData]_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_Rotation("Rotation Angle", Float) = 0
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
			sampler2D _MaskTex;
			//float4 _MainTex_ST;
			float4 _MaskTex_ST;

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
				half2 maskcoord : TEXCOORD1;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.maskcoord = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.color = v.color;
				return o;
			}
			
			float _Rotation;
			fixed4 frag (v2f IN) : SV_Target
			{
				float2 maskuv = IN.maskcoord.xy - float2(0.5,0.5);
				float ag = radians(_Rotation);

				maskuv = float2(maskuv.x*cos(ag) - maskuv.y*sin(ag),
					maskuv.x*sin(ag) + maskuv.y*cos(ag));
				maskuv += float2(0.5,0.5);

				float4 mask = tex2D(_MaskTex,maskuv);

				float4 col = tex2D(_MainTex,IN.texcoord) * IN.color;
				col.a *= mask.a;
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
