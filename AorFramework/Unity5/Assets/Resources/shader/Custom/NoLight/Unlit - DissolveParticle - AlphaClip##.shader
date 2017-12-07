// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//溶解 ,像素丢弃模式，边缘粗，支持写深度
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - DissolveParticle - AlphaClip##" {
//@@@DynamicShaderTitleRepaceEnd
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_noiseTex("Noise Texture", 2D) = "black" {}
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_mask("Mask", Range(0,1)) = 0
		_Light("Light", float) = 1
	[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
	[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
	[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
	[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
	[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
	[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
	[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
	}
	SubShader {

			Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Geometry" }


  Pass {

 

		Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
		ZWrite[_ZWrite]
		ZTest[_ZTest]
		Cull[_Cull]

		Lighting Off
		ColorMask RGB

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
                 float4  Color : Color;
            };
           
           
           struct appdata {
    float4 vertex : POSITION;
    float2 texcoord:TEXCOORD0;
      float4 Color:Color;
};

            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.Color=v.Color;
                return o;
            }
 
		 float4 _TintColor;
		  float _mask;
		  float _Light;
		  
            float4 frag (v2f i) : COLOR
            {
                    float4 noiseCol= tex2D(_noiseTex,i.uv);
                    float4 col= tex2D(_MainTex,i.uv);
                    col.a=min( col.a,noiseCol.r)  ;
                    col*=_TintColor*i.Color;
                   

				 	clip(col.a- _mask);
					col.rgb *= _Light;
                return col;
             
            }
            ENDCG
        }
 
}



}