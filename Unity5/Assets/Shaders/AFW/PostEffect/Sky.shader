//@@@DynamicShaderInfoStart
//<readonly> Sky用shader
//@@@DynamicShaderInfoEnd
Shader "Hidden/Sky"  
{

	Properties
	{
		 _MainTex("Texture", 2D) = "white" { }
		 _Color("Color", Color) = (1,1,1,1)
		 _Lighting("Lighting",  float) = 1
	}

	SubShader
	{
		
		Tags { "Queue" = "Transparent" }

		Pass
		{

			Blend OneMinusDstAlpha DstAlpha,Zero DstAlpha
			ZTest Always
			ZWrite Off
			Cull Off
			Fog {mode Off}

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			sampler2D _CurveTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Lighting;
			
			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float4 color : color;
			};

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 texcoord:TEXCOORD0;
				float4 color : color;
			};

			//顶点函数没什么特别的，和常规一样 
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex,i.uv);
				col = col* _Color*i.color;
				col.rgb = col.rgb*_Lighting;
				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader