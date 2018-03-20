Shader "T4MLiteShaders/T4MLite_Specular"
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

		_Specular("Specular (invalid in Lightmap mode)", Color) = (1,1,1,1)
		_Gloss("Gloss (invalid in Lightmap mode)", Range(8.0, 256)) = 20
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

			fixed4 _Color;
			fixed4 _Specular;
			float _Gloss;

			float _Lighting;

			struct v2f
			{
				float4 pos : SV_POSITION;

		#ifdef LIGHTMAP_ON
				float2  uv[6] : TEXCOORD0;
		#endif
		#ifdef LIGHTMAP_OFF
				float2  uv[5] : TEXCOORD0;
		#endif

				fixed3 worldPos : TEXCOORD6;
				float3 worldNormal:TEXCOORD7;
				LIGHTING_COORDS(8, 9)
				UNITY_FOG_COORDS(10)
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

				o.worldPos = mul(unity_ObjectToWorld, v.vertex); // 模型坐标顶点转换世界坐标顶点
				o.worldNormal = UnityObjectToWorldNormal(v.normal); // 模型坐标法线转换世界坐标法线

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

#ifdef LIGHTMAP_ON

				finial.rgb *= _Color;

				fixed3 lightMapColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[5])).rgb;
				finial.rbg *= lightMapColor;
#else
				fixed3 worldNormal = normalize(i.worldNormal); // 法线方向
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos)); // 光照方向
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos)); // 视角方向

				fixed3 lightColor = _LightColor0.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Color; //环境光
				float  atten = LIGHT_ATTENUATION(i); //阴影
				fixed3 diffuse = _Color * lightColor * max(0, dot(worldNormal, worldLightDir)); // 漫反射

				diffuse *= _Color.rgb;
				diffuse *= atten;

				fixed3 halfDir = normalize(worldViewDir + worldLightDir); // Blinn模型 计算
				fixed3 specular = lightColor * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss); // 高光反射

				finial.rgb *= fixed4(ambient + diffuse, 1); // 相加后输出颜色;
				finial.rgb += specular;
#endif

				finial.rgb *= _Lighting;

				UNITY_APPLY_FOG(i.fogCoord, finial);

				return finial;
			}
			ENDCG
		}

		//Additional Pass
		Pass{
			Tags{ "LightMode" = "ForwardAdd" }
			//开启混合模式，让Additional Pass计算得到的光照结果可以在帧缓存中与之前的光照结果叠加
			//如果没有开启，Additional Pass会直接覆盖之前的光照结果。
			Blend One One
			CGPROGRAM
			//这个指令可以保证我们在Additional Pass中访问到正确的光照变量
			#pragma multi_compile_fwdadd
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _Splat0;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _Splat3;
			sampler2D _Control;
			sampler2D _OverColor;

			fixed4 _Color;
			fixed4 _Specular;
			float _Gloss;
			
			fixed4 _Splat0_ST;
			fixed4 _Splat1_ST;
			fixed4 _Splat2_ST;
			fixed4 _Splat3_ST;
			fixed4 _Control_ST;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2  uv[5] : TEXCOORD2;
				UNITY_FOG_COORDS(8)
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.uv[0] = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.uv[1] = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.uv[2] = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.uv[3] = TRANSFORM_TEX(v.texcoord, _Splat3);
				o.uv[4] = TRANSFORM_TEX(v.texcoord, _Control);

				UNITY_TRANSFER_FOG(o, o.pos);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{

				//主贴图颜色
				fixed3 over = tex2D(_OverColor, i.uv[4].xy);
				fixed4 Mask = tex2D(_Control, i.uv[4].xy);
				fixed3 lay1 = tex2D(_Splat0, i.uv[0].xy);
				fixed3 lay2 = tex2D(_Splat1, i.uv[1].xy);
				fixed3 lay3 = tex2D(_Splat2, i.uv[2].xy);
				fixed3 lay4 = tex2D(_Splat3, i.uv[3].xy);
				fixed3 col;
				col = (lay1.rgb * Mask.r + lay2.rgb * Mask.g + lay3.rgb * Mask.b + lay4.rgb * Mask.a);
				col *= over;

				fixed3 worldNormal = normalize(i.worldNormal);
				//平行光
		#ifdef USING_DIRECTIONAL_LIGHT
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				//其他光源
		#else
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
		#endif
				//漫反射计算
				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0,dot(worldNormal,worldLightDir));
				
				//高光反射计算
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(worldNormal,halfDir)),_Gloss);
				
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
				
				fixed4 finial = fixed4((diffuse + specular) * atten * col, 1.0);

				fixed4 black = fixed4(0, 0, 0, 1);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, finial, black);
				return finial;
			}
			ENDCG
		}
		
		Pass{

			Name "Caster"

			Tags{ "LightMode" = "ShadowCaster" }
			
			Offset 1, 1
			Cull Off

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON 
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

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			uniform sampler2D _MainTex;

			float4 frag(v2f i) : COLOR
			{
				fixed4 texcol = tex2D(_MainTex, i.uv);
				SHADOW_CASTER_FRAGMENT(i)
			}

			ENDCG

		}

		Pass{


			Name "ShadowCollector"

			Tags{ "LightMode" = "ShadowCollector" }
			Cull Off

			Fog{ Mode Off }
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON 
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcollector

			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_COLLECTOR;
				float2  uv : TEXCOORD5;
			};

			uniform float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_COLLECTOR(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			uniform sampler2D _MainTex;

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 texcol = tex2D(_MainTex, i.uv);
				SHADOW_COLLECTOR_FRAGMENT(i)
			}

			ENDCG

		}
	}
	//FallBack "Specular"
}