// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//@@@DynamicShaderInfoStart
//水晶球shader
//@@@DynamicShaderInfoEnd


Shader "Custom/longzhu/CrystalBall2" {
	Properties{


				[HideInInspector] _MainTex("Base(RGB) Gloss(A)", 2D) = "white" {}
				_Specular("Specular", Range(0, 10)) = 1
				_Shininess("Shininess", Range(0.01, 1)) = 0.5

				_Color("Main Color", Color) = (1,1,1,1)

				_RimColor("Rim Color", Color) = (1,1,1,1)
				_RimPower("Rim Power", float) = 1
				_RimWidth("Rim Width", range(0.001, 5)) = 1

				_RimColor2("Rim Color 2", Color) = (1,1,1,1)
				_RimPower2("Rim Power 2", float) = 1
				_RimWidth2("Rim Width 2", range(0.001, 5)) = 1

				[HideInInspector] _Lighting("Lighting", float) = 1
				[HideInInspector] _CutOut("CutOut", float) = 0.1

	}

		SubShader{
			Tags {
				"RenderType" = "Transparent"  "Queue" = "Transparent"
			}

			Pass {
				Tags { "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#include "UnityCG.cginc"  
				#include "Lighting.cginc"  
				#pragma vertex vert  
				#pragma fragment frag  
				#pragma fragmentoption ARB_precision_hint_fastest  
				#pragma multi_compile_fwdbase  

				fixed4 _Color;

			float4 _RimColor;
			float _RimPower;
			float _RimWidth;

			float4 _RimColor2;
			float _RimPower2;
			float _RimWidth2;

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed _Specular;
			fixed _Shininess;


			struct v2f {
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 normal : TEXCOORD1;
				fixed3 viewDir : TEXCOORD2;
				fixed3 lightDir : TEXCOORD3;
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.normal = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
				o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
				o.lightDir = normalize(_WorldSpaceLightPos0.xyz);

				return o;

			}

			fixed4 frag(v2f i) : COLOR {

				//fixed4 final = _Color;
					
				//fixed4 c = tex2D(_MainTex, i.uv.xy);
				fixed4 c = _Color;

				#ifdef CLIP_ON
					clip(c.a - _CutOut);
				#endif

				fixed4 o = c;

				//边缘光
				fixed rim = 1 - dot(i.normal, i.viewDir);
				fixed3 rimLight = smoothstep(_RimWidth, 1.0, rim);
				fixed3 rimLight2 = smoothstep(_RimWidth2, 1.0, rim);

				fixed nh = saturate(dot(i.normal, normalize(i.viewDir + i.lightDir)));
				fixed3 spec = _LightColor0.rgb * pow(nh, _Shininess * 128) * _Specular;
				o.rgb = c.rgb + spec * c.a + rimLight.rgb*_RimColor*_RimPower + rimLight2.rgb*_RimColor2*_RimPower2;
				o.a = c.a + (_RimColor.a * _RimColor2.a);
				return o;

				}
			    ENDCG
		    }

	  }


}
