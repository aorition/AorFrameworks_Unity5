// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//基础Toon
//@@@DynamicShaderInfoEnd


Shader "Custom/Light/Diffuse - Toon" {
	Properties{
	_MainTex("MainTex", 2D) = "white" {}
	_MaskTex("Mask(Must No mipMap)", 2D) = "black" {}
	//_ToonShade("ToonShader", 2D) = "white" {}
	_HideFace("_HideFace", int) = 0
	//[Toggle] _Fog("Fog?", Float) = 0
	 _Color("Color", Color) = (1,1,1,1)

 
	[HideInInspector] _Lighting("Lighting", float) = 1
	[HideInInspector] _CutOut("CutOut", float) = 0.1

	}
		SubShader{
				Tags {
					"RenderType" = "Transparent"
				}

		Pass {
			Name "BASETOON"

			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma multi_compile CLIP_OFF CLIP_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "Assets/ObjectBaseShader.cginc"	


				float _HideFace;
				sampler2D _MaskTex;
				float _UILight;


				struct appdata {
					float4 vertex : POSITION;
					float2 texcoord:TEXCOORD0;
					half4 color : COLOR;
					half3 normal : NORMAL;
				};


				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					half3 normal   : TEXCOORD1;
					half4 color : COLOR;
		 
				};

				v2f vert(appdata v) {
					v2f o;
					o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

					return o;

					}
				fixed4 frag(v2f i) : COLOR {


					//主光方向
						half3	lightDirection = lerp(_RoleDirectionalLightDir.xyz, half3(0.6, 0.3, -1), _UILight);
						//法线
						half3 normal = i.normal;
						 //卡通分界线
						 half d = max(0, dot(normal, lightDirection));

						 half  diff=d;

						 //固有色
						fixed4 col = tex2D(_MainTex,i.uv0);
						half mask = tex2D(_MaskTex, i.uv0).r;

						d = step(0.2, d)*mask.r;
						half3  ramp = lerp(i.color.rgb, fixed3(1,1,1), saturate(d));

					   //最终合成
					   half4 final;
					   final.a = col.a;
					   half3 mainlight =  ramp*_DirectionalLightDir.w*_DirectionalLightColor.rgb;

					   final.rgb = col.rgb * (mainlight  + diff*0.4);
				
					   clip(i.color.a - _HideFace);

				   #ifdef CLIP_ON
					   clip(col.a - _CutOut);
				   #endif

					   final.rgb *= (_Lighting + _HdrIntensity);
					   half isGray = step(dot(_Color.rgb, fixed4(1, 1, 1, 0)), 0);
					   half3 grayCol = dot(final.rgb, half3(0.299, 0.587, 0.114));
					   final.rgb = lerp(final.rgb*_Color.rgb, grayCol.rgb, isGray);


						  return  final;

							  }
						  ENDCG
						  }

	}

}
