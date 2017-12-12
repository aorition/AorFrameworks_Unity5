// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//水晶球shader
//@@@DynamicShaderInfoEnd


Shader "Custom/longzhu/CrystalBall" {
	Properties {

	    _Color("Main Color", Color) = (1,1,1,1)

	    _SpColor("Specular Color", Color) = (1,1,1,1)
	    _SpPower("Specular Power", float) = 1
	    _SpRange("Specular Range", float) = 1

	    _RimColor("Rim Color", Color) = (1,1,1,1)
	    _RimPower("Rim Power", float) = 1
	    _RimWidth("Rim Width", range(1.01, 5)) = 1

	    _MainTex("Main Texture", 2D) = "white" {}
	    _WaveTex("Wave Texture",2D) = "white"{}
	    _NoiseTex("Noise Texture",2D) = "white"{}
	    _Cube("Reflection Cubemap", Cube) = "_Skybox"

	    _RefPower("Reflection Power", float) = 1
		_WaveScale("Wave Scale", float) = 1
		_WaveSpeed("Wave Speed", float) = 1

		[Toggle] _Invert("Invert", Float) = 0

		[HideInInspector] _Lighting("Lighting", float) = 1
		[HideInInspector] _CutOut("CutOut", float) = 0.1

	}
	
	SubShader {
		Tags {
			"RenderType" = "Transparent"  "Queue" = "Transparent"
		}

		Pass {
			Tags { "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
            #pragma shader_feature _INVERT_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	


			float _RefPower;
			samplerCUBE _Cube;

			float _SpPower;
			float _SpRange;
			float4 _SpColor;

			float4 _RimColor;
			float _RimPower;
			float _RimWidth;

			sampler2D _WaveTex;
			sampler2D _NoiseTex;
			float4 _WaveTex_ST;
			float4 _NoiseTexST;

			float _WaveScale;
			float _WaveSpeed;


			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				half3 normal   : TEXCOORD1;
				half3 viewDir   : TEXCOORD2;
				half3 lightColor : TEXCOORD3;

				half4 color : COLOR;
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.normal = normalize( mul(v.normal, (float3x3)unity_WorldToObject));
				half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
				o.lightColor = Shade4PointLights(worldPos, o.normal);
 
				return o;

			}


			float2 clampUV(float2 uv) {
				return uv = float2(uv.x - floor(uv.x), uv.y - floor(uv.y));
			}


			fixed4 frag(v2f i) : COLOR {
				
				float2 noiseUV = i.uv;
				float delta = _Time * _WaveSpeed;
				noiseUV += clampUV(float2(delta, delta*0.76));
				float4 noiseCol = tex2D(_NoiseTex, noiseUV);

				noiseUV = i.uv;
				delta = _Time * 4 * _WaveSpeed;
				noiseUV -= clampUV(float2(delta*0.95, delta*0.57));
				float4 noiseCol2 = tex2D(_NoiseTex, noiseUV);

				float waveUV = i.uv;
				noiseCol *= 0.4 * _WaveScale;
				noiseCol2 *= 0.4 * _WaveScale;
				waveUV += noiseCol + noiseCol2;
				waveUV += _Time * 0.45;
				fixed4 waveCol = tex2D(_WaveTex, clampUV(waveUV));
				
				//主光方向
				half3 lightDir = _DirectionalLightDir.xyz;
					
			    //高光
				half splight = max(pow(saturate(dot(i.normal, normalize(lightDir.xyz - i.viewDir))), _SpRange),0);

				//反射
        #if _INVERT_ON
				half3 ref = reflect(normalize(i.viewDir), i.normal);
        #else
				half3 ref = reflect(normalize(-i.viewDir), i.normal);
        #endif

				//固有色 & 反射
			    fixed4 col = tex2D(_MainTex, i.uv);
				col.a *= _Color.a * i.color.a;
				fixed4 reflcol = texCUBE(_Cube, ref + noiseCol + noiseCol2);

				//环境光
			    fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz  ;

				//边缘光
				fixed rim = 1 - dot(i.normal, i.viewDir);
				fixed3 rimLight = smoothstep(_RimWidth, 1.0, rim);
						
				//最终合成
				half4 final;
				half3 mainlight = col.rgb*(_DirectionalLightColor.rgb + ambientLight);
							 
				final.rgb = mainlight + reflcol.rgb*_RefPower + rimLight.rgb*_RimColor*_RimPower + splight*_SpColor*_SpPower + waveCol;
				final.a = col.a;
 
		#ifdef CLIP_ON
				clip(col.a - _CutOut);
		#endif

				final.rgb *= (_Lighting + _HdrIntensity);

				float isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
				float3 grayCol = dot(final.rgb, float3(0.299, 0.587, 0.114));
				final.rgb = lerp(final.rgb*_Color.rgb, grayCol.rgb, isGray);

				return  final;

				}
			    ENDCG
		    }

	  }


}
