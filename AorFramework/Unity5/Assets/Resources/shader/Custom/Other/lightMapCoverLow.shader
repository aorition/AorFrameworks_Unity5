// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hide/lightMapCoverLow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass
    {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            sampler2D _MainTex;
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
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

		fixed4 EncodeLogLuv(fixed3 vRGB)
		{

		float3 vResult = pow(vRGB / 2, 0.5);
		  return fixed4(vResult,1);
		}
 
            float4 frag (v2f i) : COLOR
            {
             fixed3 col=DecodeLightmap( tex2D(_MainTex,i.uv));
			return EncodeLogLuv(col.rgb/2 );
            }


            ENDCG
        }


  }

}
