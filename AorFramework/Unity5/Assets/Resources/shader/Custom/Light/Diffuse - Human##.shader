// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//@@@DynamicShaderInfoStart
//笑傲经典人物材质 Y方向做渐变处理 
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/Light/Diffuse - Human##" {
//@@@DynamicShaderTitleRepaceEnd
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
		 //R:凹凸 G:高光 B:自反光
	_CtrlTex("Ctrl Texture", 2D) = "white" { }

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
		//	#pragma multi_compile_fwdbase  
			#include "Assets/ObjectBaseShader.cginc"
            
			struct appdata_normal {
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
				float2 texcoord: TEXCOORD0;
				
			};

            struct v2f {
                half4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                fixed3 eyeDir   : TEXCOORD1;
                fixed3 normal   : TEXCOORD2;
                fixed3 pos   : TEXCOORD3;
                fixed3 lightColor : TEXCOORD4;

				#ifdef FOG_ON		
 				half fogFactor: TEXCOORD5;
 				#endif

              //  LIGHTING_COORDS(5,6)  
			};
            sampler2D _CtrlTex;
            fixed _specularRange;
            fixed _specularPower;
            fixed _BacklightPower;
			fixed _emission;
            fixed _shadowDark;
			fixed4 _TintColor;

            v2f vert (appdata_normal v)
			{
                v2f o;
             //  TANGENT_SPACE_ROTATION;
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

                return o;
            }



			
			fixed4 frag (v2f i) : COLOR
			{
 

				//基础色
				fixed4 texColor = tex2D(_MainTex, i.texcoord);

			//环境光
			float4 diffuse = _DirectionalLightDir.w* _DirectionalLightColor* max(0, dot(i.normal, normalize(_DirectionalLightDir.xyz)));
			diffuse.rgb += i.lightColor;

			fixed3 dump = saturate(dot(i.normal, normalize(_DirectionalLightDir.rgb)));
			//背光
			fixed3 backLight = clamp((1 - max(dot(i.normal, i.eyeDir.xyz), 0)), 0, 1);
			backLight = pow(backLight, 2);
			diffuse.rgb *= dump *(1 - backLight);
			backLight *= min((i.normal.y + 0.5), 1.0) * _BacklightPower * _DirectionalLightColor.xyz * _DirectionalLightDir.w;


			//高光控制贴图 
			fixed3 ctrl = tex2D(_CtrlTex, i.texcoord).rgb;

			float3 spLight = pow(clamp(max(dot(i.normal, normalize(i.eyeDir + _DirectionalLightDir.rgb)), 0), 0, 0.98), _specularRange)*_specularPower*_DirectionalLightDir.w * _DirectionalLightColor.xyz;
			spLight = (spLight + backLight) * ctrl;

			//合成
			fixed4 finalColor;

			finalColor.rgb = (diffuse + i.lightColor + _AmbientColor.xyz) * texColor.xyz;
			finalColor.rgb += _emission*texColor + spLight;
			finalColor.rgb *= _Lighting;
			finalColor.rgb *= _TintColor;

			 //上下明暗
			//	finalColor.rgb*=clamp((-i.pos.x+_shadowDark),0,1);


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
