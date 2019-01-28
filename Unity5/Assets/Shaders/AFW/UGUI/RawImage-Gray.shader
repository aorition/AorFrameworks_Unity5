Shader "AFW/UGUI/RawImage - Gray"
{
	
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		_Gray("_Gray", Float) = 0

		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10

	}
	
	SubShader
	{
		LOD 100

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType"="Plane" }

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Fog { Mode Off }
		Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Gray;

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
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				#ifdef UNITY_HALF_TEXEL_OFFSET
					o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif
				return o;
			}
				
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				clip (col.a - 0.01);
					
				float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
				col.rgb = lerp(col.rgb, fixed3(grey, grey, grey), _Gray);
					
				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader
