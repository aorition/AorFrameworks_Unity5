// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/NoLight/Unlit - Water"{

Properties{
	_underGroundTex("underGroundTex(RGBA)",2D) = "white"{}
    _MaskTex("MaskTex(RGB)",2D) = "white"{}
	_MainTex("Base(RGB)",2D)="white"{}
	_noiseTex("Noise(RGB)",2D)="white"{}
	_speed("water speed",range(0.01,2)) = 1
	//_Light("Light", float) = 1.5
		_Lighting("Lighting", float) = 1

	[Toggle] _Fog("Fog?", Float) = 1
	[Toggle] _HasNight("HasNight?", Float) = 0    //夜晚效果开关
}

SubShader{

	Lighting Off

Pass {
		Name "BASEWATER"
	Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"
#pragma shader_feature _FOG_ON
#pragma multi_compile_fog
			#pragma multi_compile ___ LIGHTMAP_ON
			#pragma shader_feature _HASNIGHT_ON


	uniform sampler2D _noiseTex;
	float4 _noiseTex_ST;
	sampler2D _underGroundTex;
	float4 _underGroundTex_ST;
	sampler2D _MaskTex;
	float4 _MaskTex_ST;

	float  _speed;             
	//float  _Light;

        
           
            struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;

				#ifdef LIGHTMAP_ON
				float2  lightMapUV : TEXCOORD1;
				#endif
 
#if _FOG_ON
				 UNITY_FOG_COORDS(5)
#endif
			    float2  uv1 : TEXCOORD6;
				float2  uv2 : TEXCOORD7;
            };
           
           
	struct appdata {
		float4 vertex : POSITION;
		float2 texcoord:TEXCOORD0;
		float2 lightmapUV : TEXCOORD1;
	
	};

            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _underGroundTex);

		  		#ifdef LIGHTMAP_ON
            	o.lightMapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
           		 #endif
            
#if _FOG_ON
				UNITY_TRANSFER_FOG(o, o.pos);
#endif
                return o;
            }
									
			fixed4 frag(v2f i) : COLOR {

				float2 noiseUV=i.uv;
			    noiseUV -= _Time*_speed*0.1 /7;
				float2 mainUV=i.uv;
				mainUV+=_Time*_speed /7;
				
				float4 noiseCol = tex2D(_noiseTex, noiseUV);
				fixed4 underCol = tex2D(_underGroundTex, i.uv2);
				fixed4 maskCol = tex2D(_MaskTex, i.uv1);

			  	fixed4 mainCol = tex2D(_MainTex,mainUV);
				mainCol.rgb += noiseCol.rgb*0.2;
				mainCol.rgb *= maskCol.rgb*1.2;

				mainCol.rgb = lerp(mainCol.rgb*1.4, underCol.rgb*1.7, maskCol.a)*_Lighting;
				mainCol.a=0;
				//return fixed4(_Light, _Light, _Light,1);

			fixed3 lm = fixed3(0, 0, 0);

			#ifdef LIGHTMAP_ON
 				  mainCol.rgb*= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightMapUV));
			#endif

            #ifdef _HASNIGHT_ON
				  mainCol.rgb *= _HdrIntensity + _DirectionalLightColor*_DirectionalLightDir.w + mainCol.rgb*UNITY_LIGHTMODEL_AMBIENT.xyz;    //夜晚效果
            #endif


#if _FOG_ON
				  UNITY_APPLY_FOG(i.fogCoord, mainCol);
#endif
			
				return mainCol;
            }
			ENDCG
		}
	
	}

	
}



