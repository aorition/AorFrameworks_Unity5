Shader "T4MShaders/ShaderModel2/MobileVertex/T4M 3 Textures Test for Mobile" {
	Properties{
	_Splat0("Layer 1", 2D) = "white" {}
	_Splat1("Layer 2", 2D) = "white" {}
	_Splat2("Layer 3", 2D) = "white" {}
	_Control("Control (RGBA)", 2D) = "white" {}
	_MainTex("Never Used", 2D) = "white" {}
	}

SubShader{
		Tags{"SplatCount" = "3""RenderType" = "Opaque"  }
		Pass
	{
		Tags{ "LightMode" = "Vertex" }
		CGPROGRAM
		#pragma exclude_renderers xbox360 ps3
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
			// make fog work
		#pragma multi_compile_fog

		#include "UnityCG.cginc"
			struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv0 : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2;
			float2 uv3 : TEXCOORD3;
			//float2 uv4 : TEXCOORD4;
			fixed4 diff : COLOR0;
			UNITY_FOG_COORDS(5)
				float4 vertex : SV_POSITION;
		};
		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0, _Splat1, _Splat2;
		float4 _Splat0_ST, _Splat1_ST, _Splat2_ST;
		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv0 = TRANSFORM_TEX(v.uv, _Control);
			o.uv1 = TRANSFORM_TEX(v.uv, _Splat0);
			o.uv2 = TRANSFORM_TEX(v.uv, _Splat1);
			o.uv3 = TRANSFORM_TEX(v.uv, _Splat2);
			float4 lighting = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1);
			o.diff = lighting;
			UNITY_TRANSFER_FOG(o, o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			// sample the texture
			fixed3 splat_control = tex2D(_Control, i.uv0);
			fixed4 lay1 = tex2D(_Splat0, i.uv1);
			fixed4 lay2 = tex2D(_Splat1, i.uv2);
			fixed4 lay3 = tex2D(_Splat2, i.uv3);
			fixed4 col = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b)*i.diff;

			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
			ENDCG
		}
		
		// Lightmapped
			Pass{
			Tags{ "LightMode" = "VertexLM" }
			Blend One One //ZWrite Off
			ColorMask RGB

			CGPROGRAM
#pragma exclude_renderers xbox360 ps3
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_fog

#include "UnityCG.cginc"

			struct a2v {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
		};

		struct v2f {
			float2 uv0 : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2; 
			float2 uv3 : TEXCOORD3;
			float2 uv4 : TEXCOORD4;
			UNITY_FOG_COORDS(5)
				float4 pos : SV_POSITION;
		};

		uniform float4x4 unity_LightmapMatrix;
		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0, _Splat1, _Splat2;
		float4 _Splat0_ST, _Splat1_ST, _Splat2_ST;
		v2f vert(a2v v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv0 = TRANSFORM_TEX(v.uv, _Control);
			o.uv1 = TRANSFORM_TEX(v.uv, _Splat0);
			o.uv2 = TRANSFORM_TEX(v.uv, _Splat1);
			o.uv3 = TRANSFORM_TEX(v.uv, _Splat2);
			o.uv4 = mul(unity_LightmapMatrix, float4(v.uv1,0,1)).xy;
			UNITY_TRANSFER_FOG(o,o.pos);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv4);
		fixed3 splat_control = tex2D(_Control, i.uv4);
		fixed4 lay1 = tex2D(_Splat0, i.uv1);
		fixed4 lay2 = tex2D(_Splat1, i.uv2);
		fixed4 lay3 = tex2D(_Splat2, i.uv3);
		fixed4 col = lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b;
		col.rgb *= lm.rgb * 2;
		UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
		return col;
		}
			ENDCG
		}

				// Lightmapped, encoded as RGBM
			Pass{
			Tags{ "LightMode" = "VertexLMRGBM" }
			Blend One One// ZWrite Off
			ColorMask RGB

			CGPROGRAM
#pragma exclude_renderers xbox360 ps3
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_fog

#include "UnityCG.cginc"

			struct a2v {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
		};

		struct v2f {
			float2 uv0 : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2;
			float2 uv3 : TEXCOORD3;
			float2 uv4 : TEXCOORD4;
			UNITY_FOG_COORDS(5)
				float4 pos : SV_POSITION;
		};

		uniform float4x4 unity_LightmapMatrix;
		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0, _Splat1, _Splat2;
		float4 _Splat0_ST, _Splat1_ST, _Splat2_ST;
		v2f vert(a2v v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv0 = TRANSFORM_TEX(v.uv, _Control);
			o.uv1 = TRANSFORM_TEX(v.uv, _Splat0);
			o.uv2 = TRANSFORM_TEX(v.uv, _Splat1);
			o.uv3 = TRANSFORM_TEX(v.uv, _Splat2);
			o.uv4 = mul(unity_LightmapMatrix, float4(v.uv1, 0, 1)).xy;
			UNITY_TRANSFER_FOG(o,o.pos);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv4);
		lm *= lm.a * 2;
		fixed3 splat_control = tex2D(_Control, i.uv4);
		fixed4 lay1 = tex2D(_Splat0, i.uv1);
		fixed4 lay2 = tex2D(_Splat1, i.uv2);
		fixed4 lay3 = tex2D(_Splat2, i.uv3);
		fixed4 col = lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b;
		col.rgb *=  lm.rgb * 4;
		UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
		return col;
		}
			ENDCG
			}

				// Pass to render object as a shadow caster
			Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"

			struct v2f {
			V2F_SHADOW_CASTER;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
		}

		float4 frag(v2f i) : SV_Target
		{
			SHADOW_CASTER_FRAGMENT(i)
		}
			ENDCG
		}


	}
		FallBack "Mobile/VertexLit"
}
