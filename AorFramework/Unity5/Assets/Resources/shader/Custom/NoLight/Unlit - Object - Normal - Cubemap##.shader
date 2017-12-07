// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//@@@DynamicShaderInfoStart
//无光照物体材质 支持lightMap 定点动画
//@@@DynamicShaderInfoEnd

//@@@DynamicShaderTitleRepaceStart
Shader "Custom/NoLight/Unlit - Object - Normal - Cubemap##"  {
	//@@@DynamicShaderTitleRepaceEnd

	//@@@DynamicShaderPropRepaceStart
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Lighting("Lighting",  float) = 1
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_NormalPower("Normal Power", range(0, 1.4)) = 0.8
		_Normal("Normal", 2D) = "bump" {}
		_ReflectionPower("Reflection Power",range(0,1)) = 0.2
	    _Cubemap("Cubemap", CUBE) = "black" {}


		[Toggle] _Fog("Fog?", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4

		[HideInInspector] _CutOut("CutOut", float) = 0.1
		[HideInInspector] _power("noise Power",  Range(0.001,0.1)) = 0.2
		[HideInInspector] _wind("wind",  Range(1,10)) = 1
	}
		//@@@DynamicShaderPropRepaceEndxx xxxx 

	SubShader{

		//@@@DynamicShaderTagsRepaceStart
		Tags {
			 "Queue" = "Geometry"
		     "IgnoreProjector" = "True"
			 "RenderType" = "Geometry"
		}


		//@@@DynamicShaderTagsRepaceEnd

		//顶点光照
		Pass {
			 Tags { "LightMode" = "Vertex" }
	              Lighting Off
				  SetTexture[_MainTex] { combine texture }
	    }
	   
		Pass {
			 Tags { "LightMode" = "ForwardBase"}
			 ZWrite[_ZWrite]
			 ZTest[_ZTest]
			 Cull[_Cull]
			//@@@DynamicShaderBlendRepaceStart

		    //@@@DynamicShaderBlendRepaceEnd

			CGPROGRAM
		//	#pragma target 3.0
			#pragma multi_compile CLIP_OFF CLIP_ON 
		//	#pragma multi_compile FOG_OFF FOG_ON
			#pragma multi_compile ANIM_OFF ANIM_ON
			#pragma multi_compile ___ LIGHTMAP_ON
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _FOG_ON
			#pragma multi_compile_fog
		//	#pragma multi_compile_fwdbase  
			#include "Assets/ObjectBaseShader.cginc"


			struct appdata_lm {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord: TEXCOORD0;

			#ifdef LIGHTMAP_ON
				float2 lightmapUV : TEXCOORD1;
			#endif

			#ifdef ANIM_ON
				fixed4 vertexColor : COLOR;
			#endif

			};


			sampler2D _Normal;		
            samplerCUBE _Cubemap;
            float _NormalPower;
            float _ReflectionPower;

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;

			#ifdef LIGHTMAP_ON
				float2 lightmapUV : TEXCOORD2;
			#endif

			#if _FOG_ON
				UNITY_FOG_COORDS(3)
			#endif

	            half3 viewDir : TEXCOORD4;
			};


			v2f vert(appdata_lm v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

			#ifdef LIGHTMAP_ON
				o.lightmapUV = v.lightmapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			#endif

				o.viewDir = normalize(WorldSpaceViewDir(v.vertex));

			//顶点动画
			#ifdef ANIM_ON
				float4 vColor = v.vertexColor;
				float2 pos = frac(v.vertex.xy / 128.0f) * 128.0f + float2(-64.340622f, -72.465622f);
				float c = frac(dot(pos.xyx * pos.xyy, float3(20.390625f, 60.703125f, 2.4281209f)));
				half3 normal = mul(SCALED_NORMAL, (float3x3)unity_WorldToObject);
				v.vertex.xyz += vColor.rgb*v.normal*sin(_Time*c*_wind)*_power;
				o.vertex = UnityObjectToClipPos(v.vertex);
			#else
			#endif

			#if _FOG_ON
				UNITY_TRANSFER_FOG(o, o.vertex);
			#endif

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{

				fixed4 col = tex2D(_MainTex, i.texcoord);

				//normal & reflection(cubemap)
				half3 nrm = UnpackNormal(tex2D(_Normal, i.texcoord.xy));
				nrm = (nrm + i.viewDir.xyz) * _NormalPower;
				half3 reflectVector = reflect(-i.viewDir.xyz, nrm.xyz);
				fixed4 reflection = texCUBE(_Cubemap, reflectVector);
                
		    #ifdef LIGHTMAP_ON
				fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV.xy));
				col.rgb *= lm;
			#endif
	 
				col.rgb *= (_Lighting + _HdrIntensity);
				col *= _Color;

				col += reflection * _ReflectionPower;


			#if _FOG_ON
				UNITY_APPLY_FOG(i.fogCoord, col);
			#endif

			#ifdef CLIP_ON
				clip(col.a - _CutOut);
			#endif

			return col;

			}
		    ENDCG
	    }

	}


}