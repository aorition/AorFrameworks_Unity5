// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//@@@DynamicShaderInfoStart
//笑傲经典人物材质 Y方向做渐变处理 控制贴图R:凹凸 G:高光 B:自反光 实时光照
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/Light/Diffuse - Human 2000_0_True_2_2_1_0"  {  
//@@@DynamicShaderTitleRepaceEnd
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
		 //R:凹凸 G:高光 B:自反光
	_CtrlTex("Ctrl Texture", 2D) = "white" { }

	_Color ("Main Color", Color) = (1,1,1,1)
	_Lighting ("Lighting",  float) = 1

	_BacklightPower ("Back light Power", float) = 1
	_specularRange("Specular Range", float) = 1
	_specularPower("Specular Power", float) = 1
	_shadowDark("Shadow Dark", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1
}

SubShader {
 //@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Geometry" } 
//@@@DynamicShaderTagsRepaceEnd
 
	Pass {Blend One Zero,One Zero
ZTest Less
 ZWrite On
 Cull Back
Tags {  "LightMode" = "Vertex"}  Blend One Zero,One Zero
ZTest Less
 ZWrite On
 Cull Back
//@@@DynamicShaderBlendRepaceStart
Blend One Zero,One Zero
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
            fixed _shadowDark;
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
				fixed4 diffSamplerColor = tex2D(_MainTex, i.texcoord);
                diffSamplerColor.rgb*=_Lighting;
				diffSamplerColor*=_Color;

                //环境光
				fixed3 ambientColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 backLight =CustomClamp( (1-max( dot( i.normal, i.eyeDir.xyz ),0))*(1-max( dot( i.normal, i.eyeDir.xyz ),0))*i.normal.y,0,1)*_BacklightPower*ambientColor;
                //控制贴图 
				fixed3 ctrl=tex2D(_CtrlTex, i.texcoord).rgb;
                //高光
				fixed3 spLight =pow(CustomClamp(max(dot( i.normal, i.eyeDir.xyz ),0),0,0.98),_specularRange)*_specularPower*ctrl.g;
                //合成
				diffSamplerColor.rgb=diffSamplerColor.rgb+spLight;
                diffSamplerColor.rgb+=(backLight+i.lightColor);
                //上下明暗
				diffSamplerColor.rgb*=CustomClamp((-i.pos.x+_shadowDark),0,1);



                //先clip，再fog 不然会出错	
 			#ifdef CLIP_ON
			clip(  diffSamplerColor.a-_CutOut);
             #endif

			

 			#ifdef FOG_ON
			diffSamplerColor.a = exp2(- i.fogFactor /_fogDestiy);
			#else
			diffSamplerColor.a=1;
            #endif

		return diffSamplerColor;
            }

		ENDCG
	}
}
	Fallback "Unlit/Texture"
}
