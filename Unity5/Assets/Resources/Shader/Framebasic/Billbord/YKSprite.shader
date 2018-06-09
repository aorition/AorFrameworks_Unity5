// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//支持自定义图集的Billbord
//@@@DynamicShaderInfoEnd


Shader "Custom/Other/YKSprite" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Pass{
			Tags { "RenderType"="Transparent" }
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
			CGPROGRAM
			#pragma multi_compile FOG_OFF FOG_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets/ObjectBaseShader.cginc"

			struct vertexData {
				float4  vertex : POSITION;
				float4  color : COLOR0;
				float2  texcoord : TEXCOORD0;
			};

			struct v2f {
				float4  pos : SV_POSITION;
				float4  color : COLOR;
				float2  uv : TEXCOORD2;
				
				#ifdef FOG_ON		
					half fogFactor : TEXCOORD3;
				#endif
			};

			v2f vert(vertexData v)
			{
			    v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
			  	o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.color = v.color;
				#ifdef FOG_ON
					float4 viewpos = mul(UNITY_MATRIX_MV, v.vertex);
					o.fogFactor = max(length(viewpos.xyz) + _fogDestance, 0.0);
				#endif
				return o;
			}
			      
			float4 frag (v2f i) : COLOR
			{
				float2 uv = i.uv;
				
				float4 mainCol = tex2D(_MainTex, uv);
				if (mainCol.a < 0.1)
				{
					discard;
				}
				#ifdef FOG_ON
					mainCol.a = exp2(-i.fogFactor / _fogDestiy);
				#endif
					return mainCol * _Color * i.color;
			}
			ENDCG
		}
	} 
}
