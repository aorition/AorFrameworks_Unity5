Shader "T4MLiteShaders/T4MLite_Unit"
{
	Properties
	{

		_Splat0("Layer1 (RGB)", 2D) = "white" {}
		_Splat1("Layer2 (RGB)", 2D) = "white" {}
		_Splat2("Layer3 (RGB)", 2D) = "white" {}
		_Splat3("Layer4 (RGB)", 2D) = "white" {}
		_Control("Control (RGBA)", 2D) = "white" {}
		_OverColor("Override Color (RGB)", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)
		_Lighting("Lighting", Float) = 1
	}

	SubShader
	{

		Tags{ "Queue" = "Geometry" "RenderType" = "Geometry" }

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			Lighting On

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile LIGHTMAP_OFF  LIGHTMAP_ON 
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma exclude_renderers xbox360 ps3

			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;
			sampler2D _Control;
			sampler2D _OverColor;

			float _Lighting;
			float4 _Color;

			struct v2f
			{
				float4 pos : SV_POSITION;

		#ifdef LIGHTMAP_ON
				float2  uv[6] : TEXCOORD0;
		#endif
		#ifdef LIGHTMAP_OFF
				float2  uv[5] : TEXCOORD0;
		#endif

				LIGHTING_COORDS(6, 7)
				UNITY_FOG_COORDS(8)
			};

			fixed4 _Splat0_ST;
			fixed4 _Splat1_ST;
			fixed4 _Splat2_ST;
			fixed4 _Splat3_ST;
			fixed4 _Control_ST;

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv[1] = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv[2] = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv[3] = TRANSFORM_TEX(v.texcoord, _Splat3);
				o.uv[4] = TRANSFORM_TEX(v.texcoord, _Control);

		#ifdef LIGHTMAP_ON
				o.uv[5] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif

				UNITY_TRANSFER_FOG(o, o.pos);

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				fixed3 over = tex2D(_OverColor, i.uv[4].xy);

				fixed4 Mask = tex2D(_Control, i.uv[4].xy);
				fixed3 lay1 = tex2D(_Splat0, i.uv[0].xy);
				fixed3 lay2 = tex2D(_Splat1, i.uv[1].xy);
				fixed3 lay3 = tex2D(_Splat2, i.uv[2].xy);
				fixed3 lay4 = tex2D(_Splat3, i.uv[3].xy);

				fixed3 col;
				col = (lay1.rgb * Mask.r + lay2.rgb * Mask.g + lay3.rgb * Mask.b + lay4.rgb * Mask.a);
				col *= over;

				fixed4 finial = fixed4(col, 1);
				finial.rgb *= _Color;
				finial.rgb *= _Lighting;

			#ifdef LIGHTMAP_ON
				finial.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[5])).rgb;
			#endif

				UNITY_APPLY_FOG(i.fogCoord, finial);

				return finial;
				}
				ENDCG
			}
		}
}