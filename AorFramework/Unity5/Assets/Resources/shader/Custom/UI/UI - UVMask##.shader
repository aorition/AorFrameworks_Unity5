// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//按UV Y方向做半透明过度 可反向
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/UI/Image-UVMask##" {
//@@@DynamicShaderTitleRepaceEnd

Properties {
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

}

	SubShader {



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
      	//@@@DynamicShaderBlendRepaceStart
	      Cull Off
	     ZWrite Off
		 ZTest [unity_GUIZTestMode]
		 Blend SrcAlpha OneMinusSrcAlpha
		 //@@@DynamicShaderBlendRepaceEnd
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
             OUT.vertex = UnityObjectToClipPos(IN.vertex);
             OUT.texcoord = IN.texcoord;
             OUT.color = IN.color ;
             #ifdef PIXELSNAP_ON
             OUT.vertex = UnityPixelSnap (OUT.vertex);
             #endif

             return OUT;
         }
		sampler2D _MainTex;
         float4 _color;
		  float _mask;
		  float _reverse;
		  float _range;
         fixed4 frag(v2f IN) : COLOR
         {
			 _mask = _mask * (1.0 +_range) - _range;  		
                float4 col= tex2D(_MainTex,IN.texcoord)*IN.color;
                col.a=min(col.a, saturate((IN.texcoord.y * (1.0 - _reverse) + (1.0- IN.texcoord.y) * _reverse - _mask)/_range));
   
                   clip(col.a);
                   return col;
                return  float4(_reverse,_reverse,_reverse,1);
         }
     ENDCG
        }
        

}
}