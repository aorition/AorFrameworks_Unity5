// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//简单的UV双色控制
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - UVColor##" {
//@@@DynamicShaderTitleRepaceEnd


Properties {
   _MainTex ("Base (RGB)", 2D) = "white" {}
   _ColorStart ("Start Color", Color) = (1,1,1,1)
   _ColorEnd ("End Color", Color) = (1,1,1,1)
   _lerp ("Lerp", Range(0,1)) = 0

	   [Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
	   [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
	   [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
	   [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
	   [Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
	   [Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
	   [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2

}

SubShader {
 
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Geometry" }
 


    Pass {  


	   Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
	   ZWrite[_ZWrite]
	   ZTest[_ZTest]
	   Cull[_Cull]


        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			  fixed4 _ColorStart;
			  		  fixed4 _ColorEnd;
			  		float   _lerp;
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR
            {
     float lp= clamp((1-i.texcoord.y)*_lerp*10,0,1);
             fixed4 lerpCol=lerp(_ColorStart,_ColorEnd,lp);
                fixed4 col = tex2D(_MainTex, i.texcoord)*lerpCol;

                return col;
            }
        ENDCG
    }
}

}
