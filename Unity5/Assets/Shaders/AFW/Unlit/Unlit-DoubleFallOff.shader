//@@@DynamicShaderInfoStart
//<readonly>用了两个pss的fallOff 类似风暴英雄的诺娃
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - DoubleFallOff" 
{

    Properties 
	{
        _MainTex ("MainTex", 2D) = "white" {}
		_Color("Outline Color", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
        _lightValue ("Light", Float ) = 0.5
		_flash ("AnimFlash", 2D) = "white" {}
	 	_speed(" AnimSpeed", Float ) = 1

		[Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2

    }

    SubShader 
	{

		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Geometry" }

        Pass 
		{
      
			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]
            		   	
            CGPROGRAM

			#include "UnityCG.cginc"

			//#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag
			
			uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
			uniform sampler2D _flash; 
 			uniform float _lightValue;
 			uniform float _speed;
 		    uniform float4 _OutlineColor;

			float4 _Color;

            struct v2f 
			{
               float4 pos : SV_POSITION;
               float2 uv0 : TEXCOORD0;
               float3 I : TEXCOORD1;
			   float3 normal : TEXCOORD2;
               float3 eyeDir : TEXCOORD3;
               float3 lightDir : TEXCOORD4; 
            };

            v2f vert (appdata_tan v) 
			{
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
            
			fixed4 frag(v2f i) : SV_Target 
			{
				fixed4 col=tex2D(_MainTex,i.uv0);
				fixed3 body=pow((1- saturate(dot(i.eyeDir.xyz , i.normal))),2)*_Color*_lightValue;
		 
				//return fixed4(body, 1);
				fixed2 uv=i.uv0;
				uv+=_Time*fixed2(0,_speed);
				fixed4 animTex=tex2D(_flash,uv);
				col.rgb= body*col*_lightValue + animTex*_Color*4;
				col.a=1;
		 
				return col;
			}
			ENDCG
		}
        
		Pass 
		{
			
			Cull back
			ZWrite Off
			ZTest Always//始终通过深度测试，即可以渲染
			ColorMask RGB // alpha not used
			Blend SrcColor  One  // Normal
			
			CGPROGRAM

			#include "UnityCG.cginc"
			
			#pragma vertex vert
			#pragma fragment frag
			
			uniform float _Outline;
			uniform float4 _OutlineColor;

			struct appdata 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : POSITION;
				float4 color : COLOR;
			};
				
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(norm.xy);
				o.pos.xy += offset * o.pos.z * _Outline/o.pos.w;
				o.color = _OutlineColor;
				return o;
			}

	
			fixed4 frag(v2f i) : SV_Target 
			{
				return i.color;
			}
			ENDCG
		}//end pass
    }//end SubShader
}//end Shader
