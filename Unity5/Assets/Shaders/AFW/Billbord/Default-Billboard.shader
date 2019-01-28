//@@@DynamicShaderInfoStart
//<readonly> 范例Billboard Shader :: 此Shader将导致动态合并失效,慎用!
//@@@DynamicShaderInfoEnd
Shader "AFW/Billbord/Billboard - Default"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		//_Time ("Time", Float) = 0
		//[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{

		Tags
		{
			"Queue" = "Transparent"
			//"SortingLayer" = "Resources_Sprites"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"DisableBatching" = "True" //这个Tag很重要,Batching会导致Billboard顶点位置算错.
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#include "UnityCG.cginc"

			#pragma target 2.0

			//uniform Float _Time;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos   : POSITION;
				fixed4 color : COLOR;
				float2 uv  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			fixed4 _Color;

			v2f vert(appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color * _Color;

				#ifdef PIXELSNAP_ON
					o.pos = mul(UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					- float4(v.vertex.y, v.vertex.x, 0.0, 0.0)
					* float4(0.1, 0.1, 1.0, 1.0));
					// o.vertex = UnityPixelSnap (o.vertex);
				#endif

				return o;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
					// get the color from an external texture (usecase: Alpha support for ETC1 on android)
					color.a = tex2D(_AlphaTex, uv).r;
				#endif

				return color;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(i.uv) * i.color;
				c.rgb *= c.a;
				return c;
			}

			ENDCG
		}//end pass
	}//end SubShader
}//end Shader