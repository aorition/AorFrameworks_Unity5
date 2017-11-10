// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//@@@DynamicShaderInfoStart
//笑傲经典人物材质 Y方向做渐变处理 
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/Light/Diffuse - Human  - NormalMap##" {
//@@@DynamicShaderTitleRepaceEnd
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_NormalTex("NormalTex", 2D) = "white" {}
	_CtrlTex("Ctrl Texture", 2D) = "white" {}

	_TintColor ("Main Color", Color) = (1,1,1,1)

	_Lighting ("Lighting",  float) = 1
	_BacklightPower ("Back light Power", float) = 1
	_specularRange("Specular Range", float) = 1
	_specularPower("Specular Power", float) = 1
	_emission("Emission", float) = 0
//	_shadowDark("Shadow Dark", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
}

SubShader {
 //@@@DynamicShaderTagsRepaceStart
	Tags { "Queue"="Geometry" "IgnoreProjector"="True"  "RenderType"="Geometry"}
 //@@@DynamicShaderTagsRepaceEnd
 
	Pass {
	 Tags {  "LightMode" = "Vertex"}  
	 //@@@DynamicShaderBlendRepaceStart

	 //@@@DynamicShaderBlendRepaceEnd


		CGPROGRAM
			
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma multi_compile FOG_OFF FOG_ON 
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"
			#pragma target 3.0
			struct appdata_normal {
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
				float2 texcoord: TEXCOORD0;
				half4 tangent : TANGENT;
			};

            struct v2f {
                half4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                fixed3 eyeDir   : TEXCOORD1;
                fixed3 normal   : TEXCOORD2;
                fixed3 pos   : TEXCOORD3;
				half3 lightColor : TEXCOORD4;
				half3 Tangent : TEXCOORD5;

				#ifdef FOG_ON		
 				half fogFactor: TEXCOORD6;
 				#endif

              //  LIGHTING_COORDS(5,6)  
			};
            sampler2D _CtrlTex;
			sampler2D _NormalTex;

            fixed _specularRange;
            fixed _specularPower;
            fixed _BacklightPower;
			fixed _emission;
            fixed _shadowDark;
			fixed4 _TintColor;

            v2f vert (appdata_normal v)
			{
                v2f o;
             //TANGENT_SPACE_ROTATION;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos=	v.vertex;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
                half4 worldPos = mul( unity_ObjectToWorld, v.vertex );
                o.eyeDir = normalize( _WorldSpaceCameraPos.xyz - worldPos.xyz ).xyz;
			 //xx
				o.lightColor = Shade4PointLights(worldPos,o.normal);
			 
										
				#ifdef FOG_ON		
				float4 viewpos=mul(UNITY_MATRIX_MV, v.vertex);
 				o.fogFactor=max(length(viewpos.xyz) + _fogDestance, 0.0);
 				#endif

				// TRANSFER_VERTEX_TO_FRAGMENT(o);  
				 o.Tangent = mul(v.tangent.xyz, (float3x3)unity_WorldToObject);
                return o;
            }



			
			fixed4 frag (v2f i) : COLOR
			{

                //基础色
				fixed4 texColor = tex2D(_MainTex, i.texcoord);
	          

				 
                //直射光
				 float4 diffuse =_DirectionalLightDir.w* _DirectionalLightColor* max(0, dot( i.normal,normalize(_DirectionalLightDir.xyz) ));  
				 diffuse.rgb+=i.lightColor;

				 //normal map
				 fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, i.texcoord));
				 float3 tangent = normalize(i.Tangent.rgb);
				 float3 normal = normalize(i.normal);
				 float3 binormal = cross(normal, tangent);
				 float3x3 tbnMatrix = float3x3(tangent, binormal, normal);

				 normal = mul(normalMap.xyz, tbnMatrix);
				 fixed3 dump = saturate(dot(normal, normalize(_DirectionalLightDir.rgb)));
 
				 //背光
				 fixed3 backLight = clamp((1 - max(dot(normal, i.eyeDir.xyz), 0)), 0, 1);
				 backLight = pow(backLight, 2);
				 diffuse.rgb *= dump *(1 - backLight);
				 backLight *= min((normal.y + 0.5), 1.0) * _BacklightPower * _DirectionalLightColor.xyz * _DirectionalLightDir.w;
			
                //高光控制贴图 
				fixed3 ctrl=tex2D(_CtrlTex, i.texcoord).rgb;
				float3 spLight=pow(clamp(max(dot(normal,normalize(i.eyeDir+_DirectionalLightDir.rgb) ), 0),0,0.98), _specularRange)*_specularPower*_DirectionalLightDir.w * _DirectionalLightColor.xyz;
				spLight = (spLight + backLight) * ctrl;
			
				//合成
				
				fixed4 finalColor;
				
				finalColor.rgb=(diffuse + _AmbientColor.xyz) * texColor.xyz;
 
				finalColor.rgb+=_emission*texColor + spLight;
				finalColor.rgb*=_Lighting;
				finalColor.rgb*= _TintColor;
				finalColor.a = 1;

                //先clip，再fog 不然会出错	
 			#ifdef CLIP_ON
			clip(  texColor.a-_CutOut);
             #endif

 			#ifdef FOG_ON
			finalColor.a = exp2(- i.fogFactor /_fogDestiy);
			#else
			finalColor.a=1;
            #endif

		return finalColor;
            }

		ENDCG
	}
}
	Fallback "Unlit/Texture"
}
