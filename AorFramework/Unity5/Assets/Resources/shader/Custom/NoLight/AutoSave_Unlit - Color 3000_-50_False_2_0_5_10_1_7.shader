// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//不写深度的双面的Alpha混合的shader,层:Transparent
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/NoLight/Unlit - Color 3000_-50_False_2_0_5_10_1_7"  {  
//@@@DynamicShaderTitleRepaceEnd

Properties {
 _MainTex ("Texture", 2D) = "white" { }
_TintColor ("Color", Color) = (0.5,0.5,0.5,0.5)
_Lighting ("Lighting",  float) = 1
_CutOut("CutOut", float) = 0.1

}


	SubShader {
        //@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Transparent" } 
//@@@DynamicShaderTagsRepaceEnd
	
	Pass
    {
      //@@@DynamicShaderBlendRepaceStart
Blend SrcAlpha OneMinusSrcAlpha,One DstAlpha
ZTest Less
 ZWrite Off
 Cull Off
//@@@DynamicShaderBlendRepaceEnd

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
       		#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma multi_compile FOG_OFF FOG_ON 

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Lighting;
						
			#ifdef FOG_ON
        	float _fogDestiy;
        	float _fogDestance;
			#endif

			#ifdef CLIP_ON
			fixed _CutOut;
			#endif

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4 color : COLOR;
				#ifdef FOG_ON		
				float3 viewpos: TEXCOORD1;
 				#endif

            }

;
            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord:TEXCOORD0;
                float4 color : COLOR;
            }

;
            //顶点函数没什么特别的，和常规一样 
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.color=v.color;

				#ifdef FOG_ON		
				o.viewpos = mul(UNITY_MATRIX_MV, v.vertex);
				#endif
                return o;
            }


            
             fixed4 _TintColor;
            float4 frag (v2f i) : COLOR
            {
                float4 col= tex2D(_MainTex,i.uv);
                col=col* _TintColor*i.color;
				col.rgb*=_Lighting;


                //先clip，再fog 不然会出错	
 			#ifdef CLIP_ON
			clip(  col.a-_CutOut);
            #endif
			


 			#ifdef FOG_ON
			float fogFactor = max(length(i.viewpos.xyz) + _fogDestance, 0.0);
			col.a = col.a* exp2(-fogFactor / _fogDestiy);
             #endif

			return col;
            }


            ENDCG
        }
        

}
}