Shader "Hidden/PostEffect/RadialBlur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Level("Level",Range(1,100)) = 50
		_CenterX("Center.x",Range(0,1)) = 0.5
		_CenterY("Center.y",Range(0,1)) = 0.5
		_Range("Range",Range(0,0.5)) = 0.1
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

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Level;
				fixed _CenterX;
				fixed _CenterY;
				fixed _Range;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					//UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//	return fixed4(1,0,0,1);
					//设置径向模糊的中心位置，一般来说都是图片重心（0.5，0.5）
					fixed2 center = fixed2(_CenterX,_CenterY);

					//计算像素与中心点的距离，距离越远偏移越大
					fixed2 uv = i.uv - center;
					float3 col1 = fixed3(0,0,0);
					_Level = _Level/1000;
 
					//根据设置的level对像素进行叠加，然后求平均值
					fixed2 duv = 0;
 

					duv = min(1, max(0, uv*(1 - _Level * 0) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 1) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 2) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 3) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 4) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 5) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 6) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 7) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 8) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					duv = min(1, max(0, uv*(1 - _Level * 9) + center));
					col1 += tex2D(_MainTex, duv).rgb;

					fixed4 col = fixed4(col1.rgb * 0.1,1);

					fixed4	orgCol= tex2D(_MainTex, i.uv);
 
					col = lerp(orgCol, col, saturate((abs(_CenterX - i.uv.x) - _Range) * 3 + (abs(_CenterY - i.uv.y) - _Range) * 3));
					//return saturate((abs(_CenterX - i.uv.x) - _Range) * 3 + (abs(_CenterY - i.uv.y) - _Range) * 3);
					return  min(1, max(0, col));
				}
				ENDCG
			}//end pass
		}//end SubShader
	}//end Category
}//end Shader