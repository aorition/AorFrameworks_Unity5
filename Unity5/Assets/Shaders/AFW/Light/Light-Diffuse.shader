//@@@DynamicShaderInfoStart
//<readonly> 实时光照材质 <Fog><Clip><LightMap><Shadow><VertexAnim>
//@@@DynamicShaderInfoEnd
Shader "AFW/Light/Light - Diffuse"
{
	Properties
	{
		_TintColor("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
		[Space(10)]
		_SunLightThreshold("SunLight Threshold",Range(0,1)) = 0.25
		_ShadowThreshold ("Shadow Threshold", Range(0,1)) = 1
		_AmbientThreshold("Ambient Threshold",Range(0,1)) = 0.2
		[Space(10)]
		[Toggle] _Fog("Fog?", Float) = 1
		[Toggle] _Clip("Clip?", Float) = 0
		_CutOut("CutOut", Float) = 0
		_Lighting("Lighting", Float) = 1
		[HideInInspector][Toggle] _NoShadowCascades("No Shadow Cascades?", Float) = 0
		[Space(10)]
		[Toggle] _VexAnim("Vertex Animation?", Float) = 0
		_power("Noise Power",  Range(0.001,0.1)) = 0.2
		_wind("Wind",  Range(1,10)) = 1
		[Space(10)]
		//此段可以将 ZWrite 选项暴露在Unity的Inspector中
		[Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 1
		//此段可以将 Ztest  选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		//此段可以将 Cull  选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
		//此段可以将 Blend 选项暴露在Unity的Inspector中
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "Lighting.cginc"  
			#include "AutoLight.cginc"

			#pragma target 2.0

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile ___ LIGHTMAP_ON

			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase

			#pragma shader_feature _FOG_ON
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _VEXANIM_ON

			#pragma shader_feature NOSHADOWCASCADES

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _TintColor;
			fixed4 _ShadowColor;

			fixed _Lighting;
			fixed _CutOut;

			fixed _SunLightThreshold;
			fixed _ShadowThreshold;
			fixed _AmbientThreshold;

			fixed _wind;
			fixed _power;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD1;
				#endif
				fixed4  color : COLOR;
				fixed3	normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				#ifdef _FOG_ON
					UNITY_FOG_COORDS(1)
				#endif
				float3 normal   : TEXCOORD2;
				LIGHTING_COORDS(3,4)
				float3 worldPos : TEXCOORD5;
				#ifdef LIGHTMAP_ON
					float2 lightmapUV : TEXCOORD6;
				#endif
			};

			v2f vert (a2v v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				#ifdef _VEXANIM_ON
				    float2 pos = frac(v.vertex.xy / 128.0f) * 128.0f + float2(-64.340622f, -72.465622f);
				    float c = frac(dot(pos.xyx * pos.xyy, float3(20.390625f, 60.703125f, 2.4281209f)));
				    half3 normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				    v.vertex.xyz += v.color.rgb*v.normal*sin(_Time*c*_wind)*_power;
				    o.pos = UnityObjectToClipPos(v.vertex);
				#endif

				#ifdef LIGHTMAP_ON
				    o.lightmapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				
				#ifdef _FOG_ON
					UNITY_TRANSFER_FOG(o,o.pos);
				#endif

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			inline fixed ShadowATTENUATION(v2f i){
				fixed atten = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
				#ifdef NOSHADOWCASCADES
					float4 shadowCoord = mul(unity_WorldToShadow[0],unityShadowCoord4(i.worldPos,1));
					float3 s = abs((shadowCoord - 0.5) * 2);
					atten = max(1-step(max(max(s.x, s.y), s.z), 1), atten); 
				#endif
				return atten;
			}

			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);

				fixed alpha = col.a * _TintColor.a;

				#ifdef _CLIP_ON
					clip(alpha - _CutOut);
				#endif

				//shadow
				fixed atten = ShadowATTENUATION(i); 
				fixed3 shadow = lerp(_ShadowColor, col.rgb, atten);

				#ifdef LIGHTMAP_ON
					fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV.xy));
					col.rgb *= lm;
				#else

					float3 worldNormal = normalize(i.normal); // 法线方向
					float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos)); // 光照方向
					float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos)); // 视角方向
					
					col.rgb = _TintColor * _LightColor0.rgb * max(0, dot(worldNormal, worldLightDir));// 漫反射

					shadow = col.rgb * shadow;
				#endif

				col.rgb = lerp(col.rgb, col.rgb * _LightColor0.rgb * _LightColor0.w, _SunLightThreshold);
				col.rgb = lerp(col.rgb, shadow, _ShadowThreshold);
				col.rgb *= _TintColor.rgb;
				
				fixed4 final = fixed4(col.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientThreshold, alpha);
				final.rgb *= _Lighting;

				// apply fog
				#ifdef _FOG_ON
					UNITY_APPLY_FOG(i.fogCoord, final);
				#endif

				return final;
			}
			ENDCG
		}//end pass

		//Additional Pass
		Pass{
			Tags{ "LightMode" = "ForwardAdd" }
			//开启混合模式，让Additional Pass计算得到的光照结果可以在帧缓存中与之前的光照结果叠加
			
			///	Fog{ Color (0,0,0,0)}
			
			//如果没有开启，Additional Pass会直接覆盖之前的光照结果。
			//?Blend SrcAlpha One
			Blend One One

			CGPROGRAM
			//这个指令可以保证我们在Additional Pass中访问到正确的光照变量
			#pragma multi_compile_fwdadd
			#pragma vertex vert
			#pragma fragment frag
	 		#pragma multi_compile_fog
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _TintColor;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2  uv : TEXCOORD2;
			 	UNITY_FOG_COORDS(3)
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

		 		UNITY_TRANSFER_FOG(o, o.pos);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{

				//主贴图颜色
				fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 worldNormal = normalize(i.normal);
				//平行光
				#ifdef USING_DIRECTIONAL_LIGHT
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					//其他光源
				#else
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				#endif
				//漫反射计算
				fixed3 diffuse = _LightColor0.rgb * _TintColor.rgb * max(0,dot(worldNormal,worldLightDir));
				//平行光没有衰减
				#ifdef USING_DIRECTIONAL_LGITH
					fixed atten = 1.0;
				#else
					//点光源
					#if defined(POINT)
						float3 lightCoord = mul(unity_WorldToLight,float4(i.worldPos,1)).xyz;
						//使用点到光源的距离值的平方来取样，可以避开开方操作
						//使用宏UNITY_ATTEN_CHANNEL来得到衰减纹理中衰减值所在的分量，以得到最终的衰减值。
						fixed atten = tex2D(_LightTexture0,dot(lightCoord,lightCoord).rr).UNITY_ATTEN_CHANNEL;
					//聚光灯
					#elif defined(SPOT)
						float4 lightCoord = mul(unity_WorldToLight,float4(i.worldPos,1));
						//角度衰减，距离衰减
						fixed atten = (lightCoord.z > 0) * tex2D(_LightTexture0,lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0,dot(lightCoord,lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#else
						fixed atten = 1.0;
					#endif
				#endif

				fixed4 finial = fixed4(diffuse * atten * col, 1.0);
				fixed4 black = fixed4(0, 0, 0, 1);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, finial, black);

				return finial;
			}
			ENDCG
		}//end pass

		Pass//ShadowCaster
        {

            Tags { "LightMode" = "ShadowCaster" }
           
            Fog { Mode Off }
            ZWrite On ZTest Less Cull Off
            Offset 1, 1
             
            CGPROGRAM
			 
			#include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma fragmentoption ARB_precision_hint_fastest
            
			#pragma shader_feature _CLIP_ON
			#pragma shader_feature _VEXANIM_ON

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _TintColor;
			fixed _Lighting;
			fixed _CutOut;

			fixed _wind;
			fixed _power;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				fixed4  color : COLOR;
			};

            struct v2f
            { 
               //V2F_SHADOW_CASTER;
			   float4 pos:SV_POSITION;
			   float2 uv:TEXCOORD0;
            };
           
            v2f vert(a2v v)
            {
                v2f o;
				o.uv = TRANSFORM_TEX(v.vertex, _MainTex);
				#ifdef _VEXANIM_ON
				    float2 pos = frac(v.vertex.xy / 128.0f) * 128.0f + float2(-64.340622f, -72.465622f);
				    float c = frac(dot(pos.xyx * pos.xyy, float3(20.390625f, 60.703125f, 2.4281209f)));
				    half3 normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				    v.vertex.xyz += v.color.rgb*v.normal*sin(_Time*c*_wind)*_power;
				    o.pos = UnityObjectToClipPos(v.vertex);
				#endif
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed alpha = col.a * _TintColor.a * _Lighting;

				#ifdef _CLIP_ON
					clip(alpha - _CutOut);
				#endif

                SHADOW_CASTER_FRAGMENT(i)
            }
 
            ENDCG
        }//end pass

	}//end SubShader

	FallBack "Mobile/Diffuse"

}//end Shader
