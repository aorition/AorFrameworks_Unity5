// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//支持两个点灯
Shader "Custom/Light/Diffuse - 4CombieTexture" {
Properties {
    _Splat0 ("Layer1 (RGB)", 2D) = "white" {}
	_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
	_Splat2 ("Layer3 (RGB)", 2D) = "white" {}
	_Splat3("Layer3 (RGB)", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	_Lighting ("Lighting",  float) = 1
}
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True"  "RenderType"="Geometry"}


Pass {
  Tags {
       "LightMode" = "Vertex" }
	Lighting Off
	SetTexture [_Splat0] {
     combine texture } 
	}

    Pass {
	  
	   Tags {  "LightMode" = "ForwardBase" } 
 
		CGPROGRAM
		//Upgrade NOTE: excluded shader from DX11 and Xbox360 because it uses wrong array syntax (type[size] name)
		#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile ___ LIGHTMAP_ON
		#pragma multi_compile FOG_OFF FOG_ON  
	//	#pragma multi_compile LIGHTMAP_LOGLUV LIGHTMAP_SCALE
		//#pragma multi_compile_fwdbase  
		#include "Assets/ObjectBaseShader.cginc"


		sampler2D _Splat0 ;
		sampler2D _Splat1 ;
		sampler2D _Splat2 ;
		sampler2D _Splat3;
		sampler2D _Control;

		struct v2f {
			half4  pos : SV_POSITION;
			float2 uv[6]:TEXCOORD0;
			fixed3 lightColor:color;  


				#ifdef FOG_ON		
					fixed4 normal : TEXCOORD7;
				#else
					fixed3 normal : TEXCOORD7;
				#endif
		};

		half4 _Splat0_ST;
		half4 _Splat1_ST;
		half4 _Splat2_ST;
		half4 _Splat3_ST;
		half4 _Control_ST;


		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv[0] = TRANSFORM_TEX (v.texcoord, _Splat0);
			o.uv[1] = TRANSFORM_TEX (v.texcoord, _Splat1);
			o.uv[2] = TRANSFORM_TEX (v.texcoord, _Splat2);
			o.uv[3] = TRANSFORM_TEX (v.texcoord, _Splat3);
			o.uv[4] = TRANSFORM_TEX(v.texcoord, _Control);

			o.uv[5] = float2(0, 0);
			#ifdef LIGHTMAP_ON
            	o.uv[5] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
            
            o.normal.xyz = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
			#ifdef FOG_ON	
				o.normal.w = 1;
			#endif
 			half4 worldPos = mul( unity_ObjectToWorld, v.vertex );

 			o.lightColor = Shade4PointLights (worldPos, o.normal.xyz);
 


			#ifdef FOG_ON		
				float4 viewpos=mul(UNITY_MATRIX_MV, v.vertex);

				//体积雾
				o.normal.w = -(mul(unity_ObjectToWorld, v.vertex).y + _volumeFogOffset) * _volumeFogDestiy;
				// 大气雾
				o.normal.w= max(length(viewpos.xyz) + _fogDestance, o.normal.w);
 			#endif
				    
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 Mask = tex2D( _Control, i.uv[4].xy );
			fixed3 lay1 = tex2D( _Splat0, i.uv[0].xy );
			fixed3 lay2 = tex2D( _Splat1, i.uv[1].xy );
			fixed3 lay3 = tex2D( _Splat2, i.uv[2].xy );
			fixed3 lay4 = tex2D(_Splat3, i.uv[3].xy);
    		fixed4 c;
			c.rgb = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b + lay4.xyz * Mask.a);
			c.a=1;

 
			#ifdef LIGHTMAP_ON
				 
						fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,  i.uv[5]));
						c.rgb*=lm;
 
           		// c.rgb *= DecodeLogLuv(tex2D(unity_Lightmap, i.uv[4]));
            #endif
			
				 
			c.rgb+=i.lightColor;
			c=c* _Color*(_Lighting+ _HdrIntensity);
			

			#ifdef FOG_ON
		//	c.a=i.fogFactor;
			c.a = saturate(exp2(-i.normal.w / _fogDestiy));
			#endif
				return c;
		}
		ENDCG
    }
    
 
}
} 