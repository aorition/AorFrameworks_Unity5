//@@@DynamicShaderInfoStart
//AlphaMask
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/RawImage/RawImage-AlphaMask##" {
//@@@DynamicShaderTitleRepaceEnd

	Properties {
		 [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		 _MaskTex ("Mask Texture", 2D) = "white" {}

		 _Main_Rotation ("Main Rotation Angle", Float) = 0
		 _Mask_Rotation ("Mask Rotation Angle", Float) = 0

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
         #include "UnityCG.cginc"
		
		sampler2D _MainTex;
		sampler2D _MaskTex;

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

		 float4 _MainTex_ST;
		 float4 _MaskTex_ST;

         v2f vert(appdata_t IN)
         {
             v2f OUT;
             OUT.vertex = mul(UNITY_MATRIX_MVP,  IN.vertex);
             OUT.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
			 OUT.maskcoord = TRANSFORM_TEX(IN.texcoord,_MaskTex);
             OUT.color = IN.color ;

             return OUT;
         }

		 float _Main_Rotation;
		 float _Mask_Rotation;

         fixed4 frag(v2f IN) : COLOR
         {
			 float2 uv = IN.texcoord.xy - float2(0.5,0.5);
			 float mag = radians(_Main_Rotation);

			 uv =  float2(    uv.x*cos(mag) - uv.y*sin(mag),
                                      uv.x*sin(mag) + uv.y*cos(mag) );
			 uv += float2(0.5,0.5);

			 float2 maskuv = IN.maskcoord.xy - float2(0.5,0.5);
			 float ag = radians(_Mask_Rotation);

			 maskuv =  float2(    maskuv.x*cos(ag) - maskuv.y*sin(ag),
                                      maskuv.x*sin(ag) + maskuv.y*cos(ag) );
			 maskuv += float2(0.5,0.5);

		     float4 mask =  tex2D(_MaskTex,maskuv);
			 float4 col = tex2D(_MainTex,uv) * IN.color;

			 col.a *= mask.a;
			 return col;
         }
     ENDCG
        }
		}
}