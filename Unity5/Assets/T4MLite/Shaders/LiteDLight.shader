Shader "T4MShaders/T4M-Lite-Dlight" {
	Properties{
		_Splat0("Layer1 (RGB)", 2D) = "white" {}
		_Splat1("Layer2 (RGB)", 2D) = "white" {}
		_Splat2("Layer3 (RGB)", 2D) = "white" {}
		_Splat3("Layer4 (RGB)", 2D) = "white" {}
		_Control("Control (RGBA)", 2D) = "white" {}
		_OverColor("Override Color (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{
		"Queue" = "Geometry"
		"IgnoreProjector" = "True"
		"RenderType" = "Geometry"
	}

		Pass{
		Tags{ "LightMode" = "ForwardBase" }
		Lighting On
			CGPROGRAM
			#include "UnityCG.cginc"
            #include"Lighting.cginc"
            #include "AutoLight.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile LIGHTMAP_OFF  LIGHTMAP_ON 
            #pragma multi_compile_fwdbase
            //#pragma multi_compile ___ LIGHTMAP_ON
			#pragma exclude_renderers xbox360 ps3
			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;
			sampler2D _Control;
			sampler2D _OverColor;

			struct v2f {
					float4  pos : SV_POSITION;
					fixed3 lightColor : TEXCOORD6;
					float3 worldNormal:TEXCOORD7;
					LIGHTING_COORDS(8, 9)
			#ifdef LIGHTMAP_ON
					float2  uv[6] : TEXCOORD0;
			#endif
			#ifdef LIGHTMAP_OFF
					float2  uv[5] : TEXCOORD0;
			#endif
			};

			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;
			float4 _Control_ST;

			#ifdef LIGHTMAP_ON
				/// fixed4 unity_LightmapST;
				// sampler2D unity_Lightmap;
			#endif

			v2f vert(appdata_full  v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv[1] = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv[2] = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv[3] = TRANSFORM_TEX(v.texcoord, _Splat3);
				o.uv[4] = TRANSFORM_TEX(v.texcoord, _Control);
				half4  worldPos = mul(unity_ObjectToWorld, v.vertex);

				half3 normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldNormal = worldNormal;
				fixed3 worldNorma2 = normalize(worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				o.lightColor = fixed3(0, 0, 0);
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				//自定义光源系统在这里 xx
				o.lightColor = _LightColor0.rgb  * saturate(dot(worldNormal, worldLightDir));

					//Shade4PointLights(worldPos, normal);
				#ifdef LIGHTMAP_ON
						o.uv[5] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 over = tex2D(_OverColor, i.uv[4].xy);
				
				fixed4 Mask = tex2D(_Control, i.uv[4].xy);
				fixed3 lay1 = tex2D(_Splat0, i.uv[0].xy);
				fixed3 lay2 = tex2D(_Splat1, i.uv[1].xy);
				fixed3 lay3 = tex2D(_Splat2, i.uv[2].xy);
				fixed3 lay4 = tex2D(_Splat3, i.uv[3].xy);

				fixed3 col;
				col = (lay1.rgb * Mask.r + lay2.rgb * Mask.g + lay3.rgb * Mask.b + lay4.rgb * Mask.a);
				col *= over;
				float  atten = LIGHT_ATTENUATION(i);
			//#ifdef LIGHTMAP_ON
			//	//c.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[5])).rgb+ i.lightColor;
			//#endif
				fixed3 worldNorma2 = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 light= _LightColor0.rgb  * saturate(dot(worldNorma2, worldLightDir));

				//col += ambient*light;
				col = col+light*atten*ambient;
				
				return fixed4(col, 1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}