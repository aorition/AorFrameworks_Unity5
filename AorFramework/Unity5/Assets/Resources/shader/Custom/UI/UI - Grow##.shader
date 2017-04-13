//@@@DynamicShaderInfoStart
//带更多选项的自发光贴图材质
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/UI/Image-Grow##" {
//@@@DynamicShaderTitleRepaceEnd

Properties {
 	[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
	
	_TintStrength ("Tint Color Strength", Range(0, 5)) = 1
	_CoreStrength ("Core Color Strength", Range(0, 8)) = 1
	_CutOutLightCore ("CutOut Light Core", Range(0, 1)) = 0.5

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

         fixed4 frag(v2f IN) : COLOR
         {
			fixed4 tex = tex2D(_MainTex, IN.texcoord);
			fixed4 col = tex.g * _TintStrength + tex.r * IN.color * _CoreStrength  - _CutOutLightCore; 
			col.a = tex.a;
			return saturate(col);
         }
     ENDCG
        }
        

}
}