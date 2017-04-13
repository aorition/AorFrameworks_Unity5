#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


Shader "Custom/Light/Diffuse - Toon" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask Tex", 2D) = "black" {}
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
	_outline("Outline Range", Range(1,8)) = 4
	_ToonShade("ToonShader", 2D) = "white" {}
	_AddColor("Add Color", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{
		"RenderType" = "Transparent"
	}



		Pass{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#pragma multi_compile CLIP_OFF CLIP_ON
#pragma multi_compile FOG_OFF FOG_ON 
#pragma vertex vert
#pragma fragment frag
#include "Assets/ObjectBaseShader.cginc"	

		float _outline;
	float4	_OutlineColor;

	//	float _backRange;
	sampler2D _ToonShade;
	sampler2D _MaskTex;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
		float3 normal   : TEXCOORD2;
		float3 eyeDir   : TEXCOORD3;
		fixed3 lightColor : TEXCOORD4;
		half4 Tangent : TEXCOORD5;
		half4 color : color;
	};

	v2f vert(appdata_full v) {
		v2f o;
		o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;

		o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
		float3 worldN = mul((float3x3)unity_ObjectToWorld, v.normal * 1.0);


		half4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.eyeDir.xyz = normalize(worldPos - _WorldSpaceCameraPos.xyz).xyz;
		o.lightColor = Shade4PointLights(worldPos, o.normal);
		o.Tangent = float4(mul(v.tangent.xyz, (float3x3)unity_WorldToObject), v.tangent.w);
		return o;
	}
	fixed4 frag(v2f i) : COLOR{
		half4 mask = tex2D(_MaskTex, i.uv0);

		half3 lightDirection = _DirectionalLightDir.xyz;

		half3 normal = normalize(i.normal);


		half d = max(0, dot(normal, lightDirection));
		half3 ramp = tex2D(_ToonShade, float2(d,d)).rgb;

		ramp = pow(ramp,3);
		//分区域
		half area = lerp(ramp.r, ramp.g, mask.r);
		ramp = lerp(i.color.rgb, fixed3(1, 1, 1), area);

		//高光
		half splight = max(pow(saturate(dot(normal , normalize(lightDirection.xyz - i.eyeDir))),  mask.b * 124 + 4),0)* mask.b;

	//	return fixed4(splight, splight, splight, 1);
		//描边
		half eyeD = dot(normal, -i.eyeDir);
		half outlight = pow(clamp(1 - eyeD, 0, 1), _outline)*_OutlineColor.a;
		//return fixed4(1 - eyeD, 1 - eyeD, 1 - eyeD,1);
		fixed4 col = tex2D(_MainTex,i.uv0);

		//环境光
		fixed3	ambientLight = UNITY_LIGHTMODEL_AMBIENT.xyz * 2;
		//3方向环境光

		//		half rate = 0.5* dot(i.normal, fixed3(0, 1, 0)) + 0.5;
		//		float3 BM = lerp(fixed3(0.3, 0.1, 0), fixed3(0, 0.2, 0), rate);
		//		float3 UM = lerp(fixed3(0, 0.2, 0), fixed3(0, 0.1, 0.4), rate);
		//	 	ambientLight = lerp(BM, UM, rate);


		half4 final;
		final.a = 1;
		half3 mainlight = col.rgb*(ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb + i.lightColor.rgb* eyeD + ambientLight.rgb);


		final.rgb = mainlight;
		final.rgb += splight*mask.g * 2;
		final.rgb = lerp(final.rgb, _OutlineColor, outlight);
		return  final;
	}
		ENDCG
	}

	}


		//	FallBack "Diffuse"
		//   CustomEditor "ShaderForgeMaterialInspector"
}
