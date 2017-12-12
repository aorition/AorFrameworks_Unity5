// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//注意：Shader须配合UITextture使用
//@@@DynamicShaderInfoStart
//带更多选项的自发光贴图材质
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NGUI/NGUI-Grow##" {
//@@@DynamicShaderTitleRepaceEnd

	Properties
	{
		[PerRendererData]_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_TintStrength("Tint Color Strength", Range(0, 5)) = 1
		_CoreStrength("Core Color Strength", Range(0, 8)) = 1
		_CutOutLightCore("CutOut Light Core", Range(0, 1)) = 0.5
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
			float _CutOutLightCore;
			float _TintStrength;
			float _CoreStrength;

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
				fixed4 tex = tex2D(_MainTex, IN.texcoord);
				fixed4 col = tex.g * _TintStrength + tex.r * IN.color * _CoreStrength - _CutOutLightCore;
				col.a = tex.a;
				return saturate(col);
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
