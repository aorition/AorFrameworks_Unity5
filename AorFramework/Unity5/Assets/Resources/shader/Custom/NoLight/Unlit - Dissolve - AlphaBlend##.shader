//@@@DynamicShaderInfoStart
//溶解 ,alpha混合模式
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - Dissolve - AlphaBlend##" {
//@@@DynamicShaderTitleRepaceEnd
Properties {
	_MainTex ("Texture", 2D) = "white" {}
	_noiseTex ("Noise Texture", 2D) = "white" {}
	_TintColor("TintColor", Color)=(1,1,1,1)
    _mask("Mask", Range(0,5))=0 
          
}
	SubShader {

	 //@@@DynamicShaderTagsRepaceStart
	Tags {
	"Queue"="Transparent"
	 "IgnoreProjector"="True" 
	 "RenderType"="Geometry"
	 }
 //@@@DynamicShaderTagsRepaceEnd


  Pass
        {
		  

			//@@@DynamicShaderBlendRepaceStart
			Blend SrcAlpha OneMinusSrcAlpha
			Zwrite Off
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
 
		 float4 _TintColor;
		  float _mask;
		  
            float4 frag (v2f i) : COLOR
            {
                    float4 noiseCol= tex2D(_noiseTex,i.uv);
					float4 col= tex2D(_MainTex,i.uv)*_TintColor;

					_mask=saturate( noiseCol.r-_mask+1);
				 	//return _mask;
					col.a=min(saturate(_mask), col.a);
					return col;
             
            }
            ENDCG
        }
 
}



}