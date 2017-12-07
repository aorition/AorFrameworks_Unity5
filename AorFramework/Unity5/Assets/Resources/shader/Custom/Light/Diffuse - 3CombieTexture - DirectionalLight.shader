// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//带(主光)高光参数的3层混合地表贴图
//@@@DynamicShaderInfoEnd

Shader "Custom/Light/Diffuse - 3CombieTexture - DirectionalLight" {
Properties {
    _Splat0 ("Layer1 (RGB)", 2D) = "white" {}
	_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
	_Splat2 ("Layer3 (RGB)", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	_Lighting ("Lighting",  float) = 1
	_SpPower("SpPower", float) = 1
	_SpRange("SpRange", float) = 12
	[Toggle] _Fog("Fog?", Float) = 1
}
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True"  "RenderType"="Geometry"}

	LOD 600
 

    Pass {
	  
	   Tags {  "LightMode" = "ForwardBase" } 
 
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile ___ LIGHTMAP_ON
		#pragma shader_feature _FOG_ON
		#pragma multi_compile_fog
		#include "Assets/ObjectBaseShader.cginc"

		float _SpRange;
		float _SpPower;
		sampler2D _Splat0 ;
		sampler2D _Splat1 ;
		sampler2D _Splat2 ;
		sampler2D _Control;

		struct v2f {
			half4  pos : SV_POSITION;
			half2  uv[5]:TEXCOORD0;
			#if _FOG_ON
				UNITY_FOG_COORDS(6)
			#endif

				half3 eyeDir   : color;
				half4 normal   : TEXCOORD7;
 
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
			o.uv[3] = TRANSFORM_TEX(v.texcoord, _Control);
			o.uv[4] = half2(0, 0);

			o.normal.xyz = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
			o.normal.w = 0;

			#ifdef LIGHTMAP_ON
            	o.uv[4] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
            
				half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);



			#if _FOG_ON
				UNITY_TRANSFER_FOG(o, o.pos);
			#endif
				    
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
 
			//主光方向
			half3 lightDirection = _DirectionalLightDir.xyz;
			half3 normal = i.normal;
			//高光
			half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))), _SpRange),0);
			splight *= _SpPower;

		

			fixed4 Mask = tex2D( _Control, i.uv[3].xy );
			fixed3 lay1 = tex2D( _Splat0, i.uv[0].xy );
			fixed3 lay2 = tex2D( _Splat1, i.uv[1].xy );
			fixed3 lay3 = tex2D( _Splat2, i.uv[2].xy );

    		fixed4 c;
			c.rgb = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b );
			c.a=1;


			#ifdef LIGHTMAP_ON
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,  i.uv[4]));
				c.rgb*=lm;
            #endif
		c = c + UNITY_LIGHTMODEL_AMBIENT + splight*c*Mask.a;
		c=c* _Color*(_Lighting+ _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w;
	 	
	 
		#if _FOG_ON
				UNITY_APPLY_FOG(i.fogCoord, c);
		#endif

			 return c;
		}
		ENDCG
    }
    
 
}

SubShader{
	Tags{
	"RenderType" = "Transparent"
}

LOD 200
 
UsePass "Custom/NoLight/Unlit - 3CombieTexture/BASECOMBIE"

}


} 