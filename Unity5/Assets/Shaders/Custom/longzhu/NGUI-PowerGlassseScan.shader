// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/longzhu/NGUI-PowerGlassseScan"
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
 
			Blend SrcColor One 

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

			uv.x -= _Time ;
			uv = clampUV(uv);

			fixed4 scan =   tex2D(_ScanLine, uv);
			scan = lerp(fixed4(0, 0, 0, 0), scan, col.a);
 
			  return  scan;
			}
			ENDCG
		}
	}

}
