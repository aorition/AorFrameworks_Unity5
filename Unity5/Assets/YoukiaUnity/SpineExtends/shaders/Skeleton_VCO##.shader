Shader "Spine/Skeleton-VCO##"
{
	Properties
	{
		[NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)

		_fPower("Fill Power", Float) = 1
		_fColor("Fill Color (RGB)", Color) = (1,1,1,1)
		//默认 写深度, 开启ZTest
		[Enum(Off,0, On,1)] _zWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		//
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 0

		[Toggle(ENABLE_CUTOFF)] _ENABLE_CUTOFF("Alpha Cutoff?", Float) = 0
		_CutOff("alpha cutoff", Range(0,1)) = 0.1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "FColorOutput" }
		LOD 100

		Fog{ Mode Off }

		Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
		ZWrite[_zWrite]
		ZTest[_zTest]
		Cull[_cull]

		Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma shader_feature ENABLE_CUTOFF
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			//#include "Assets/ObjectBaseShader.cginc"	
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			fixed4 _Color;

			float _fPower;
			float4 _fColor;
			float _vColorOutput;
			fixed _Cutoff;

			float _CutOut;//兼容

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
#ifdef ENABLE_CUTOFF
			clip(col.a - _Cutoff);
#endif
				return col;
			}
			ENDCG
		}

		Pass{
			Name "Caster"
			Tags{ "LightMode" = "ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog{ Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct v2f {
				V2F_SHADOW_CASTER;
				float2  uv : TEXCOORD1;
			};

			v2f vert(appdata_base v) {
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
					o.uv = v.texcoord;
				return o;
			}

			float4 frag(v2f i) : COLOR{
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG

		}

	}

	SubShader{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass{
			ColorMaterial AmbientAndDiffuse
			SetTexture[_MainTex]{
				Combine texture * primary DOUBLE, texture * primary
			}
		}

	}

}
