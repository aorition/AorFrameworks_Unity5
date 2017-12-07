// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Hidden/buou_dan" {
    Properties {
        _node_8531 ("node_8531", 2D) = "white" {}
        _byg_color ("byg_color", Color) = (0.5,0.5,0.5,1)
        _byg_range ("byg_range", Range(0, 20)) = 20
        _tex_color ("tex_color", Color) = (0.5,0.5,0.5,1)
        _node_2622 ("node_2622", Range(0, 1)) = 0

        [Toggle] _HasNight("HasNight?", Float) = 0    //夜晚效果开关
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
 
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _HASNIGHT_ON
  
            #include "UnityCG.cginc"

 
            uniform sampler2D _node_8531; uniform float4 _node_8531_ST;
            uniform float4 _byg_color;
            uniform float _byg_range;
            uniform float4 _tex_color;
            uniform float _node_2622;

            //夜晚系统全局变量
            float _HdrIntensity;
            float3 _DirectionalLightColor;
            float4 _DirectionalLightDir;


            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
       
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
 
                float4 _node_8531_var = tex2D(_node_8531,TRANSFORM_TEX(i.uv0, _node_8531));
                float node_9760 = pow(1.0-max(0,dot(normalDirection, viewDirection)),(_byg_range*_node_2622));
                float3 emissive = ((_node_8531_var.rgb+_tex_color.rgb)+(node_9760*_byg_color.rgb));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);

                #ifdef _HASNIGHT_ON
                    finalRGBA.rgb *= _HdrIntensity+_DirectionalLightColor*_DirectionalLightDir.w+ finalRGBA.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
                #endif
             
                return finalRGBA;
            }
            ENDCG
        }
    }
 
}
