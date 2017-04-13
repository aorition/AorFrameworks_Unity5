//@@@DynamicShaderInfoStart
//溶解 ,像素丢弃模式，边缘粗，支持写深度
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/UI/Image-Dissolve-AlphaClip##" {
//@@@DynamicShaderTitleRepaceEnd

Properties {
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
		
		sampler2D _MainTex;
		float _CutOutLightCore;
		float _TintStrength;
		float _CoreStrength;
		
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
             OUT.vertex = mul(UNITY_MATRIX_MVP,  IN.vertex);
             OUT.uv = IN.texcoord;
             OUT.color = IN.color ;
             #ifdef PIXELSNAP_ON
             OUT.vertex = UnityPixelSnap (OUT.vertex);
             #endif

             return OUT;
         }
			
		float _mask;
		float _lineSize;	
		float4 _lineColor;
		sampler2D _noiseTex;
        float4 frag (v2f i) : COLOR
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
    }
        

}
}