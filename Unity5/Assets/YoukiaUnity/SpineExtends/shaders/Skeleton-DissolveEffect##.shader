// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spine/Skeleton-DissolveEffect##"
{

	Properties{

		_Color("Color", Color) = (1,1,1,1)
		_DissolveColor("Dissolve Color", Color) = (0,0,0,0)
		_DissolveEdgeColor("Dissolve Edge Color", Color) = (1,1,1,1)
		_MainTex("Base 2D", 2D) = "white"{}
		_DissolveMap("DissolveMap", 2D) = "white"{}
		_DissolveThreshold("DissolveThreshold", Range(0,1)) = 0
		_ColorFactor("ColorFactor", Range(0,1)) = 0.7
		_DissolveEdge("DissolveEdge", Range(0,1)) = 0.8

		[Enum(Off,0, On,1)] _zWrite("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 4
		//
		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 0

	}

	SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{

			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
			ZWrite[_zWrite]
			ZTest[_zTest]
			Cull[_cull]

			CGPROGRAM

			#include "Lighting.cginc"  

			#pragma vertex vert  
			#pragma fragment frag     

			uniform fixed4 _Color;
			uniform fixed4 _DissolveColor;
			uniform fixed4 _DissolveEdgeColor;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _DissolveMap_ST;
			uniform sampler2D _DissolveMap;
			uniform float _DissolveThreshold;
			uniform float _ColorFactor;
			uniform float _DissolveEdge;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};

			v2f vert(appdata_base v)
			{
				v.vertex.y += _DissolveThreshold * 0.25;
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _DissolveMap);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//采样Dissolve Map  
				fixed4 dissolveValue = tex2D(_DissolveMap, i.uv2);
				//小于阈值的部分直接discard  
				if (dissolveValue.r < _DissolveThreshold)
				{
					discard;
				}
				fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
				fixed3 color = tex.rgb;

				//优化版本，尽量不在shader中用分支判断的版本,但是代码很难理解啊....  
				float percentage = _DissolveThreshold / dissolveValue.r;
				//如果当前百分比 - 颜色权重 - 边缘颜色  
				float lerpEdge = sign(percentage - _ColorFactor - _DissolveEdge);
				//貌似sign返回的值还得saturate一下，否则是一个很奇怪的值  
				fixed3 edgeColor = lerp(_DissolveEdgeColor.rgb, _DissolveColor.rgb, saturate(lerpEdge));
				//最终输出颜色的lerp值  
				float lerpOut = sign(percentage - _ColorFactor);
				//最终颜色在原颜色和上一步计算的颜色之间差值（其实经过saturate（sign（..））的lerpOut应该只能是0或1）  
				fixed3 colorOut = lerp(color, edgeColor, saturate(lerpOut));
				return fixed4(colorOut, tex.a);

			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}