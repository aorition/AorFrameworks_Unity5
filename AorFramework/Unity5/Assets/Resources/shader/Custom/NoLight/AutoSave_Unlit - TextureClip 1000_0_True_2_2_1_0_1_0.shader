//@@@DynamicShaderInfoStart
//写深度的单面的Alpha混合的shader,层:Background
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/NoLight/Unlit - TextureClip 1000_0_True_2_2_1_0_1_0"  {  
//@@@DynamicShaderTitleRepaceEnd
Properties {
	_MainTex ("Texture", 2D) = "white" {}
	_noiseTex ("Noise Texture", 2D) = "white" {}
    _color("texColor", Color)=(1,1,1,1)

    _lineColor("LineColor", Color)=(1,1,1,1)
    _lineSize("LineSize", Range(0,0.05))=0
    _mask("Mask", Range(0,1))=0 
          
}
	SubShader {
//@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Background" } 
//@@@DynamicShaderTagsRepaceEnd

 	Lighting Off

  Pass {


  	 //@@@DynamicShaderBlendRepaceStart
Blend One Zero,One Zero
ZTest Less
 ZWrite On
 Cull Back
//@@@DynamicShaderBlendRepaceEnd

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _noiseTex;   
            float4 _MainTex_ST;
           
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            };
           
           
           struct appdata {
    float4 vertex : POSITION;
    float2 texcoord:TEXCOORD0;
};

            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
 
		 float4 _color;
		  float _mask;
		  float _lineSize;	
		  float4 _lineColor;
		  
            float4 frag (v2f i) : COLOR
            {
                    float4 noiseCol= tex2D(_noiseTex,i.uv);
                    float clipValue = max(noiseCol.r-_mask, -0.00001);
                    clip(clipValue);
                    clipValue = max((_lineSize - clipValue), 0.0) / _lineSize;
                   // clipValue = step(0.0, (_lineSize - clipValue));
                    
                float4 col= tex2D(_MainTex,i.uv)* _color + clipValue*_lineColor;
             //   col.a = 1.0;
                return col;
             
            }
            ENDCG
        }
 
}



}