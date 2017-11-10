// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/NoLight/Unlit - Water - CubeMap"{

Properties{
	_underGroundTex("underGroundTex(RGBA)",2D) = "white"{}
	_landLight("landLight", float) = 1
	_MaskTex("MaskTex(RGB)",2D) = "white"{}
	_MainTex("Base(RGB)",2D)="white"{}
	_noiseTex("Noise(RGB)",2D)="white"{}
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	_speed("water speed",range(0.01,2))=1
	_Scale("wave Scale",range(0.01,1))=1
	_Color("Color", Color) = (1,1,1,1)
	_Lighting("Lighting",  float) = 1
 }
	SubShader {

		Lighting Off 
	
Pass {
	Tags { "LightMode" = "ForwardBase" }
	
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"
			#pragma multi_compile FOG_OFF FOG_ON
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
 

                     		
	sampler2D _underGroundTex;
	  sampler2D _noiseTex;
	  sampler2D _MaskTex;

	float4 _noiseTex_ST;
	float4 _MaskTex_ST;
	float4 _underGroundTex_ST;
	uniform samplerCUBE _Cube;

    half  _speed; 
    half  _Scale;              
	half  _landLight;
        
           
            struct v2f {
                half4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			
				float3 viewDir:TEXCOORD1;
               	float3 normal : TEXCOORD2;
			//	float3 worldNormal:TEXCOORD3;
				#ifdef LIGHTMAP_ON
				 half2  lightMapUV : TEXCOORD4;
				#endif
 
				#ifdef FOG_ON
				float4 viewpos:TEXCOORD5;		
 				#endif
				float2  uv1 : TEXCOORD6;
				float2  uv2 : TEXCOORD7;
            };
           
           
	struct appdata {
		half4 vertex : POSITION;
		float2 texcoord:TEXCOORD0;
		half4 normal:NORMAL;
		float2 lightmapUV : TEXCOORD1;
	
	};

            
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _underGroundTex);
               half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
			   o.viewDir = (worldPos.xyz - _WorldSpaceCameraPos.xyz).xyz;

		  	//	o.worldNormal = mul(SCALED_NORMAL, (float3x3)_World2Object);
				o.normal= normalize(mul(v.normal.rgb, (float3x3)unity_WorldToObject));
		  		
		  		#ifdef LIGHTMAP_ON
            	o.lightMapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
           		 #endif
            
            	#ifdef FOG_ON		
					 o.viewpos=mul(UNITY_MATRIX_MV, v.vertex);
 				#endif
                return o;
            }
			
			float2 clampUV(float2 uv) {
			
				return 	uv = float2(uv.x - floor(uv.x), uv.y - floor(uv.y));
			}

			fixed4 frag(v2f i) : COLOR {

			float2 noiseUV=i.uv;
			float t = _Time;
			 
			//return fixed4(t, t, t, 1);
			float detal=t *_speed;
			noiseUV+= clampUV(float2(detal,detal*0.76));
	
    	 	float4 noiseCol = tex2D(_noiseTex,noiseUV);
		//	fixed4 noiseCol = fixed4(1, 1, 1, 1);
    			
    		noiseUV=i.uv;
			detal=t*4 *_speed;
    		noiseUV-= clampUV(float2(detal*0.95,detal*0.57));
    		 fixed4 noiseCol2 = tex2D(_noiseTex,noiseUV);
			//fixed4 noiseCol2 = fixed4(1, 1, 1, 1);
	
				float2 mainUV=i.uv;

				noiseCol=noiseCol*0.4*_Scale;
				noiseCol2=noiseCol2*0.4*_Scale;
			 	mainUV+=noiseCol+noiseCol2;
				
	 
				mainUV+=t*0.1;
				
			  	fixed4 mainCol = tex2D(_MainTex, clampUV(mainUV));
				fixed4 underCol = tex2D(_underGroundTex, i.uv2);
				fixed4 maskCol = tex2D(_MaskTex, i.uv1);

				half3 ref=reflect(normalize(i.viewDir), i.normal);
				 
				  fixed4 reflcol = texCUBE( _Cube, ref   + noiseCol + noiseCol2 );
				 
				mainCol.a=0;
 

				 fixed3 lm = fixed3(0, 0, 0);
			#ifdef LIGHTMAP_ON
		 
				  mainCol.rgb*= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightMapUV));

 
			#endif


				#ifdef FOG_ON
 				float fogFactor=max(length(i.viewpos.xyz) + _fogDestance, 0.0);
				mainCol.a = exp2(- fogFactor /_fogDestiy);
				#else
				mainCol.a=1;
				#endif


				float c =1- abs(dot(i.normal, i.viewDir));
				c = pow(c, 8);
 
				 mainCol.rgb=mainCol.rgb+reflcol*0.3f;
				 mainCol.rgb *= maskCol.rgb*_Lighting;
				 mainCol.rgb *= (1 + _HdrIntensity);
				mainCol.rgb = lerp(mainCol.rgb, underCol.rgb*(_landLight+_HdrIntensity),  maskCol.a);
				return mainCol;
            }
			ENDCG
		}
	
	}

	
}



