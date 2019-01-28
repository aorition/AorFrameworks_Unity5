Shader "Hidden/PostEffect/Reverse" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	Category
	{
		SubShader 
		{
			Pass 
			{
				ZTest Always
				Fog { Mode off }

				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag

				sampler2D _MainTex;
				float4 _MainTex_ST;

				struct v2f {
					float4  pos : SV_POSITION;
					float2  uv : TEXCOORD1;
				};

				//顶点函数没什么特别的，和常规一样
				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.uv.y = 1 - o.uv.y;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 mainCol = tex2D(_MainTex, i.uv);
					return mainCol;
				}
				ENDCG
			}//end pass
		}//end SubShader
	}//end Category
}//end Shader