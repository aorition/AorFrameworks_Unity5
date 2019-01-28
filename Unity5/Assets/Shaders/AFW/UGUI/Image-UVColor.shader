//@@@DynamicShaderInfoStart
//<readonly> UGUI 简单的UV双色控制
//@@@DynamicShaderInfoEnd
Shader "AFW/UGUI/Image - UVColor"
{

	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	
		_ColorStart ("Start Color", Color) = (1,1,1,1)
		_ColorEnd ("End Color", Color) = (1,1,1,1)
		_lerp ("Lerp", Range(0,1)) = 0
	
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

	Category {

		 Tags
		 { 
			 "Queue"="Transparent" 
			 "IgnoreProjector"="True" 
			 "RenderType"="Transparent" 
			 "PreviewType"="Plane"
			 "CanUseSpriteAtlas"="True"
		 }

		 Stencil
		 {
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		 }
  	 
		 Lighting Off
		 Fog { Mode Off }
		 Cull Off
		 ZWrite Off
		 ZTest [unity_GUIZTestMode]
		 Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]

		 SubShader
		 {

			Pass
			{
				Name "SPRITE_BASE"

				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile DUMMY PIXELSNAP_ON

				sampler2D _MainTex;
				fixed4 _ColorStart;
				fixed4 _ColorEnd;
				float _lerp;

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

				fixed4 frag(v2f IN) : SV_Target
				{
					float lp= saturate((1-IN.texcoord.y)*_lerp*10);
					fixed4 lerpCol=lerp(_ColorStart,_ColorEnd,lp);
					fixed4	 col= tex2D(_MainTex, IN.texcoord) * IN.color * lerpCol;	
					return  col;
				}
				ENDCG
			}//end pass
		 }//end SubShader
	}//end Category
}//end Shader
