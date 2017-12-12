// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/longzhu/NGUI-PowerGlassse"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_ScanLine("Base (RGB), Alpha (A)", 2D) = "black" {}
 
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
			Fog { Mode Off }
 
			Blend DstColor Zero,SrcAlpha Zero

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			sampler2D _ScanLine;
		 
			float4 _MainTex_ST;
			float4 _ScanLine_ST;
	 
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 texcoord_scan : TEXCOORD1;
 
				fixed4 color : COLOR;
			};

			v2f o;

			v2f vert(appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.texcoord_scan = TRANSFORM_TEX(v.texcoord, _ScanLine);
 
				o.color = v.color;
	 
				return o;
			}

			float2 clampUV(float2 uv) {

				return 	uv = float2(uv.x - floor(uv.x), uv.y - floor(uv.y));
			}

			fixed4 frag(v2f IN) : SV_Target
			{
			fixed4 col = tex2D(_MainTex, IN.texcoord);

			float2 uv = IN.texcoord_scan;

			uv.y += _Time * 40;
			uv = clampUV(uv);

			fixed4 scan = tex2D(_ScanLine, uv);

 

			col.rgb = col.rgb * scan;
			col.rgb *= col.a * 3;
			clip(col.a - 0.5);
			//return col;
			  return  fixed4(col.r*1.8,col.g*1, col.b*0.5,col.a);	
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
			 
				//ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMaterial AmbientAndDiffuse

				SetTexture[_MainTex]
				{
					Combine Texture * Primary
				}
			}
			}
}
