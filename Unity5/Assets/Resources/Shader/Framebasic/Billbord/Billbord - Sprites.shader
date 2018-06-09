Shader "FrameBase/Billbord/Billbord - Sprites"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				float4x4 mvMat = UNITY_MATRIX_MV;
				float3 scale = float3(length(float3(mvMat[0][0], mvMat[0][1], mvMat[0][2])), length(float3(mvMat[1][0], mvMat[1][1], mvMat[1][2])), length(float3(mvMat[2][0], mvMat[0][1], mvMat[0][2])));
				mvMat = float4x4(scale.x, 0, 0, mvMat[0][3],
					0, scale.y, 0, mvMat[1][3],
					0, 0, scale.z, mvMat[2][3],
					0, 0, 0, 1);
				float4x4 mvpMat = mul(UNITY_MATRIX_P, mvMat);
				OUT.vertex = mul(mvpMat, IN.vertex);

				//OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);

				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
