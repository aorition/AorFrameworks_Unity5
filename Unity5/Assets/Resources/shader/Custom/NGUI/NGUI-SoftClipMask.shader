// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NGUI/NGUI-SoftClipMask"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_CenterOffset("Center Offset", float) = 1
		_Softness("Softness", float) = 120
		_width("Width", float) = 0
	}

		SubShader
		{
			LOD 200

			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}

			Pass
			{
				Cull Off
				Lighting Off
				ZWrite Off
			 	Offset -1, -1
				Fog { Mode Off }
				ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _MaskTex;
				half _CenterOffset;
				half _width;
				half _Softness;

				struct appdata_t
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					fixed gray : TEXCOORD2;
				};

				v2f o;

				v2f vert(appdata_t v)
				{
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = v.texcoord;
					o.gray = step(dot(v.color, fixed4(1,1,1,0)),0);
					return o;
				}

				half4 frag(v2f IN) : COLOR
				{
					// Sample the texture
					half4 col = tex2D(_MainTex, IN.texcoord);//tex2D(_MainTex, IN.texcoord) * IN.color;

					//mask
					float4 mask = tex2D(_MaskTex, IN.texcoord);

					//渐变
					float2 uv = IN.texcoord.xy;
					col.a = min(1 - pow((abs(uv.y - _CenterOffset) *_width), _Softness), col.a)*mask.r;

					fixed4 gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

					gray.a = col.a* IN.color.a;
					col = lerp(col* IN.color, gray, IN.gray);

					return col;
				}
				ENDCG
			}
		}

			SubShader
				{
					LOD 100

					Tags
					{
						"Queue" = "Transparent"
						"IgnoreProjector" = "True"
						"RenderType" = "Transparent"
					}

					Pass
					{
						Cull Off
						Lighting Off
						ZWrite Off
						Fog { Mode Off }
						ColorMask RGB
						Blend SrcAlpha OneMinusSrcAlpha
						ColorMaterial AmbientAndDiffuse

						SetTexture[_MainTex]
						{
							Combine Texture * Primary
						}
					}
				}
}
