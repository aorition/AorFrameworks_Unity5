Shader "Custom/Sprites/Base Fog"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	Category
	{
		
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		SubShader
		{
			Pass
			{
				
				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile DUMMY PIXELSNAP_ON
				#pragma multi_compile FOG_OFF FOG_ON 
			
				sampler2D _MainTex;

				#ifdef FOG_ON
					float _fogDestiy;
					float _fogDestance;
				#endif

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
					#ifdef FOG_ON		
						float3 viewpos: TEXCOORD1;
					#endif
				};

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif
					#ifdef FOG_ON		
						OUT.viewpos = mul(UNITY_MATRIX_MV, IN.vertex);
					#endif
					return OUT;
				}

				fixed4 frag(v2f IN) : COLOR
				{
					fixed4 col= tex2D(_MainTex, IN.texcoord) * IN.color;

					#ifdef FOG_ON
						float fogFactor = max(length(IN.viewpos.xyz) + _fogDestance, 0.0);
						col.a = col.a* exp2(-fogFactor / _fogDestiy);
					#endif
					return col;
				}
				ENDCG
			}//end pass
		}//end SubShader
	}//end Category
}//end Shader