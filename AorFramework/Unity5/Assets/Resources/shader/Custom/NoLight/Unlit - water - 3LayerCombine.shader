// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/NoLight/Unlit - water - 3LayerCombine"{

	Properties{

		_MainTex("underGround(RGB)",2D) = "white"{}
		_MoveTex("Move(RGB)",2D) = "white"{}
		_noiseTex("Noise(RGB)",2D) = "white"{}
		_waterSpeed("water speed",range(0.01,2)) = 1
		_speed("wave speed",range(0.01,2)) = 1
		_Scale("wave Scale",range(0.01,1)) = 1
	}
		SubShader{

			Lighting Off

	Pass {
		Tags { "LightMode" = "ForwardBase" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/ObjectBaseShader.cginc"
				#pragma multi_compile FOG_OFF FOG_ON
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF


			uniform sampler2D _MoveTex;
		half4 _MoveTex_ST;

		uniform sampler2D _noiseTex;
		half4 _noiseTex_ST;

		uniform samplerCUBE _Cube;

		half  _speed;
		half  _waterSpeed;
		half  _Scale;




				struct v2f {
					half4  pos : SV_POSITION;
					half2  uv : TEXCOORD0;
					fixed3 viewDir : TEXCOORD1;
					half2 uv1 : TEXCOORD2;
					half2 uv2:TEXCOORD3;
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
			float3 normal:NORMAL;
			half2 lightmapUV : TEXCOORD1;

		};


		//顶点函数没什么特别的，和常规一样
		v2f vert(appdata v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			o.uv1 = TRANSFORM_TEX(v.texcoord, _MoveTex);
			o.uv2 = TRANSFORM_TEX(v.texcoord, _noiseTex);
		   half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		  o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz).xyz;

		  //	o.worldNormal = mul(SCALED_NORMAL, (float3x3)_World2Object);
			  //o.ref= reflect( -o.viewDir, o.worldNormal );

			  #ifdef LIGHTMAP_ON
			  o.lightMapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			   #endif

			  #ifdef FOG_ON		
				   o.viewpos = mul(UNITY_MATRIX_MV, v.vertex);
			  #endif
			  return o;
		  }

		  fixed4 frag(v2f i) : COLOR {

		  half2 noiseUV = i.uv2;
		  half detal = _Time *_speed;
		  noiseUV += half2(detal,detal*0.76);

		  float4 noiseCol = tex2D(_noiseTex,noiseUV);


		  noiseUV = i.uv;
		  detal = _Time * 4 * _speed;
		  noiseUV -= half2(detal*0.95,detal*0.57);
		  fixed4 noiseCol2 = tex2D(_noiseTex,noiseUV);


			  half2 mainUV = i.uv;
			  half2 moveUV = i.uv1;

			  noiseCol = noiseCol*0.4*_Scale;
			  noiseCol2 = noiseCol2*0.4*_Scale;
			  moveUV += noiseCol + noiseCol2;
			  mainUV += noiseCol + noiseCol2;

			  moveUV += _Time*_waterSpeed;

			  fixed4 mainCol = tex2D(_MainTex,mainUV);
			  fixed4 moveCol = tex2D(_MoveTex, moveUV);
			  mainCol.a = 0;

			  mainCol += moveCol;

			  //mainCol= mainCol+reflcol*CustomClamp(1-(1-dot(i.worldNormal,i.viewDir)),0.05 ,0.5);



			   fixed3 lm = fixed3(0, 0, 0);
		  #ifdef LIGHTMAP_ON
	 
				mainCol.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightMapUV));

 
		  #endif


			  #ifdef FOG_ON
			  float fogFactor = max(length(i.viewpos.xyz) + _fogDestance, 0.0);
			  mainCol.a = exp2(-fogFactor / _fogDestiy);
			  #else
			  mainCol.a = 1;
			  #endif

			  return mainCol;
		  }
		  ENDCG
	  }

		}


}



