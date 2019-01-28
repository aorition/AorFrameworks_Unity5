//@@@DynamicShaderInfoStart
//<readonly>T4M 4通道 support：<LightMap><Fog><Shadow>
//@@@DynamicShaderInfoEnd
Shader "AFW/Unlit/Unlit - 4CombieTexture" 
{
	
	Properties
	{
		_TintColor("Color", Color) = (1,1,1,1)
		[Space(10)]
		_Splat0("Layer1 (RGB)", 2D) = "white" {}
		_Splat1("Layer2 (RGB)", 2D) = "white" {}
		_Splat2("Layer3 (RGB)", 2D) = "white" {}
		_Splat3("Layer4 (RGB)", 2D) = "white" {}
		_Control("Control (RGBA)", 2D) = "red" {}
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
		[Space(10)]
		_SunLightThreshold("SunLight Threshold",Range(0,1)) = 0.25
		_ShadowThreshold ("Shadow Threshold", Range(0,1)) = 1
		_AmbientThreshold("Ambient Threshold",Range(0,1)) = 0.2
		[Space(10)]
		[Toggle] _Fog("Fog?", Float) = 1
		[HideInInspector]_Lighting("Lighting",  float) = 1
	}

	SubShader
	{
		
		Tags{ "RenderType" = "Geometry" }
		
		Pass{
			
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			//#pragma target 2.0

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile ___ LIGHTMAP_ON

			#pragma shader_feature _FOG_ON

			#pragma exclude_renderers xbox360 ps3

			sampler2D _Splat0;
			fixed4 _Splat0_ST;
			sampler2D _Splat1;
			fixed4 _Splat1_ST;
			sampler2D _Splat2;
			fixed4 _Splat2_ST;
			sampler2D _Splat3;
			fixed4 _Splat3_ST;
			
			sampler2D _Control;
			float4 _Control_ST;

			fixed4 _TintColor;
			fixed4 _ShadowColor;
			fixed _Lighting;

			fixed _SunLightThreshold;
			fixed _ShadowThreshold;
			fixed _AmbientThreshold;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD1;
				#endif
				//fixed4  color : COLOR;
				//fixed3	normal : NORMAL;
			};

			struct v2f 
			{	
				fixed4  pos:SV_POSITION;
				fixed2  uv0:TEXCOORD0;
				fixed2  uv1:TEXCOORD1;
				fixed2  uv2:TEXCOORD2;
				fixed2  uv3:TEXCOORD3;
				fixed2  uv4:TEXCOORD4;
				fixed3  worldPos:TEXCOORD5;

				//添加内置宏，声明一个用于阴影纹理采样的坐标，参数是下一个可用的插值寄存器的索引值
				LIGHTING_COORDS(6,7)
				
				#ifdef _FOG_ON
					UNITY_FOG_COORDS(8)
				#endif

				#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD9;
				#endif

			};

			v2f vert (a2v v)
			{
				
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv1 = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv3 = TRANSFORM_TEX(v.texcoord, _Splat3);
				o.uv4 = TRANSFORM_TEX(v.texcoord, _Control);

				#ifdef LIGHTMAP_ON
				    o.lightmapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				o.worldPos = mul(unity_ObjectToWorld, v.vertex); // 模型坐标顶点转换世界坐标顶点
				
				TRANSFER_SHADOW(o);

				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o, o.pos);
				#endif

				return o;
			}

			fixed ShadowATTENUATION(v2f i){
				fixed atten = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
				float4 shadowCoord = mul(unity_WorldToShadow[0],unityShadowCoord4(i.worldPos,1));
				float3 s = abs((shadowCoord - 0.5) * 2);
				atten = max(1-step(max(max(s.x, s.y), s.z), 1), atten); 
				return atten;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 Mask = tex2D(_Control, i.uv4.xy);
				fixed4 lay1 = tex2D(_Splat0, i.uv0.xy);
				fixed4 lay2 = tex2D(_Splat1, i.uv1.xy);
				fixed4 lay3 = tex2D(_Splat2, i.uv2.xy);
				fixed4 lay4 = tex2D(_Splat3, i.uv3.xy);

				fixed3 col = (lay1.xyz * Mask.r + lay2.xyz * Mask.g + lay3.xyz * Mask.b + lay4.xyz * Mask.a);
				fixed alpha =(lay1.w * Mask.r + lay2.w * Mask.g + lay3.w * Mask.b + lay4.a * Mask.a) * _TintColor;

				//shadow
				fixed atten = ShadowATTENUATION(i);

				col.rgb *= _TintColor.rgb;
				col.rgb = lerp(col.rgb, col.rgb * _LightColor0.rgb * _LightColor0.w, _SunLightThreshold);

				fixed3 shadow = lerp(_ShadowColor, col.rgb, atten);
				col.rgb = lerp(col.rgb, shadow, _ShadowThreshold);

				#ifdef LIGHTMAP_ON
					fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV.xy));
					col.rgb *= lm;
				#endif

				fixed4 final = fixed4(col.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientThreshold, alpha);
				final.rgb *= _Lighting;

				#ifdef _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, final);
				#endif

				return final;
			}
			ENDCG
		}//end pass
	}//end subShader

	FallBack "Mobile/Diffuse"

}//end Shader