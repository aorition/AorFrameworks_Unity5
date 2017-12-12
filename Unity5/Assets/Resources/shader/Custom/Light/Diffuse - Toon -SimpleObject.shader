// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/Light/Diffuse - Toon - SimpleObject" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
	_SpPower("SpPower", float) = 1
		_SpRange("SpRange", float) = 12
	_Color("Color", Color) = (1,1,1,1)
		_Lighting("Lighting", float) = 1

		[Toggle] _Fog("Fog?", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[HideInInspector] _CutOut("CutOut", float) = 0.1

	}
		SubShader{
		Tags{
		"RenderType" = "Transparent"
	}


		LOD 600
		Pass{
		Tags{ "LightMode" = "ForwardBase" }
		Cull[_Cull]
		Blend[_SrcBlend][_DstBlend],[_SrcAlphaBlend][_DstAlphaBlend]
		CGPROGRAM
#pragma multi_compile CLIP_OFF CLIP_ON
#pragma shader_feature _FOG_ON
#pragma multi_compile_fog
#pragma multi_compile ___ LIGHTMAP_ON

#pragma vertex vert
#pragma fragment frag
#include "Assets/ObjectBaseShader.cginc"	


	float _SpRange;
	float _SpPower;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
		half4 normal   : TEXCOORD1;
		half3 eyeDir   : TEXCOORD2;
		float3 lightColor : TEXCOORD5;    //点光

#ifdef LIGHTMAP_ON
		half2 lightmapUV   : TEXCOORD3;
#endif
#if _FOG_ON
		UNITY_FOG_COORDS(4)
#endif
	};

	v2f vert(appdata_full v) {
		v2f o;
		o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.pos = UnityObjectToClipPos(v.vertex);

		o.normal.xyz = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
		o.normal.w = 0;

		half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.eyeDir = normalize(worldPos - _WorldSpaceCameraPos.xyz);
		o.lightColor = Shade4PointLights(worldPos, o.normal);    //点光

#ifdef LIGHTMAP_ON
		o.lightmapUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#if _FOG_ON
		UNITY_TRANSFER_FOG(o, o.pos);
#endif
		return o;

	}
	fixed4 frag(v2f i) : COLOR{


		//主光方向
		half3 lightDirection = _DirectionalLightDir.xyz;

		//法线
		half3 normal = i.normal;


		//高光
		half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))), _SpRange),0);
		splight *= _SpPower;


		//固有色
		fixed4 col = tex2D(_MainTex,i.uv0);


		half4 final;
		final.a = min(col.a, _Color.a);

		half3 mainlight = col.rgb*_DirectionalLightColor.rgb + col.rgb*splight + col.rgb* i.lightColor.rgb * 3;

		final.rgb = mainlight;

#ifdef LIGHTMAP_ON

		fixed3 lm = DecodeLogLuv(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV));
		final.rgb *= lm;

#endif

#ifdef CLIP_ON
		clip(col.a - _CutOut);
#endif


		final.rgb *= (_Lighting + _HdrIntensity)*_DirectionalLightColor*_DirectionalLightDir.w + UNITY_LIGHTMODEL_AMBIENT.xyz;
		float isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
		float3 grayCol = dot(final.rgb, float3(0.299, 0.587, 0.114));
		final.rgb = lerp(final.rgb*_Color.rgb, grayCol.rgb, isGray);


#if _FOG_ON
		UNITY_APPLY_FOG(i.fogCoord , final);
#endif
		/*float lum = (max(max(final.r, final.g), final.b) + min(min(final.r, final.g), final.b)) / 2;

		final.a = pow(clamp(lum,0, 1), 1.0) * 1.8;*/

		return  final;

	}
		ENDCG
	}

	}

		SubShader{
		Tags{
		"RenderType" = "Transparent"
	}

		LOD 200

		UsePass "Custom/NoLight/Unlit - BaseObject/BASEOBJECT"

	}


}
