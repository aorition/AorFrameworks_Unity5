Shader "Hidden/PostEffect/GaussBlur" 
{
	Properties
	{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Offset("Level",Range(0,2)) = 0.01
	}

	Category
	{

		SubShader
		{
    
			Pass
			{

				ZTest Always

				Fog{ Mode off }

				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag

				sampler2D _MainTex;
				float4 _MainTex_ST;

				float _Offset;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 pos : POSITION;
					float2 uv : TEXCOORD0;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

					float4 tex = tex2D(_MainTex, i.uv);
		
					float3 col=0;
					col.rgb = tex* 2;

					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(_Offset, 0)))).rgb;
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(-_Offset, 0)))).rgb;
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(0, _Offset)))).rgb;
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(0, -_Offset)))).rgb;
		
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(_Offset, _Offset) * 0.5))).rgb;
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(-_Offset, _Offset) * 0.5))).rgb;
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(_Offset, -_Offset) * 0.5))).rgb;
					col += tex2D(_MainTex, min(1, max(0, i.uv + float2(-_Offset, -_Offset) * 0.5))).rgb;
		
					fixed4 final = fixed4(col * 0.1, tex.a);
		
					return max(0, final);
				}
				ENDCG
			}//end pass
		}//end SubShader
	}//end Category
}//end Shader