//@@@DynamicShaderInfoStart
//<readonly> UGUI UV遮罩
//@@@DynamicShaderInfoEnd
Shader "AFW/UGUI/Image - UVMask" 
{
	
	Properties 
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

		_mask("Mask", Range(0,1))=0
		_range("Range", Range(0.01,1))=0
		[MaterialToggle] _reverse ("MaskReverse", Float) = 0

		[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil ("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector]_ColorMask ("Color Mask", Float) = 15

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
	
		Pass
		{
			
    		Lighting Off
    		Fog { Mode Off }
			Cull Off
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile DUMMY PIXELSNAP_ON

			sampler2D _MainTex;
			float4 _color;
			float _mask;
			float _reverse;
			float _range;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color ;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				_mask = _mask * (1.0 +_range) - _range;  		
				float4 col= tex2D(_MainTex,IN.texcoord)*IN.color;
				col.a=min(col.a, saturate((IN.texcoord.y * (1.0 - _reverse) + (1.0- IN.texcoord.y) * _reverse - _mask)/_range));
				clip(col.a);
				return col;
				//return  float4(_reverse,_reverse,_reverse,1);
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader