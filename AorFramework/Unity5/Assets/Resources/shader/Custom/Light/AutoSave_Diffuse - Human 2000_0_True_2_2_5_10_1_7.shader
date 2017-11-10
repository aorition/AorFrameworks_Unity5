// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//@@@DynamicShaderInfoStart
//写深度的单面的Alpha混合的shader,层:Geometry
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/Light/Diffuse - Human 2000_0_True_2_2_5_10_1_7"  {  
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
	_shadowDark("Shadow Dark", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
}

SubShader {
 //@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Geometry" } 
//@@@DynamicShaderTagsRepaceEnd
 
	Pass {
	 Tags {  "LightMode" = "Vertex"}  
	 //@@@DynamicShaderBlendRepaceStart
Blend SrcAlpha OneMinusSrcAlpha,One DstAlpha
ZTest Less
 ZWrite On
 Cull Back
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
				//	return _DirectionalLight;
                //基础色
				fixed4 texColor = tex2D(_MainTex, i.texcoord);
	            fixed4 finalColor;
				finalColor.rgb=texColor*_Lighting;
				finalColor*=_TintColor;
				 
                //环境光
				//diffuse = Kd * lightColor * max(N·L,0)。
				 float4 diffuse =_DirectionalLightDir.w* _DirectionalLightColor* max(0, dot( i.normal,normalize(_DirectionalLightDir.xyz) ));  
				 diffuse+=_AmbientColor;
				 diffuse.rgb+=i.lightColor;

                fixed3 backLight =CustomClamp( (1-max( dot( i.normal, i.eyeDir.xyz ),0))*i.normal.y,0,1)*_BacklightPower*_AmbientColor;
                //控制贴图 
				fixed3 ctrl=tex2D(_CtrlTex, i.texcoord).rgb;
                //高光
				float3 spLight=pow(CustomClamp(max(dot( i.normal,normalize(i.eyeDir+_DirectionalLightDir.rgb) ),0),0,0.98),_specularRange)*_specularPower*ctrl;


                //合成
				 diffuse.rgb+=spLight+backLight;
				 finalColor.rgb*=diffuse;
				
				 finalColor.rgb+=_emission*texColor;
				      //上下明暗
				finalColor.rgb*=CustomClamp((-i.pos.x+_shadowDark),0,1);
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
