// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/NoLight/Unlit - Water - CubeMap - alpha"{

Properties{

	_MainTex("Base(RGB)",2D)="white"{}
	_noiseTex("Noise(RGB)",2D)="white"{}
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	_speed("water speed",range(0.01,2))=1
	_Scale("wave Scale",range(0.01,1))=1
	_light("waterLight", float)=1
	_offset("Offset", Vector)=(0, 0, 0, 0) 
 }
// 	PC
	SubShader {
	 	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }



	
Pass {
	Tags {  "LightMode" = "ForwardBase" }
		//	LOD 100
   		Blend SrcAlpha OneMinusSrcAlpha
		//Lighting Off 
		ZWrite Off
		//ZTest LEqual
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"
			#pragma multi_compile FOG_OFF FOG_ON
			#pragma multi_compile ___ LIGHTMAP_ON
 
    		
                     		     
	sampler2D _noiseTex;
	half4 _noiseTex_ST;
                    		  
	samplerCUBE _Cube;

    half  _speed; 
    half  _Scale;              
    half  _light; 
 float4 _offset;
        
           
            struct v2f {
                half4  pos : SV_POSITION;
                half2  uv : TEXCOORD0;
                half3 viewDir:TEXCOORD1;
               	half3 ref : TEXCOORD2;
				half3 worldNormal:TEXCOORD3;
				half3 worldPos:TEXCOORD4;

				#ifdef LIGHTMAP_ON
				 half2  lightMapUV : TEXCOORD5;
				#endif
  
				#ifdef FOG_ON
				float4 viewpos:TEXCOORD6;		
 				#endif
			
            };
 
           
	struct appdata {
		half4 vertex : POSITION;
		half2 texcoord:TEXCOORD0;
		half3 normal:NORMAL;
		half2 lightmapUV : TEXCOORD1;
	};

            
            //顶点函数没什么特别的，和常规一样
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.worldPos = mul( unity_ObjectToWorld, v.vertex );
              o.viewDir = normalize( _WorldSpaceCameraPos.xyz -o.worldPos.xyz ).xyz;

		  		o.worldNormal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
		  		o.ref= reflect( -o.viewDir, o.worldNormal );
		  		
		  		#ifdef LIGHTMAP_ON
            	o.lightMapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
           	 #endif
            
			#ifdef FOG_ON		
					 o.viewpos=mul(UNITY_MATRIX_MV, v.vertex);
 				#endif
                return o;
            }
									
			fixed4	 frag(v2f i) : COLOR {
 
			//用图做UV采样，精度必须float
			half2 noiseUV=i.uv;
			float detal=_Time *_speed;
			noiseUV+=float2(detal,detal*0.76);
	
    		float4 noiseCol = tex2D(_noiseTex,noiseUV);
    			
    			
    		noiseUV=i.uv;
			detal=_Time*4 *_speed;
    		noiseUV-= half2(detal*0.95,detal*0.57);
    		half4 noiseCol2 = tex2D(_noiseTex,noiseUV);
 
 
				noiseCol=noiseCol*0.4*_Scale;
				noiseCol2=noiseCol2*0.4*_Scale;
			 
				 
				half2 mainUV=i.uv;
			 	mainUV+=noiseCol+noiseCol2;
				
	 
				mainUV+=_Time*0.06;
					
			  	fixed4 mainCol = tex2D(_MainTex,mainUV);	
             
	

				fixed4 reflcol = texCUBE( _Cube, i.ref+mainCol*0.3+(noiseCol+noiseCol2)*0.4);
	 			reflcol=reflcol*reflcol*reflcol;
				mainCol.a=0;

            
 			 	mainCol= mainCol+reflcol*clamp((1-dot(i.worldNormal , i.viewDir)),0.05 ,0.5);
 			 
			 
 		 		#ifdef LIGHTMAP_ON
				float2 lightMapUV=i.lightMapUV;
				lightMapUV+= (noiseCol+noiseCol2)*0.2+float2(_offset.x,_offset.y);
 
						fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,lightMapUV));
						 mainCol.rgb*=lm;
 
			 		#endif


				#ifdef FOG_ON
 				float fogFactor=max(length(i.viewpos.xyz) + _fogDestance, 0.0);
				float fogFactor2= -(i.worldPos.y + _volumeFogOffset) *_volumeFogDestiy;

				fogFactor = max(fogFactor, fogFactor2);

				mainCol.a =  exp2(- fogFactor /_fogDestiy)*_light;
				#else
				mainCol.a=1;
				#endif
		 
 
			return mainCol;
			//	return mainCol*_light;
            }
			ENDCG
		}
	
	}

	
}



