//@@@DynamicShaderInfoStart
//<readonly> UGUI 溶解 ,像素丢弃模式，边缘粗，支持写深度
//@@@DynamicShaderInfoEnd
Shader "AFW/UGUI/Image - Dissolve - AlphaClip" 
{

	Properties 
	{
 		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_noiseTex ("Noise Texture", 2D) = "white" {}
		
		_lineColor("LineColor", Color)=(1,1,1,1)
		_lineSize("LineSize", Range(0.01,0.1))=0.1
		_mask("Mask", Range(0,1))=0 

		[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil ("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector]_ColorMask ("Color Mask", Float) = 15

		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
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
			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile DUMMY PIXELSNAP_ON

			sampler2D _MainTex;
			float _CutOutLightCore;
			float _TintStrength;
			float _CoreStrength;
			
			float _mask;
			float _lineSize;	
			float4 _lineColor;
			sampler2D _noiseTex;

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
				half2 uv  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.texcoord;
				OUT.color = IN.color ;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 noiseCol= tex2D(_noiseTex,i.uv);
				float clipValue = max(noiseCol.r-_mask, -0.1);
				
				clip(clipValue);
				clipValue = max((_lineSize - clipValue), 0.0) / _lineSize;
				float4 col= tex2D(_MainTex,i.uv)* i.color + clipValue*_lineColor*_lineColor.a;
				clip(col.a - 0.1);
				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader