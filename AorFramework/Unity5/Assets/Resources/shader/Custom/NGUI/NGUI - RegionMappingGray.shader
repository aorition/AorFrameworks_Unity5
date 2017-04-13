//自定义部分变灰

Shader "Custom/NGUI/NGUI-RegionMapping-sGray" {

	Properties{
		_MainTex ("MainTex", 2D) = "white" {}
		_RegionTex ("RegionTex", 2D) = "white" {}
		_MappingTex ("MappingTex", 2D) = "white" {}
		_Lum("Lum", Range(0, 1)) = 0.5
	}

	SubShader{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass{
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog{ Mode Off }
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			//ColorMask RGB
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers d3d11 xbox360 ps3 flash
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _RegionTex;
			sampler2D _MappingTex;
			float _Lum;
			float4 _MainTex_ST;

			struct v2f {
				float4 pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			v2f vert(appdata_tan v)
			{
				v2f o;

				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float offset = 1 / 256 * 0.5;

				float3 color = tex2D(_MainTex, i.uv).xyz;
				float3 gray = Luminance(color) * _Lum;
				float3 region = tex2D(_RegionTex, i.uv).xyz;
				//float4 mapping = tex2D(_MappingTex, i.uv).xyz;
				float3 mapping = tex2D(_MappingTex, float2(region.r + offset, offset)).xyz;
				
				float3 finial = lerp(color, gray, mapping.r);
				return float4(finial, 1);
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
			Fog{ Mode Off }
			Offset -1, -1
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