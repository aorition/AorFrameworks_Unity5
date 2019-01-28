//@@@DynamicShaderInfoStart
//<readonly> NGUI 简单的UV双色控制 (注意：Shader须配合UITextture使用)
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - UVColor"
{
	
	Properties
	{
		[PerRendererData]_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_ColorStart("Start Color", Color) = (1,1,1,1)
		_ColorEnd("End Color", Color) = (1,1,1,1)
		_lerp("Lerp", Range(0,1)) = 0
	}

	SubShader
	{
		LOD 200

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Lighting Off
			Fog{ Mode Off }
			Offset -1, -1
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _ColorStart;
			fixed4 _ColorEnd;
			float _lerp;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float lp = saturate((1 - i.uv.y)*_lerp * 10);
				fixed4 lerpCol = lerp(_ColorStart,_ColorEnd,lp);
				fixed4 col = tex2D(_MainTex, i.uv) * i.color * lerpCol;

				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader

	SubShader
	{
		LOD 100

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
			{
				Combine Texture * Primary
			}
		}//end pass
	}//end SubShader
}//end Shader
