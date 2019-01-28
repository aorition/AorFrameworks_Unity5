//@@@DynamicShaderInfoStart
//<readonly> NGUI 按UV Y方向做半透明过度 可反向 (1字头带裁切) (注意：Shader须配合UITextture使用)
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - UVMask 1" 
{

	Properties
	{
		[PerRendererData]_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_mask("Mask", Range(0,1)) = 0
		_range("Range", Range(0.01,1)) = 0
		[MaterialToggle] _reverse("MaskReverse", Float) = 0
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

			float4 _color;
			float _mask;
			float _reverse;
			float _range;

			//NGUI 1字头 -- 标配参数组
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
			//NGUI 1字头 -- 标配参数组 end

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
				float2 worldPos : TEXCOORD1;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color;
				o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				_mask = _mask * (1.0 + _range) - _range;
				float4 col = tex2D(_MainTex,i.uv)*i.color;
				col.a = min(col.a, saturate((i.uv.y * (1.0 - _reverse) + (1.0 - i.uv.y) * _reverse - _mask) / _range));

				clip(col.a);

				//NGUI 1字头 -- 标配裁切算法
				float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				col.a *= min(max(min(factor.x, factor.y), 0.0), 1.0);
				//NGUI 1字头 -- 标配裁切算法 end

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
}
