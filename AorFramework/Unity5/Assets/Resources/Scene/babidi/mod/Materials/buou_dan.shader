// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33477,y:32727,varname:node_4013,prsc:2|emission-9702-OUT;n:type:ShaderForge.SFN_Fresnel,id:9760,x:32577,y:32843,varname:node_9760,prsc:2|EXP-2094-OUT;n:type:ShaderForge.SFN_Tex2d,id:8531,x:32686,y:32501,ptovrint:False,ptlb:node_8531,ptin:_node_8531,varname:node_8531,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:8320,x:32540,y:33115,ptovrint:False,ptlb:byg_color,ptin:_byg_color,varname:node_8320,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:9702,x:32896,y:32814,varname:node_9702,prsc:2|A-5461-OUT,B-1937-OUT;n:type:ShaderForge.SFN_Multiply,id:2094,x:32367,y:32833,varname:node_2094,prsc:2|A-1169-OUT,B-2622-OUT;n:type:ShaderForge.SFN_Slider,id:1169,x:31994,y:32832,ptovrint:False,ptlb:byg_range,ptin:_byg_range,varname:node_1169,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:20,max:20;n:type:ShaderForge.SFN_Multiply,id:1937,x:32747,y:32978,varname:node_1937,prsc:2|A-9760-OUT,B-8320-RGB;n:type:ShaderForge.SFN_Color,id:435,x:32686,y:32704,ptovrint:False,ptlb:tex_color,ptin:_tex_color,varname:node_435,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:5461,x:32875,y:32565,varname:node_5461,prsc:2|A-8531-RGB,B-435-RGB;n:type:ShaderForge.SFN_Slider,id:2622,x:31970,y:33053,ptovrint:False,ptlb:node_2622,ptin:_node_2622,varname:node_2622,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:8531-8320-1169-435-2622;pass:END;sub:END;*/

Shader "Shader Forge/buou_dan" {
    Properties {
        _node_8531 ("node_8531", 2D) = "white" {}
        _byg_color ("byg_color", Color) = (0.5,0.5,0.5,1)
        _byg_range ("byg_range", Range(0, 20)) = 20
        _tex_color ("tex_color", Color) = (0.5,0.5,0.5,1)
        _node_2622 ("node_2622", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _node_8531; uniform float4 _node_8531_ST;
            uniform float4 _byg_color;
            uniform float _byg_range;
            uniform float4 _tex_color;
            uniform float _node_2622;
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
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _node_8531_var = tex2D(_node_8531,TRANSFORM_TEX(i.uv0, _node_8531));
                float node_9760 = pow(1.0-max(0,dot(normalDirection, viewDirection)),(_byg_range*_node_2622));
                float3 emissive = ((_node_8531_var.rgb+_tex_color.rgb)+(node_9760*_byg_color.rgb));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
