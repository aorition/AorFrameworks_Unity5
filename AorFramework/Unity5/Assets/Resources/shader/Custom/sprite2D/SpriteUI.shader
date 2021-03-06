﻿Shader "Custom/Sprites/SpriteUI"
{
 Properties
 {
     [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
     [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

	 _StencilComp ("Stencil Comparison", Float) = 8
	_Stencil ("Stencil ID", Float) = 0
	_StencilOp ("Stencil Operation", Float) = 0
	_StencilWriteMask ("Stencil Write Mask", Float) = 255
	_StencilReadMask ("Stencil Read Mask", Float) = 255
	_ColorMask ("Color Mask", Float) = 15

	 _Gray("_Gray", Float) = 0
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

  	 Cull Off
     Lighting Off
     ZWrite Off
     Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend SrcAlpha OneMinusSrcAlpha 

 SubShader
 {

  Pass
     {
     Name "SPRITE_BASE"
     CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #pragma multi_compile DUMMY PIXELSNAP_ON
         #include "UnityCG.cginc"


     
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
             OUT.vertex = mul(UNITY_MATRIX_MVP,  IN.vertex);
             OUT.texcoord = IN.texcoord;
             OUT.color = IN.color ;
             #ifdef PIXELSNAP_ON
             OUT.vertex = UnityPixelSnap (OUT.vertex);
             #endif

             return OUT;
         }

         sampler2D _MainTex;
		 float _Gray;


         fixed4 frag(v2f IN) : COLOR
         {
			 fixed4	 col= tex2D(_MainTex, IN.texcoord) * IN.color;	
			 float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
			 col.rgb = lerp(col.rgb, fixed3(grey, grey, grey), _Gray);
			return  col;
         }
     ENDCG
     }
 
 }
 }
}
