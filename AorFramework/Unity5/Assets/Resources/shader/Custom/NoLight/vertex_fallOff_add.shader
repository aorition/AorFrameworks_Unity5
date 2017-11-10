// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Saint/model/vertex - lerp_add" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _alpha("Alpha", float) = 1
           _light("Light", float) = 1
    }
    

    SubShader
    {
       // AlphaTest Greater .2
       		Blend SrcAlpha One
       		Lighting Off ZWrite Off
        pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
           float _alpha;
           float _light;
            struct v2f {

                float4  pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2  uv : TEXCOORD2;
                 float3 worldvertpos : TEXCOORD1;

            };
           
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata_base v)
            {
                v2f o;
                   o.pos = UnityObjectToClipPos(v.vertex);
     
                   o.normal=v.normal;
              	  o.uv =    TRANSFORM_TEX(v.texcoord,_MainTex);
                    o.worldvertpos = ObjSpaceViewDir  (v.vertex).xyz;
                return o;
            }
 
 
            float4 frag (v2f i) : COLOR
            {
            
                i.normal = normalize(i.normal);
                    float3 viewdir = normalize( i.worldvertpos);
                    
                    float4 texCol = tex2D(_MainTex,i.uv);
                     texCol.a =pow((1- saturate(dot(viewdir, i.normal))),_alpha);
				 

      			       return texCol*_light;
 
            }
            ENDCG
        }
    }
}