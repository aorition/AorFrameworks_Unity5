Shader "AFW/NGUI/NGUI - SoftClipMaskUvMove"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_CenterOffset("Center Offset", float) = 1
		_Softness("Softness", float) = 120
		_width("Width", float) = 0
		_XOffset("XOffset", float) = 0
		_YOffset("YOffset", float) = 0
		_Scale("Sclae", float) = 1
	}

	SubShader
	{
		LOD 200

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

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

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;

			half _CenterOffset;
			half _width;
			half _Softness;

			float _XOffset;
			float _YOffset;
			float _Scale;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				fixed gray : TEXCOORD2;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = v.texcoord;
				o.gray = step(dot(v.color, fixed4(1,1,1,0)),0);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// Sample the texture
				half4 col = tex2D(_MainTex, float2((i.uv.x - _XOffset)/ _Scale, (i.uv.y - _YOffset)/ _Scale ));

				//mask
				float4 mask = tex2D(_MaskTex, i.uv);

				//渐变
				float2 uv = i.uv.xy;
				col.a = min(1 - pow((abs(uv.y - _CenterOffset) *_width), _Softness), col.a)*mask.r;

				fixed4 gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

				gray.a = col.a* i.color.a;
				col = lerp(col* i.color, gray, i.gray);

				col.a = max(0, col.a);

				return col;
			}
			ENDCG
		}//end Pass
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
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
			{
				Combine Texture * Primary
			}
		}//end pass
	}//end SubShader
}//end Shader
