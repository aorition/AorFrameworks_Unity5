//@@@DynamicShaderInfoStart
//<readonly> UGUI /按UV Y方向做半透明过度 可反向
//@@@DynamicShaderInfoEnd
Shader "AFW/UGUI/Image - AlphaMask" 
{
	
	Properties 
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_Rotation ("Rotation Angle", Float) = 0

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
			 float4 _MaskTex_ST;
			 sampler2D _MaskTex;

			 float _Rotation;

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
				 half2 maskcoord : TEXCOORD1;
			 };

			 v2f vert(appdata_t IN)
			 {
				 v2f OUT;
				 OUT.vertex = UnityObjectToClipPos(IN.vertex);
				 OUT.texcoord = IN.texcoord;
				 OUT.maskcoord = TRANSFORM_TEX(IN.texcoord,_MaskTex);
				 OUT.color = IN.color ;
				 #ifdef PIXELSNAP_ON
				 OUT.vertex = UnityPixelSnap (OUT.vertex);
				 #endif

				 return OUT;
			 }

			 fixed4 frag(v2f IN) : SV_Target
			 {
				 float2 maskuv = IN.maskcoord.xy - float2(0.5,0.5);
				 float ag = radians(_Rotation);

				 maskuv =  float2(    maskuv.x*cos(ag) - maskuv.y*sin(ag),
										  maskuv.x*sin(ag) + maskuv.y*cos(ag) );
				 maskuv += float2(0.5,0.5);

				 float4 mask =  tex2D(_MaskTex,maskuv);
			 
				 float4 col = tex2D(_MainTex,IN.texcoord) * IN.color;
				 col.a *= mask.a;
				 return col;
			 }
			 ENDCG
		}//end pass
	}//end SubShader
}//end Shader