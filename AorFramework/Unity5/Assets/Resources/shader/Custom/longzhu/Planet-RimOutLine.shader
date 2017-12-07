// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/longzhu/Planet-RimOutline" {
	Properties {
		_MainColor ("MainColor", Color) = (1,1,1,1)
		_SelfLight ("SelfLight", float) = 1
		_MainTex ("Main Texture", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimWidth ("Rim Width", float) = 1

		_OutlineColor("Outline Color", Color) = (1,0,0,1)
		_OutlineWidth("Outline Width", range(0, 0.4)) = 1

		[Toggle] _HasNight("HasNight?", Float) = 0    //夜晚效果开关

	}
	SubShader {
	    Tags { "RenderType"="Opaque" }
	    Blend SrcAlpha OneMinusSrcAlpha


	    Pass {
	        Tags{ "LightMode" = "Always" }
	        Cull Front
			ZWrite On
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _OutlineWidth;
            float4 _OutlineColor;

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;

                v.vertex.xyz += v.normal * _OutlineWidth;
                o.pos = UnityObjectToClipPos (v.vertex);

                return o; 
            }

            fixed4 frag(v2f i): COLOR {
                return _OutlineColor;
            }
            ENDCG

	    }


		Pass {
       		Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma shader_feature _HASNIGHT_ON
                #include "UnityCG.cginc"

                struct appdata 
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f 
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed3 color : COLOR;

                };

                uniform fixed4 _MainColor;
                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                uniform fixed4 _RimColor;
                float _RimWidth;
                float _SelfLight;

                //夜晚系统全局变量
                float _HdrIntensity;
                float3 _DirectionalLightColor;
                float4 _DirectionalLightDir;


                v2f vert (appdata_base v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);

                    float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                    float dotProduct = 1 - dot(v.normal, viewDir);
                   
                    o.color = smoothstep(1 - _RimWidth, 1.0, dotProduct);
                    o.color *= _RimColor;

                    o.uv = v.texcoord.xy;
                    return o;
                }



                fixed4 frag(v2f i) : COLOR {
                    fixed4 c = tex2D(_MainTex, i.uv) * _MainColor * _SelfLight;
                    c.rgb += i.color;

                #ifdef _HASNIGHT_ON
                    c.rgb *= _HdrIntensity+_DirectionalLightColor*_DirectionalLightDir.w+ c.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
                #endif

                    return c;
                }
            ENDCG
        }

	}

}
