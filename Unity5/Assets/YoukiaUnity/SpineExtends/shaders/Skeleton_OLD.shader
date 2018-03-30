// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spine/Skeleton_OLD" {
	Properties {
		_CutOff("alpha cutoff", Range(0,1)) = 0.1

		_MainTex ("Texture to blend", 2D) = "black" {}
		_Color("Color", Color) = (1,1,1,1)

		_Stencil("Stencil ID", Float) = 0
		_StencilComp("Stencil Comparison", Float) = 8 // 0 Disabled, 1 Never, 2 Less, 3 Equal, 4 LessEqual, 5 Greater, 6 NotEqual, 7 GreaterEqual, 8 Always
		_StencilOp("Stencil Operation", Float) = 0 // 0 Keep, 1 Zero, 2 Replace, 3 IncrementSaturate, 4 DecrementSaturate 5 Invert, 6 IncrementWrap, 7 DecrementWrap
		// _StencilWriteMask("Stencil Write Mask", Float) = 255
		// _StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		
	}
	// 2 texture stage GPUs
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		// Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		LOD 100

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		// Lighting Off
		
		ColorMask [_ColorMask]
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			// ReadMask [_StencilReadMask]
			// WriteMask [_StencilWriteMask]
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			// #include "Lighting.cginc"
			// #include "AutoLight.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				half2 uv : TEXCOORD0;
				// float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;

				// float3 worldNormal :  TEXCOORD1;
				// float3 worldPos :  TEXCOORD2;

				// SHADOW_COORDS(3)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;
			fixed _Cutoff;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// o.worldNormal = UnityObjectToWorldNormal(v.normal);
				// o.worldPos = UnityObjectToWorldDir(v.vertex);

				// TRANSFER_SHADOW(o)

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * _Color;
				/*
				fixed3 normal = normalize(i.worldNormal);
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed4 albedo = tex2D(_MainTex, i.uv) * _Color;
				// clip(albedo.a - _Cutoff);

				// 环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				// 漫反射
				fixed halfLambert = dot(normal, lightDir) * 0.5 + 0.5;
				fixed3 diffuse = _LightColor0.rgb * albedo.rgb * halfLambert; // saturate(dot(normal, lightDir));
				
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

				return fixed4(ambient + diffuse * atten, albedo.a);
				*/
			}
			ENDCG
		}
		
		Pass { // shadow
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1

			ZWrite On
			ZTest LEqual
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			struct v2f { 
				V2F_SHADOW_CASTER;
				float2  uv : TEXCOORD1;
			};

			uniform float4 _MainTex_ST;

			v2f vert (appdata_base v) {
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;
			float4 frag (v2f i) : COLOR {
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
