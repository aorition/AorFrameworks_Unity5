// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//@@@DynamicShaderInfoStart
//不写深度的双面的Alpha混合的shader,层:Transparent
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Custom/NoLight/Unlit - Holographic 3000_0_False_2_0_5_10_1_7"  {  
//@@@DynamicShaderTitleRepaceEnd

    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
        _lightValue ("Light", Float ) = 0.5
		_flash ("AnimFlash", 2D) = "white" {}
	 	 _speed(" AnimSpeed", Float ) = 1
    }
    SubShader {
 

        Pass {
      
            Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Geometry" }

            		   	
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
         //   #pragma target 3.0
			 uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			 uniform sampler2D _flash; 
 			 uniform float _lightValue;
 			uniform float    _speed;
 		    uniform float4 _OutlineColor;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 I : TEXCOORD1;
             	float3 normal   : TEXCOORD2;
                float3 eyeDir   : TEXCOORD3;
                float3 lightDir : TEXCOORD4; 
     
            };
            v2f vert (appdata_tan v) {
                v2f o;
                o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize( mul( unity_ObjectToWorld, half4( v.normal, 0 ) ).xyz );
                float3 worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);
                float3 viewDir = WorldSpaceViewDir( v.vertex );
                o.lightDir = WorldSpaceLightDir(v.vertex);  
               	o.I = reflect( -viewDir, worldN );
				half4 worldPos = mul( unity_ObjectToWorld, v.vertex );
				o.eyeDir.xyz = normalize( _WorldSpaceCameraPos.xyz - worldPos ).xyz;
                return o;
            }
            
            
            
      fixed4 frag(v2f i) : COLOR {
		fixed4 col=tex2D(_MainTex,i.uv0);
		fixed body=pow((1- saturate(dot(i.eyeDir.xyz , i.normal))),3);

		fixed2 uv=i.uv0;
		uv+=_Time*fixed2(0,_speed);
		fixed4 animTex=tex2D(_flash,uv);
		col=col*_lightValue+max(0,body)*_OutlineColor*10+animTex*0.5;
		col.a=1;
		return   col;
       }
       ENDCG
 }
        
 Pass {
	//@@@DynamicShaderTagsRepaceStart
Tags {   "Queue"="Transparent" } 
//@@@DynamicShaderTagsRepaceEnd
	//@@@DynamicShaderBlendRepaceStart
Blend SrcAlpha OneMinusSrcAlpha,One DstAlpha
ZTest Less
 ZWrite Off
 Cull Off
//@@@DynamicShaderBlendRepaceEnd
	CGPROGRAM
	#include "UnityCG.cginc"
	#pragma vertex vert
	#pragma fragment frag
			

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
	
		};
		struct v2f {
			float4 pos : POSITION;
			float4 color : COLOR;
		};
		uniform float _Outline;
		uniform float4 _OutlineColor;
		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
			float2 offset = TransformViewToProjection(norm.xy);
			o.pos.xy += offset * o.pos.z * _Outline/o.pos.w;
			o.color = _OutlineColor;
			return o;
		}

	
			half4 frag(v2f i) :COLOR {
				return i.color;
			}
			ENDCG
		}
		
    }

}
