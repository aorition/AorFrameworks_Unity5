//@@@DynamicShaderInfoStart
//海岸线shader
//@@@DynamicShaderInfoEnd

Shader "Custom/longzhu/Unlit - Coastline" {


	Properties{
	 _MainTex("Texture", 2D) = "white" { }
	_WaveLoop("WaveLoop", float) = 10
	_Color("Color", Color) = (1,1,1,1)
	_Lighting("Lighting",  float) = 1
	_CutOut("CutOut", float) = 0.1

	}


		SubShader{

		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }


		Pass
		{
		Blend SrcAlpha OneMinusSrcAlpha,SrcAlpha One 
		ZTest Less
		ZWrite Off
		Cull Back

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/ObjectBaseShader.cginc"
				#pragma multi_compile FOG_OFF FOG_ON 

				float _WaveLoop;


				struct v2f {
					half4  pos : SV_POSITION;
					float2  uv : TEXCOORD0;
					fixed4 color : color;
					#ifdef FOG_ON		
					float3 viewpos: TEXCOORD1;
					#endif

				};

				struct appdata {
					float4 vertex : POSITION;
					float2 texcoord:TEXCOORD0;
					fixed4 color : color;
				};

				//顶点函数没什么特别的，和常规一样 
				v2f vert(appdata v)
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.color = v.color;

					#ifdef FOG_ON		
					o.viewpos = mul(UNITY_MATRIX_MV, v.vertex);
					#endif
 
					return o;
				}

				float4 frag(v2f i) : COLOR
				{
				 
					//float y = 0.5 + sin(_time *(_WaveLoop))*0.5 + i.color.g;
					float y = _Time * _WaveLoop - floor(_Time * _WaveLoop) + i.color.g;
				 
					float y1 = i.uv.y - y*(1 - i.color.r);
					fixed4 col = tex2D(_MainTex,float2(i.uv.x,max(i.color.r,y1)));
					// return fixed4(max(i.color.r, i.uv.y - y), max(i.color.r, i.uv.y - y), max(i.color.r, i.uv.y - y), 1);
					col = col* _Color;
					col.rgb *= _Lighting + _HdrIntensity;
					col.a = min(i.color.a , col.a);
					col.a = min(max(0,1 - y), col.a);

				#ifdef FOG_ON
				float fogFactor = max(length(i.viewpos.xyz) + _fogDestance, 0.0);
				col.a = saturate(col.a* exp2(-fogFactor / _fogDestiy));
				 #endif

				return col;
				}


				ENDCG
			}


	}
}