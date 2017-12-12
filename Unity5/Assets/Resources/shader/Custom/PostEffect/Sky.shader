// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//@@@DynamicShaderInfoStart
//自发光贴图材质 支持上色和亮度 一般特效贴图用此shader
//@@@DynamicShaderInfoEnd


//@@@DynamicShaderTitleRepaceStart
Shader "Hidden/Sky"  {
	//@@@DynamicShaderTitleRepaceEnd

	Properties{
	 _MainTex("Texture", 2D) = "white" { }
	_Color("Color", Color) = (1,1,1,1)
	_Lighting("Lighting",  float) = 1


	}


		SubShader{

	Tags {   "Queue" = "Transparent" }


		Pass
		{

	Blend OneMinusDstAlpha DstAlpha,Zero DstAlpha
	ZTest Always
	ZWrite Off
	Cull Off
	Fog {mode Off}


				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"


				sampler2D _MainTex;
				sampler2D _CurveTex;
				float4 _MainTex_ST;
				  float4 _Color;
				float _Lighting;





				struct v2f {
					float4  pos : SV_POSITION;
					float2  uv : TEXCOORD0;
					float4 color : COLOR;


				}

	;
				struct appdata {
					float4 vertex : POSITION;
					float2 texcoord:TEXCOORD0;
					float4 color : COLOR;
				}

	;
				//顶点函数没什么特别的，和常规一样 
				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.color = v.color;
					return o;
				}



				float4 frag(v2f i) : COLOR
				{
					float4 col = tex2D(_MainTex,i.uv);
					col = col* _Color*i.color;
					col.rgb = col.rgb*_Lighting;
					return col;
				}


				ENDCG
			}


	}
}