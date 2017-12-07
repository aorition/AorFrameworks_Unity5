Shader "Custom/UI/Image-Color-lighting##" {

	Properties{

		_Texture("Texture", 2D) = "white" {}
		
		_TintColor("Tint Color", Color) = (1,1,1,1)

		[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
		[HideInInspector]_ColorMask("Color Mask", Float) = 15

	}

	SubShader{
		
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Pass
		{
			Lighting Off
			Fog{ Mode Off }
			//@@@DynamicShaderBlendRepaceStart
			Cull Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			//@@@DynamicShaderBlendRepaceEnd
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			sampler2D _Texture;
			float4 _Texture_ST;

			float4 _TintColor;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				half2 uv  : TEXCOORD0;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.uv = TRANSFORM_TEX(IN.uv, _Texture);
				o.color = IN.color;
				#ifdef PIXELSNAP_ON
						o.vertex = UnityPixelSnap(IN.vertex);
				#endif

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{

				fixed4	col = tex2D(_Texture, i.uv) * i.color * _TintColor;
				return  col;
				
			}

			ENDCG
		}


	}
}