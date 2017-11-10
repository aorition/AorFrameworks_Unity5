// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/NoLight/Unlit - Water - Simple"{

Properties{

	_MainTex("Base(RGB)",2D)="white"{}
	_noiseTex("Noise(RGB)",2D)="white"{}
	_speed("water speed",range(0.01,2))=1
	_Scale("wave Scale",range(0.01,1))=1
	_Light("Light", float)=1
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
 

                     		
                     		     
	uniform sampler2D _noiseTex;
	half4 _noiseTex_ST;

    half  _speed; 
    half  _Scale;              
    half  _Light; 

        
           
            struct v2f {
                half4  pos : SV_POSITION;
                half2  uv : TEXCOORD0;
				#ifdef LIGHTMAP_ON
				 half2  lightMapUV : TEXCOORD4;
				#endif
 
				#ifdef FOG_ON
				float4 viewpos:TEXCOORD5;		
 				#endif
            };
           
           
	struct appdata {
		half4 vertex : POSITION;
		half2 texcoord:TEXCOORD0;
		half2 lightmapUV : TEXCOORD1;
	
	};

            
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
               half4 worldPos = mul( unity_ObjectToWorld, v.vertex );

		  		#ifdef LIGHTMAP_ON
            	o.lightMapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
           		 #endif
            
            	#ifdef FOG_ON		
					 o.viewpos=mul(UNITY_MATRIX_MV, v.vertex);
 				#endif
                return o;
            }
									
			fixed4 frag(v2f i) : COLOR {

			half2 noiseUV=i.uv;
			half detal=_Time *_speed;
			noiseUV+=half2(detal,detal*0.76);
	
    	 	float4 noiseCol = tex2D(_noiseTex,noiseUV);
			 
    		noiseUV=i.uv;
			detal=_Time*4 *_speed;
    		noiseUV-= half2(detal*0.95,detal*0.57);
		 	float4 noiseCol2 = tex2D(_noiseTex,noiseUV);
 
	
   				half2 mainUV=i.uv;

				noiseCol=noiseCol*0.4*_Scale;
				noiseCol2=noiseCol2*0.4*_Scale;
			 	mainUV+=noiseCol+noiseCol2;
				
	 
				mainUV+=_Time*0.1;
				
			  	fixed4 mainCol = tex2D(_MainTex,mainUV);	
   
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
			
				return mainCol*_Light;
            }
			ENDCG
		}
	
	}

	
}



