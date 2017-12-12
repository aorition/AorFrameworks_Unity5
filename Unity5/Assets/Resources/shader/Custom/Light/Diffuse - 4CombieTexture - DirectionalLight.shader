// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/Light/Diffuse - 4CombieTexture - DirectionalLight" {

Properties {
	_SpPower0 ("SpPower1", float) = 1
    _Splat0 ("Layer1 (RGB)", 2D) = "white" {}
    _SpPower1("SpPower2", float) = 1
	_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
	_SpPower2("SpPower3", float) = 1
	_Splat2 ("Layer3 (RGB)", 2D) = "white" {}
	_SpPower3("SpPower4", float) = 1
	_Splat3("Layer4 (RGB)", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}

	//_SpPower("SpPower", float) = 1
		_SpRange("SpRange", float) = 12

	_Color ("Main Color", Color) = (1,1,1,1)
	_Lighting ("Lighting",  float) = 1

		[Toggle] _Fog("Fog?", Float) = 1
}

SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True"  "RenderType"="Geometry"}

	LOD 600


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

#pragma shader_feature _FOG_ON
#pragma multi_compile_fog

	//	#pragma multi_compile LIGHTMAP_LOGLUV LIGHTMAP_SCALE
		//#pragma multi_compile_fwdbase  
		#include "Assets/ObjectBaseShader.cginc"


		sampler2D _Splat0 ;
		sampler2D _Splat1 ;
		sampler2D _Splat2 ;
		sampler2D _Splat3;
		sampler2D _Control;

		float _SpRange;
		//float _SpPower;

		float _SpPower0;
		float _SpPower1;
		float _SpPower2;
		float _SpPower3;

		struct v2f {
			half4  pos : SV_POSITION;
			float2 uv[6]:TEXCOORD0;

			//fixed3 lightColor:color;  

#if _FOG_ON
			UNITY_FOG_COORDS(6)
#endif

			half3 eyeDir   : color;
			half4 normal : TEXCOORD7;
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
			o.normal.w = 0;

 			half4 worldPos = mul( unity_ObjectToWorld, v.vertex );

			o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);

 			//o.lightColor = Shade4PointLights (worldPos, o.normal.xyz);

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
			//splight *= _SpPower;

			half splight_r = splight * _SpPower0;
			half splight_g = splight * _SpPower1;
			half splight_b = splight * _SpPower2;
			half splight_a = splight * _SpPower3;


			fixed4 Mask = tex2D( _Control, i.uv[4].xy );
			fixed3 lay1 = tex2D( _Splat0, i.uv[0].xy );
			fixed3 lay2 = tex2D( _Splat1, i.uv[1].xy );
			fixed3 lay3 = tex2D( _Splat2, i.uv[2].xy );
			fixed3 lay4 = tex2D(_Splat3, i.uv[3].xy);
    		fixed4 c;
			c.rgb = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b + lay4.xyz* Mask.a);
			c.a=1;

 
			#ifdef LIGHTMAP_ON
				 
						fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,  i.uv[5]));
						c.rgb*=lm;
 
           		// c.rgb *= DecodeLogLuv(tex2D(unity_Lightmap, i.uv[4]));
            #endif
			
				 
			/*c.rgb+=i.lightColor;
			c=c* _Color*(_Lighting+ _HdrIntensity);*/

			//c = c + UNITY_LIGHTMODEL_AMBIENT + splight*c*Mask.b;
			//c += UNITY_LIGHTMODEL_AMBIENT + splight_r*c*Mask.r + splight_g*c*Mask.g + splight_b*c*Mask.b + splight_a*c*Mask.a;
			//c = c* _Color*(_Lighting + _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w;


			c.rgb *= (_Lighting + _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w + UNITY_LIGHTMODEL_AMBIENT.xyz;
			c += splight_r*c*Mask.r + splight_g*c*Mask.g + splight_b*c*Mask.b + splight_a*c*Mask.a;
			c *= _Color;
			
			c.rgb = min(c.rgb, float3(1, 1, 1));
			c.a = 1;


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

UsePass "Custom/NoLight/Unlit - 4CombieTexture/BASECOMBIE"

}


} 