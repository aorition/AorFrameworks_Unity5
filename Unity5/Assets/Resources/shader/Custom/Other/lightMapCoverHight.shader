// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hide/lightMapCoverHight" {
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
       fixed3x3 M = fixed3x3(
           0.2209, 0.3390, 0.4184,
           0.1138, 0.6780, 0.7319,
           0.0102, 0.1130, 0.2969 );
       fixed4 vResult;
       fixed3 Xp_Y_XYZp = mul(vRGB, 
M);
       Xp_Y_XYZp = max(Xp_Y_XYZp, 
fixed3(1e-6, 1e-6, 1e-6));
       vResult.xy = Xp_Y_XYZp.xy / 
Xp_Y_XYZp.z;
       fixed Le = 2 * 
log2(Xp_Y_XYZp.y) + 127;
       vResult.w = frac(Le);
       vResult.z = (Le - 
(floor(vResult.w*255.0f)) / 255.0f) / 255.0f;
       return vResult;
		}
 
            float4 frag (v2f i) : COLOR
            {
             fixed3 col=DecodeLightmap(tex2D(_MainTex,i.uv));
			return EncodeLogLuv(col.rgb);
            }


            ENDCG
        }


  }

}
